using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AN : MonoBehaviour {
    public float testangle = 0;
    public float factor = 0.1f;
    // Use this for initialization
    void Start()
    {
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Mathf.Abs(Input.compass.magneticHeading - testangle)>1)
        {
            testangle = testangle * (1f - factor) + Input.compass.magneticHeading * factor;
        }
        
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, Input.compass.magneticHeading, 0), Time.deltaTime * 2);
        transform.rotation = Quaternion.Euler(0,0 , testangle);//testangle
        transform.parent.GetChild(2).gameObject.GetComponent<Text>().text = "" + ((int)testangle);
        //testangle.ToString();
    }
    void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 40;
        titleStyle.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);
        GUI.Label(new Rect(400, 10, 500, 200), Input.compass.magneticHeading.ToString()+"   "+testangle.ToString(), titleStyle);
        //GUI.TextArea(new Rect(100, 0, 100, 100), Input.compass.magneticHeading.ToString());
    }
}
