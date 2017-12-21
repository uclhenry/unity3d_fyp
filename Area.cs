using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine;


[XmlRoot("Area")]
public class Area
{
	public string Id;
	public string Name;
	public string ModifiedDate;

	[XmlArray("Pois"),XmlArrayItem("Poi")]
	public POI[] POIs;

 	//-------------------------------XML serializer/deserializer--------------------------------------------//
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(Area));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static Area Load()
	{
		string LastTime="";
		string LastTimeNew="";

		if(Application.loadedLevelName == "ExperimentNavigation")
		{
			if(!File.Exists(SceneTools.AreaZipFileLocal()))
			{
			   SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());
				new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);				
			}
		}
		else //the usual
		{
			if(File.Exists (SceneTools.AreaZipLastTimeLocal()))
			{
				LastTime = File.ReadAllText(SceneTools.AreaZipLastTimeLocal());
			
				SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipLastTimeUrl(), SceneTools.AreaZipLastTimeLocal() );
					
				LastTimeNew = File.ReadAllText(SceneTools.AreaZipLastTimeLocal());
						
				if(Convert.ToInt64(LastTimeNew) > Convert.ToInt64 (LastTime))
				{
					Debug.Log ("Downloading new zip file");
					SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());
					new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);		
				}
				else 
				{
					Debug.Log ("Zip file is latest version");
                    Debug.Log("Downloading new zip file");
                    SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());
                    new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
                }
			}
			else
			{
                Debug.Log("Zip not File.Exists (SceneTools.AreaZipLastTimeLocal())");
                SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipLastTimeUrl(), SceneTools.AreaZipLastTimeLocal() );
				SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());
				new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);				
			}
		}

		var serializer = new XmlSerializer(typeof(Area));

		using(var stream = new FileStream(Path.Combine(Application.persistentDataPath + "/" + SceneTools.AreaNameDefault(), SceneTools.AreaNameDefault() + ".xml" ), FileMode.Open))
		{
			return serializer.Deserialize(stream) as Area;
		}
	}
	//-------------------------end---XML serializer/deserializer--------------------------------------------//
	
	public POI FindPoi(int cosId)
	{
		foreach(POI poi in POIs)
			if (poi.GetCosId() == cosId) 
				return poi;
				
		return null;
	}
}

[XmlRoot("Pois")]
public class POI
{
	public string Id;
	public string Name;
	public string ImageTarget;
	public double Latitude;
	public double Longitude;
	public string TargetHeight;
	public string TargetWidth;
	public float  SimilarityThreshold;
	public string userId;
	
	[XmlArray("ContentContainers"),XmlArrayItem("ContentContainer")]
	public ContentContainer[] ContentContainers;


	//non-serializable properties and attributes
	GameObject 		  markerObj;
	public GameObject GetMarkerObj(){return markerObj;}
	public void 	  SetMarkerObj(GameObject obj){markerObj = obj;}
	
	GameObject 		  mapObj;
	public GameObject GetMapObj(){return mapObj;}
	public void 	  SetMapObj(GameObject obj){mapObj = obj;}
	
	int		 		  cosId;
	public int GetCosId(){return cosId;}
	
	public GPSlocation GetGPSlocation()
	{
		return new GPSlocation(Latitude,Longitude);
	}
	
	
	public void Instantiate(GameObject areaObject, int newCosId)
	{
		markerObj = new GameObject();
		markerObj.transform.parent = areaObject.transform;
		markerObj.name = "Poi_"+Name;
		//metaioTracker script = (metaioTracker)markerObj.AddComponent<metaioTracker>();
		markerObj.tag = "POI";
		cosId = newCosId;
		//script.cosID = cosId; //first cosID must be 1

        //if (Eras.Length > 1) //Do not hide slider when there's only 1 era
        {
			//ControlContentContainers ccs = markerObj.AddComponent<ControlContentContainers>();
            //ccs.SetPoi(this);
        }
        
		foreach (ContentContainer cc in ContentContainers)
			cc.Instantiate (markerObj);
    }

}

public class ContentContainer
{
	public string Id;
	public string Name;
	public string Description;

	[XmlArray("Contents"),XmlArrayItem("Content")]
	public Content[] Contents;

	public void Instantiate(GameObject poi)
	{
		GameObject ContCont = new GameObject ();
		ContCont.transform.parent = poi.transform;
		ContCont.name = Name;

		if(Contents != null) //arp, skip the empty content containers
		foreach (Content c in Contents) 
		{
			c.Instantiate (ContCont);
		}
	}

}

