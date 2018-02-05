using UnityEngine;
using System.Collections;

public class ActiveCamera : MonoBehaviour
{

    public GameObject camFirst;
    public GameObject camThree;
    public GameObject camSky;

    public float changeInterval = 1.0f;

    public string showStr = "";

    void Start()
    {
        FindCamera();
    }

    //找到摄像机对象  
    void FindCamera()
    {
        camFirst = GameObject.Find("Main Camera");//第一人称视角  
        camThree = GameObject.Find("Camera");//第三人称视角  
        camSky = GameObject.Find("CameraSky");//空中俯瞰视角  
        SetFalse();
        camFirst.SetActive(true);
        

    }
    void Update()
    {

    }

    void OnGUI()
    {
        ChangeAngle1();
    }
    void ChangeAngle1()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetFalse();
            camFirst.SetActive(true);
            showStr = "切换至第一人称视角";
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            SetFalse();
            camSky.SetActive(true);
            showStr = "切换至空中视角";
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            SetFalse();
            camThree.SetActive(true);
            showStr = "切换至第三人称视角";
        }
        //Debug.Log(showStr);
        GUILayout.Button(showStr);  
    }

    void SetFalse()
    {
        camFirst.SetActive(false);
        camThree.SetActive(false);
        camSky.SetActive(false);
    }



}
