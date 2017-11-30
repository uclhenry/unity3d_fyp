using UnityEngine;
using System.Collections;
using System.Linq;
using Vuforia;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;

using System;

public class DynamicDataSetLoader : MonoBehaviour
{
    private string _result;
    //public bool once = false;
    // specify these in Unity Inspector
    public GameObject augmentationObject = null;  // you can use teapot or other object
    public GameObject secondObject = null;
    public string dataSetName = "";  //  Assets/StreamingAssets/QCAR/DataSetName
    public bool once = false;
    public static List<POI> Pois = null;
    public List<GameObject> RenderObjects = null;
    public int i = 0;
    public GameObject arrow = null;
    public int renderArrow = 7;
    public List<List<POI>> closet = null;
    // Use this for initialization
    void Start()
    {
        
        //prepareRenderObjects();
        StartCoroutine(LoadXML());
        // Vuforia 6.2+
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
    }
    public void prepareRenderObjects() {
        RenderObjects = new List<GameObject>();
        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder));
        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Capsule));
        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));

        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        RenderObjects.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));

    }
    public void readExampleXml()
    {
        XElement result = XElement.Parse(_result);//
        //Debug.Log(result.ToString());
        IEnumerable<XElement> elements = from ele in result.Descendants("Pois").Elements("Poi")
                                         select ele;
        SetUpPois(elements);
        //createAlltext();
    }// static
    public void SetUpPois(IEnumerable<XElement> elements)
    {
        Pois = new List<POI>();

        //Debug.Log("inside ShowInfoByElements");
        foreach (var ele in elements)
        {
            //Debug.Log("a loop");
            //Debug.Log(ele.Element("Name").Value);
            POI point = new POI();
            point.Name = ele.Element("Name").Value;
            point.Id = ele.Element("Id").Value;
            point.ImageTarget = ele.Element("ImageTarget").Value;
            point.TargetHeight = ele.Element("TargetHeight").Value;
            point.TargetWidth = ele.Element("TargetWidth").Value;
            point.userId = ele.Element("userId").Value;
            String s = ele.Element("Latitude").Value;

            s = decimal.Round(decimal.Parse(s), 6).ToString();
            point.Latitude = s;
            s = ele.Element("Longitude").Value;
            s = decimal.Round(decimal.Parse(s), 6).ToString();
            point.Longitude = s;
            //point.Latitude = Convert.ToDouble(ele.Element("Latitude").Value);
            //point.Longitude = Convert.ToDouble(ele.Element("Longitude").Value);
            point.SimilarityThreshold = Convert.ToSingle(ele.Element("SimilarityThreshold").Value);
            Pois.Add(point);
        }//k c//k u
        closet = new List<List<POI>>();

        foreach (var p in Pois)
        {
            POI nearest = null;
            Decimal minDistance = 9999;

            foreach (var nb in Pois)
            {
                if (p != nb)
                {
                    Decimal diffLong = decimal.Parse(nb.Longitude) - decimal.Parse(p.Longitude);
                    Decimal diffLat = decimal.Parse(nb.Latitude) - decimal.Parse(p.Latitude);
                    Decimal currentDis = diffLat * diffLat + diffLong * diffLong;
                    if (minDistance > currentDis) {
                        nearest = nb;
                        minDistance = currentDis;
                    }

                    //Debug.Log("name:" + nb.Name + "," + p.Name);
                    //Debug.Log("Long:" + (decimal.Parse(nb.Longitude) - decimal.Parse(p.Longitude)).ToString());
                    //Debug.Log("Lat:" + (decimal.Parse(nb.Latitude) - decimal.Parse(p.Latitude)).ToString());

                }
            }
            List<POI> pair = new List<POI>();
            pair.Add(p);
            pair.Add(nearest);
            closet.Add(pair);
        }
        //foreach (var pair in closet) {
            
        //    POI nb = pair[1];
        //    POI p = pair[0];
        //    Decimal diffLong = decimal.Parse(nb.Longitude) - decimal.Parse(p.Longitude);
        //    Decimal diffLat = decimal.Parse(nb.Latitude) - decimal.Parse(p.Latitude);
        //   Debug.Log(pair[0].Name + "---" + pair[1].Name+"  "+diffLat+"  "+diffLong);
        //}
    }
     
    void LoadDataSet()
    {

        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(dataSetName))
        {

            objectTracker.Stop();  // stop tracker so that we can add new dataset

            if (!objectTracker.ActivateDataSet(dataSet))
            {
                // Note: ImageTracker cannot have more than 100 total targets activated
                Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
            }

            if (!objectTracker.Start())
            {
                Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
            }

            int counter = 0;

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in tbs)
            {
                if (tb.name == "New Game Object")
                {

                    // change generic name to include trackable name
                    tb.gameObject.name = ++counter + ":DynamicImageTarget-" + tb.TrackableName;

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    tb.gameObject.AddComponent<TurnOffBehaviour>();

                    if (augmentationObject != null)
                    {
                        // instantiate augmentation object and parent to trackable
                        GameObject augmentation = (GameObject)GameObject.Instantiate(augmentationObject);
                        //augmentation.transform.parent = tb.gameObject.transform;
                        augmentation.transform.SetParent(tb.gameObject.transform);
                        augmentation.transform.localPosition = new Vector3(0f, 0f, 0f);
                        Quaternion target = Quaternion.Euler(90f, 0, 0);
                        augmentation.transform.localRotation = target;// Quaternion.identity;

                        augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                        augmentation.gameObject.SetActive(true);
                        augmentation.name = tb.TrackableName;

                        GameObject second = (GameObject)GameObject.Instantiate(secondObject);
                        second.transform.parent = tb.gameObject.transform;
                        second.transform.localPosition = new Vector3(-1f, 0f, 1f);
                        second.gameObject.SetActive(true);
                        Quaternion target2 = Quaternion.Euler(90f, 0, 0);
                        second.transform.localRotation = target2;

                        //
                        //arrowObject.transform.parent = tb.gameObject.transform;

                    }
                    else
                    {
                        Debug.Log("<color=yellow>Warning: No augmentation object specified for: " + tb.TrackableName + "</color>");
                    }
 
                }
            }
        }
        else
        {
            Debug.LogError("<color=yellow>Failed to load dataset: '" + dataSetName + "'</color>");
        }
    }
    void RenderText(Transform board, POI p) {
        Text _currencyText = board.GetChild(0).gameObject.GetComponent<Text>();
        _currencyText.text = board.name;
        //Debug.Log("point" + p.Name);
        Text t2 = board.GetChild(1).gameObject.GetComponent<Text>();
        Text t3 = board.GetChild(2).gameObject.GetComponent<Text>();
        Text t4 = board.GetChild(3).gameObject.GetComponent<Text>();
        Text t5 = board.GetChild(4).gameObject.GetComponent<Text>();
        Text t6 = board.GetChild(5).gameObject.GetComponent<Text>();
        Text t7 = board.GetChild(6).gameObject.GetComponent<Text>();
        //find the poi with name = board.name
        //get the location and other infomation
        t2.text = "locate at " + p.Latitude + " ," + p.Longitude;
        t3.text = "id : " + p.Id;
        t4.text = "user id : " + p.userId;
        t5.text = "Target Height :" + p.TargetHeight;
        t6.text = "latitude : " + p.Latitude;
        t7.text = "longitude :" + p.Longitude;
    }
    void Update()
    {
        IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
        foreach (TrackableBehaviour tb in tbs)
        {
            //board from tb
            //find the p with same name 
            // tb.TrackableName == point Name
            Transform board = tb.gameObject.transform.GetChild(0);
            Transform Marker = tb.gameObject.transform.GetChild(1);
            TextMesh t = Marker.GetChild(2).gameObject.GetComponent<TextMesh>();
            if(renderArrow > 0){
            
                GameObject arrowObject = (GameObject)GameObject.Instantiate(arrow);
                arrowObject.SetActive(true);
                arrowObject.transform.parent = tb.gameObject.transform;
                renderArrow--;


            }
            
            //GetComponent<TextMesh>()
            //POI p = null;
            if (Pois != null) {
                //int i = 0;
                foreach (POI p in Pois)
                {
                    if (p.Name == tb.TrackableName)
                    {
                        if (p.rendered == false) {
                            RenderText(board, p);
                            p.rendered = true;
                            t.text = p.Name;


                        }
                        Decimal a = 0;
                        Decimal b = 0;
                        foreach (var pair in closet)
                        {
                            if (pair[0] ==p)
                            {
                                POI nb = pair[1];
                                Decimal diffLong = decimal.Parse(nb.Longitude) - decimal.Parse(p.Longitude);
                                Decimal diffLat = decimal.Parse(nb.Latitude) - decimal.Parse(p.Latitude);
                                Transform arrow = tb.gameObject.transform.GetChild(2);
                                arrow.LookAt(new Vector3((float)diffLong, (float)diffLat, 0));
                                arrow.GetChild(2).GetChild(0).GetComponent<Text>().text = nb.Name; 
                            }
                        }

                    }

                }
            }
            //new code start
            //if (i < RenderObjects.Count)
            //{
            //    GameObject g = RenderObjects[i];
            //    Debug.Log("add!!!!" + g.ToString());
            //    if (g.transform.parent == null)
            //    {
            //        Debug.Log("Set up BABA");
            //        g.transform.parent = tb.gameObject.transform;
            //        g.transform.localPosition = new Vector3(0f, 0f, 0f);
            //    }
            //    i++;
            //}
            //new code end
            


        }


    }
    IEnumerator LoadXML()
    {
        string sPath = "";
        if (Application.platform == RuntimePlatform.WindowsEditor) sPath = "file://" + Application.streamingAssetsPath + "/VisAge.xml";
        else if (Application.platform == RuntimePlatform.Android)
        {
            sPath = Application.streamingAssetsPath + "/VisAge.xml";
        }
        //sPath = "t";
        WWW www = new WWW(sPath);
        yield return www;
        _result = www.text;
        readExampleXml();
    }

}