public class Content
{
	public string 			Id;
	public PoiDataType   	Type;
	public string 			Description;
	public PositionVector 	Position;
	public RotationVector 	Rotation;
	public ScaleVector    	Scale;

	public void Instantiate(GameObject contentContainer)
	{
        GameObject content;

        switch(Type)
		{
		case PoiDataType.texture2D:
			content 					= (GameObject) GameObject.CreatePrimitive(PrimitiveType.Plane);
			content.transform.parent 	= contentContainer.transform;
			content.name 				= content.transform.parent.name + "_tex2D";
			//add content to touchinput layer to manipulate it with a touch screen
			//if the layer is set is that it seems to be rendered on the CameraMap... and that's a problem
			content.layer 				= LayerMask.NameToLayer("VisAgeContent");//SceneTools.ContentLayerMaskName());
			
			content.transform.position 	= Position.getVector3();
			content.transform.rotation 	= /*Rotation.getQuaternion() +*/ Quaternion.Euler(0f, 180f, 0f);
			//content.transform.localScale= Scale.getVector3();
			content.transform.localScale = new Vector3(100,100,100);
			
			string p = Path.Combine(Application.persistentDataPath,SceneTools.AreaNameDefault()+"/"+Description);
			if(System.IO.File.Exists(p))
			{
				Texture2D texture = new Texture2D(512,512);
				texture.LoadImage(System.IO.File.ReadAllBytes(p));
				content.GetComponent<Renderer>().material.mainTexture = texture;
				content.GetComponent<Renderer>().material.shader = Shader.Find("metaio/UnlitTexture");
			}
			else
			{
				Debug.LogWarning("Texture " + p + " does not exist");
			}
			break;
			
		case PoiDataType.object3D:
            content 					= new GameObject();
            content.transform.parent 	= contentContainer.transform;
            content.name 				= content.transform.parent.name + "_obj3D";
			//content.layer 				= LayerMask.NameToLayer(SceneTools.ContentLayerMaskName());
                
            //Model3D model3D 			= GameObject.Find("Object3DManager").GetComponent<Model3D>();
			//GameObject[] models 		= model3D.Load(Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "/"+Description));


			break;
		
        case PoiDataType.audio:
			content = new GameObject();
			content.transform.parent 	= contentContainer.transform;
			content.name 				= content.transform.parent.name + "_audio";
			//content.layer 				= LayerMask.NameToLayer(SceneTools.ContentLayerMaskName());
			
			content.transform.position 	= Position.getVector3();
			//content.transform.rotation = /*Rotation.getQuaternion();*/ Quaternion.Euler(0f, 180f, 0f);
			//content.transform.localScale = Scale.getVector3();
			//content.AddComponent<ControlAudio>();

            AudioSource asource = content.AddComponent<AudioSource>();
            asource.playOnAwake = false;
			WWW www = new WWW("file://" + Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "/"+Description));
            AudioClip clip = www.GetAudioClip();
            clip = www.GetAudioClip(false,false,AudioType.MPEG);
            clip.name = Description;            
            asource.clip = clip;
			break;
		
        case PoiDataType.text:
			content = new GameObject();
			content.transform.parent 	= contentContainer.transform;
			content.name 				= content.transform.parent.name + "_text";
			//content.layer 				= LayerMask.NameToLayer(SceneTools.ContentLayerMaskName());
			
			//content.transform.position = Position.getVector3();
			//content.transform.rotation = /*Rotation.getQuaternion();*/ Quaternion.Euler(0f, 180f, 0f);
			content.transform.localScale = Scale.getVector3();
			content.AddComponent<GUIText>();
			content.GetComponent<GUIText>().text = Description;
            break;

		default:
			Debug.LogWarning("POI type not implemented");
			break;
			
		}

		
	}
}

public enum PoiDataType
{
	texture2D = 0,
	object3D = 1,
	audio	 = 2,
	text	 = 3,
}

public class MyVector3
{
	public float x;
	public float y;
	public float z;

	public Vector3 	  getVector3(){return new Vector3 (x, y, z);}
	public Quaternion getQuaternion(){return Quaternion.Euler(x,y,z);}
}

[XmlRoot("Position")]
public class PositionVector : MyVector3{}
[XmlRoot("Rotation")]
public class RotationVector : MyVector3{}
[XmlRoot("Scale")]
public class ScaleVector : MyVector3{}


