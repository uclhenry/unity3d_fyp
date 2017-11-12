using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggToolkit;
using System.Xml.Linq;
using System.Linq;
using System;
using UnityEngine.UI;

public class XMLtest : MonoBehaviour
{
    public Text myText;
    List<POI> modelList = new List<POI>();
    // Use this for initialization
    void Start()
    {
        XElement result = LoadXML("Assets/VisAge.xml");//任性的地址
        Debug.Log(result.ToString());
        IEnumerable<XElement> elements = from ele in result.Descendants("Pois").Elements("Poi")
                                         select ele;
        ShowInfoByElements(elements);
        createAlltext();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private XElement LoadXML(string path)
    {
        XElement xml = XElement.Load(path);
        return xml;
    }
    private void ShowInfoByElements(IEnumerable<XElement> elements)
    {
        Debug.Log("inside ShowInfoByElements");
        foreach (var ele in elements)
        {
            //Debug.Log("a loop");
            Debug.Log(ele.Element("Name").Value);
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
    public void createAlltext()
    {
        float n = 5F;
        float t = 0F;
        foreach (var point in modelList) {
            Text temp = createMytext(new Vector3(t*5, t, 0F));
            t = t + n;
            temp.text = point.Name;
        }
    }
    public Text createMytext(Vector3 position)
    {
        Text atext = Instantiate(myText);


        GameObject canvas = new GameObject("_Canvas", typeof(Canvas));
        canvas.transform.position = position;
        //canvas.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
        //tempT.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>(),false);
        atext.GetComponent<Transform>().SetParent(canvas.GetComponent<Transform>(), true);
        
        atext.GetComponent<RectTransform>().localPosition = position;
        atext.GetComponent<RectTransform>().localScale = new Vector3(0.1F, 0.1F, 0.1F);
        return atext;
    }
}
public class POI
{
    public string Id;
    public string Name;
    public string ImageTarget;
    public double Latitude;
    public double Longitude;
    public string TargetHeight;
    public string TargetWidth;
    public float SimilarityThreshold;
    public string userId;

}