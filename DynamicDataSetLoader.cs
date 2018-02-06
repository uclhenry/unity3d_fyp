using UnityEngine;
using System.Collections;
using System.Linq;
using Vuforia;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Text.RegularExpressions;
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
    public Area area;
    public Dictionary<String, int> SceneToRendered = new Dictionary<String, int>();
    public int SceneIndex = 0;
    // Use this for initialization
    void Start()
    {
        //get xml from server unzip 
        area = Area.Load();
        
        new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
        CreateTrackerPois();

        //prepareRenderObjects();
        StartCoroutine(LoadXML());
        // Vuforia 6.2+
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
        //GameObject test = GameObject.Find("Poi_Bentham");
        //String vuforiaTargetName = 
        //Transform poi = GameObject.Find("1:DynamicImageTarget-Patch00").transform;
        //test.transform.parent = poi;
        linkToVuforiaTarget();
    }
    void SetSceneIndex(int i) {
        SceneIndex = i;
    }
    int GetSceneIndex()
    {
        return SceneIndex;
    }
    void CreateTrackerPois()
    {
        GameObject areaObject = new GameObject();
        areaObject.name = "[Trackers]";
        int i = 0;
        foreach (POI poi in area.POIs)
        {
            poi.Instantiate(areaObject, ++i);
            SceneToRendered.Add(poi.Name, 0);
            Debug.Log(poi.Id);
        }
        foreach (var p in SceneToRendered) {
            Debug.Log(p);
        }
    }
    void GoNextScene(string name) {
        SceneToRendered[name] += 1; 
    }
    void linkToVuforiaTarget() {
        foreach (POI p in area.POIs)
        {         
            Debug.Log(p.Id);
            String vuforiaTarget = "DynamicImageTarget-" + p.Id;
            vuforiaTarget = "DynamicImageTarget-Patch00"; 
            Transform poi = GameObject.Find(vuforiaTarget).transform;
            Transform display = GameObject.Find("Poi_"+p.Name).transform;
            display.parent = poi;

        }
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
      
        IEnumerable<XElement> elements = from ele in result.Descendants("Pois").Elements("Poi")
                                         select ele;
        SetUpPois(elements);
       
    }
    public static void SetUpPois(IEnumerable<XElement> elements)
    {
        Pois = new List<POI>();

        //Debug.Log("inside ShowInfoByElements");
        
        foreach (var ele in elements)
        {
            //Debug.Log("a loop");
            //Debug.Log(ele.Element("Name").Value);
            POI point = new POI();
            point.Name = ele.Element("Name").Value;
            point.Name = point.Name.Replace(".","");
            Debug.Log(point.Name.ToString());
            //point.Name = mod;
            point.Id = ele.Element("Id").Value;
            point.ImageTarget = ele.Element("ImageTarget").Value;
            point.TargetHeight = ele.Element("TargetHeight").Value;
            point.TargetWidth = ele.Element("TargetWidth").Value;
            //point.userId = ele.Element("userId").Value;
            String s = ele.Element("Latitude").Value;
            decimal d = 0;
            d = decimal.Round(decimal.Parse(s), 6);//.ToString();
            point.Latitude = (double)d;
            s = ele.Element("Longitude").Value;
            d = decimal.Round(decimal.Parse(s), 6);//.ToString();
            point.Longitude = (double)d;
            //point.Latitude = Convert.ToDouble(ele.Element("Latitude").Value);
            //point.Longitude = Convert.ToDouble(ele.Element("Longitude").Value);
            point.SimilarityThreshold = Convert.ToSingle(ele.Element("SimilarityThreshold").Value);
            Pois.Add(point);
        }

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
                    tb.gameObject.name ="DynamicImageTarget-" + tb.TrackableName; ++counter;

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

                        //GameObject second = (GameObject)GameObject.Instantiate(secondObject);
                        //second.transform.parent = tb.gameObject.transform;
                        //second.transform.localPosition = new Vector3(0f, 0f, 0f);
                        //second.gameObject.SetActive(true);
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
        Text Name = board.GetChild(0).gameObject.GetComponent<Text>();
        Name.text = p.Name;// board.name;
        //Debug.Log("point" + p.Name);
        Text location = board.GetChild(1).gameObject.GetComponent<Text>();
        Text Id = board.GetChild(2).gameObject.GetComponent<Text>();
        Text UserId = board.GetChild(3).gameObject.GetComponent<Text>();
        Text TargetHeight = board.GetChild(4).gameObject.GetComponent<Text>();
        Text Latitude = board.GetChild(5).gameObject.GetComponent<Text>();
        Text Longitude = board.GetChild(6).gameObject.GetComponent<Text>();
        //find the poi with name = board.name
        //get the location and other infomation
        location.text = "locate at " + p.Latitude + " ," + p.Longitude;
        Id.text = "id : " + p.Id;
        UserId.text = "user id : " + p.userId;
        TargetHeight.text = "Target Height :" + p.TargetHeight;
        Latitude.text = "latitude : " + p.Latitude;
        Longitude.text = "longitude :" + p.Longitude;
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
            //POI p = null;
            if (area.POIs != null) {
                //int i = 0;
                foreach (POI p in area.POIs)
                {
                    if (p.Id == tb.TrackableName)
                    {
                        if (p.rendered == false) {
                            RenderText(board, p);

                            p.rendered = true;
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
            //GameObject arrowObject = (GameObject)GameObject.Instantiate(arrow);


        }


    }
    void NextScene() {
        foreach (KeyValuePair<string, int> kvp in SceneToRendered) {
            int tmp = SceneToRendered[kvp.Key];
            int numberScene = GameObject.Find("Poi_" + kvp.Key).transform.childCount;
            tmp = (tmp + 1) % numberScene;



        }
    }
    IEnumerator LoadXML()
    {

        WWW www = new WWW(SceneTools.VisAgeXml());
        yield return www;
        _result = www.text;
        readExampleXml();

    }
    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width * 0.8f, 2f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height), "Next Scene")) {
            SceneIndex += 1;
            int numberScene = 2;//GameObject.Find("Poi_" + kvp.Key).transform.childCount;
            SceneIndex = SceneIndex % numberScene;
            //Debug.Log(SceneIndex);
            //for(i = 0;i< GameObject.Find("Poi_Bentham").transform.childCount; i++)
            //{
            //    if(i!=SceneIndex)
            //    GameObject.Find("Poi_Bentham").transform.GetChild(i).gameObject.SetActive(false);
            //}

        }
    }
    //void OnGUI() {

    //    String AudioName = "a2b3b90144e44071b42c0f371beffcb8.mp3";
    //GameObject musicOb = GameObject.Find("file:" + AudioName);

    //    if (GUI.Button(new Rect(10, 10, 100, 50), "PLAY"))
    //    {
    //        Debug.Log("Playing! " + musicOb.GetComponent<AudioSource>().isPlaying);

    //        Debug.Log("Play! "+ musicOb.GetComponent<AudioSource>().name);
    //        musicOb.GetComponent<AudioSource>().Play();

    //    }
    //}

}
