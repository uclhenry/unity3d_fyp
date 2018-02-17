using UnityEngine;
using System.Collections;

public class ActiveCamera : MonoBehaviour
{

    public GameObject ARCamera;
    public GameObject MapCamera;
    

    public float changeInterval = 1.0f;

    public string showStr = "";
    public float guiXScale;
    public float guiYScale;
    public Rect guiRect;

    void Start()
    {
        FindCamera();
        guiXScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.width : Screen.height) / 480.0f;
        guiYScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.height : Screen.width) / 640.0f;
        // setup the gui area
        guiRect = new Rect(4.0f * guiXScale, 64.0f * guiXScale, Screen.width / guiXScale - 32.0f * guiXScale, 32.0f * guiYScale);

    }

    //找到摄像机对象  
    void FindCamera()
    {
        ARCamera = GameObject.Find("ARCamera");//第一人称视角  
        MapCamera = GameObject.Find("Main Camera");//第三人称视角  
        MapCamera.GetComponent<Camera>().enabled = false;
        //MapCamera.SetActive(true);
        

    }
    void Update()
    {

    }

    void OnGUI()
    {
        ChangeCamera();
        ClickButtonToChangeView();
    }
    void ChangeCamera()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetFalse();
            ARCamera.GetComponent<Camera>().enabled = true;
            showStr = "切换至第一人称视角";
        }

        else if (Input.GetKey(KeyCode.Alpha3))
        {
            SetFalse();
            MapCamera.GetComponent<Camera>().enabled = true;
            showStr = "切换至第三人称视角";
        }
        //Debug.Log(showStr);
        GUILayout.Button(showStr);  
    }
    void ClickButtonToChangeView() {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        if (GUI.Button(new Rect(0,2f/8* Screen.height,0.2f*Screen.width,0.1f*Screen.height), "toggle View")) {
            bool previousState = ARCamera.GetComponent<Camera>().enabled;
            if (previousState == false)
            {
                ARCamera.GetComponent<Camera>().enabled = true;
                MapCamera.GetComponent<Camera>().enabled = false;
 
                GameObject.Find("[Map]").transform.parent = hiddenScenes;
            }
            else if (previousState == true) {
                ARCamera.GetComponent<Camera>().enabled = false;
                MapCamera.GetComponent<Camera>().enabled = true;

                GameObject.Find("[Map]").transform.parent = null;
            }
        }
    }

    void SetFalse()
    {
        ARCamera.GetComponent<Camera>().enabled = false;
      
        MapCamera.GetComponent<Camera>().enabled = false;
        
    }



}
