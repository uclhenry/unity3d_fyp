using UnityEngine;
using System.Collections;

using System.Xml.Linq;
using System.Xml;
using System.IO;

public class SAtest : MonoBehaviour
{
    private string _result;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadXML());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 如前文所述，streamingAssets只能使用www来读取，
    /// 如果不是使用www来读取的同学，就不要问为啥读不到streamingAssets下的内容了。
    /// 这里还可以使用persistenDataPath来保存从streamingassets那里读到内容。
    /// </summary>
    IEnumerator LoadXML()
    {
        string sPath = "";
        if (Application.platform == RuntimePlatform.WindowsEditor) sPath = "file://" + Application.streamingAssetsPath + "/t.xml";
        else if (Application.platform == RuntimePlatform.Android) {
            sPath = Application.streamingAssetsPath + "/t.xml";
        }
            //sPath = "t";
            WWW www = new WWW(sPath);
        yield return www;
        _result = www.text;
    }

    void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 20;
        titleStyle.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);
        GUI.Label(new Rect(400, 10, 500, 200), _result, titleStyle);
    }

}