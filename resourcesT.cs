﻿using UnityEngine;
using System.Collections;
using System.Linq;
using Vuforia;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;
using EggToolkit;
using System;

public class resourcesT : MonoBehaviour
{
    private string _result;
    //public bool once = false;
    // specify these in Unity Inspector
    public GameObject augmentationObject = null;  // you can use teapot or other object
    public string dataSetName = "";  //  Assets/StreamingAssets/QCAR/DataSetName
    public bool once = false;
    public static List<POI> Pois = null;
    // Use this for initialization
    void Start()
    {

        LoadXML("Visage");
        // Vuforia 6.2+
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
    }
    public  void readExampleXml()
    {
        XElement result = XElement.Parse(_result);//任性的地址
        //Debug.Log(result.ToString());
        IEnumerable<XElement> elements = from ele in result.Descendants("Pois").Elements("Poi")
                                         select ele;
        SetUpPois(elements);
        //createAlltext();
    }
     public static void SetUpPois(IEnumerable<XElement> elements)
    {
        Pois = new List<POI>();

        Debug.Log("inside ShowInfoByElements");
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
            point.Latitude = ele.Element("Latitude").Value;
            point.Longitude = ele.Element("Longitude").Value;
            //point.Latitude = Convert.ToDouble(ele.Element("Latitude").Value);
            //point.Longitude = Convert.ToDouble(ele.Element("Longitude").Value);
            point.SimilarityThreshold = Convert.ToSingle(ele.Element("SimilarityThreshold").Value);
            Pois.Add(point);

            //Debug.Log(ele.Element("blog").Value);
            //model.BookPrice = Convert.ToDouble(ele.Element("price").Value);

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
                    tb.gameObject.name = ++counter + ":DynamicImageTarget-" + tb.TrackableName;

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    tb.gameObject.AddComponent<TurnOffBehaviour>();

                    if (augmentationObject != null)
                    {
                        // instantiate augmentation object and parent to trackable
                        GameObject augmentation = (GameObject)GameObject.Instantiate(augmentationObject);
                        augmentation.transform.parent = tb.gameObject.transform;
                        augmentation.transform.localPosition = new Vector3(0f, 0f, 0f);
                        Quaternion target = Quaternion.Euler(90f, 0, 0);
                        augmentation.transform.localRotation = target;// Quaternion.identity;

                        augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                        augmentation.gameObject.SetActive(true);
                        augmentation.name = tb.TrackableName;
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
    void Update() {
        //if (once == false) {
        //    doOnce();
        //    once = true;
        //}
        
        IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
        foreach (TrackableBehaviour tb in tbs) {
            Transform board = tb.gameObject.transform.GetChild(0);
            Text _currencyText = board.GetChild(0).gameObject.GetComponent<Text>();
            _currencyText.text = board.name;
            POI p = null;

            foreach (POI point in Pois) {
                if (point.Name == board.name) {
                    p = point;
                }
                if (p != null)
                {
                    Text t2 = board.GetChild(1).gameObject.GetComponent<Text>();
                    Text t3 = board.GetChild(2).gameObject.GetComponent<Text>();
                    //find the poi with name = board.name
                    //get the location and other infomation
                    t2.text = "locate at " + p.Latitude + " ," + p.Longitude;
                    t3.text = "id : " + p.Id;

                }

            }



        }


    }
    private void LoadXML(string path)
    {
        _result = Resources.Load(path).ToString();
        //XmlDocument doc = new XmlDocument();
        //doc.LoadXml(_result);

        readExampleXml();
    }
    void doOnce() {
        //XMLMANGER.readExampleXml();
        //Pois =  XMLMANGER.modelList.GetRange(0, XMLMANGER.modelList.Count);
        //List<string> lstNew = lstTest.GetRange(0, 3);
        //foreach (POI point in Pois)
        //{
        //    Debug.Log(point.Name);
        //}

    }
}