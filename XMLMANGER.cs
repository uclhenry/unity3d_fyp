using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggToolkit;
using System.Xml.Linq;
using System.Linq;
using System;
using UnityEngine.UI;


public class XMLMANGER {

    public static List<POI> modelList = new List<POI>();
    // Use this for initialization
    public static void readExampleXml()
    {
        XElement result = LoadXML("Assets/VisAge.xml");//任性的地址
        //Debug.Log(result.ToString());
        IEnumerable<XElement> elements = from ele in result.Descendants("Pois").Elements("Poi")
                                         select ele;
        SetUpPois(elements);
        //createAlltext();
    }
    static XElement LoadXML(string path)
    {
        XElement xml = XElement.Load(path);
        return xml;
    }
    public static void SetUpPois(IEnumerable<XElement> elements)
    {
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
            point.Latitude = Convert.ToDouble(ele.Element("Latitude").Value);
            point.Longitude = Convert.ToDouble(ele.Element("Longitude").Value);
            point.SimilarityThreshold = Convert.ToSingle(ele.Element("SimilarityThreshold").Value);
            modelList.Add(point);

            //Debug.Log(ele.Element("blog").Value);
            //model.BookPrice = Convert.ToDouble(ele.Element("price").Value);

        }

    }
}
