using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    private MusicPlayer music;
    //spublic Button yourButton;
    public GameObject AudioCanvas = null;
    public GameObject buttonPrefeb;
    private GameObject newButton = null;
    private bool getName = false;
    private bool setUp = false;
    private String AudioName = "";
    int playing = 1;
    int pause = 2;
    int stop = 3;
    
    public void sendAudioName(String description) {
        AudioName = description;
        getName = true;
    }
    void Start()
    {
        if (this.transform.parent != null) { 
            Debug.Log(this.transform.parent.name);
            AudioCanvas = this.transform.parent.GetChild(this.transform.parent.childCount - 1 ).gameObject;
            newButton = Instantiate(buttonPrefeb) as GameObject;
            newButton.transform.SetParent(AudioCanvas.transform, false);
            music = (GetComponent("MusicPlayer") as MusicPlayer);//获取播放器对象
            if (getName) { 
                music.Setup(AudioName);
                setUp = true;
            }
            Button btn = newButton.GetComponent<Button>();
            //newButton.transform.position = new Vector3(0, 0, 0);
            Debug.Log(newButton.GetComponent<RectTransform>().localPosition);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            btn.onClick.AddListener(TaskOnClick);
            newButton.GetComponentInChildren<Text>().text = this.transform.parent.name + " Play >";
        }
    }
     void Update()
    {
        if (getName && !setUp)
        {
            music.Setup(AudioName);
            setUp = true; 

        }
    }
    void TaskOnClick()
    {

        // Debug.Log("AudioName  " + AudioName);
        // Debug.Log("Play"+music.Sound.clip.name);
        // Debug.Log("isPlaying" + music.Sound.isPlaying+music.Sound.isActiveAndEnabled);
        // GameObject musicOb = GameObject.Find("file:" + AudioName);
        //musicOb.GetComponent<AudioSource>().Play();
        // Debug.Log("musicOb" + musicOb.GetComponent<AudioSource>().clip.name + musicOb.GetComponent<AudioSource>().isActiveAndEnabled);
        //music.Play();
        if (music.Sound.isPlaying == false)
        {
            music.Play();
            if (music.state == playing)
                newButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = this.transform.parent.name + "- Pause";

        }

        else
        {
            music.PauseAudio();
            if (music.state == pause)
                newButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = this.transform.parent.name + "- Play";
        }
    }

    //void OnGUI()
    //{

    //    if (music.state == stop || music.state == pause)
    //    {
    //        if (GUI.Button(new Rect(10, 10, 100, 50), "PLAY"))
    //        {

    //            Debug.Log("Play!");
    //            music.Play();//调用播放器Play方法

    //        }
    //        if (music.state == pause)
    //            if (GUI.Button(new Rect(120, 10, 100, 50), "Stop"))
    //            {

    //                Debug.Log("Stop!");
    //                music.StopAudio();

    //            }

    //    }
    //    else if (music.state == playing) {
    //        if (GUI.Button(new Rect(10, 10, 100, 50), "PAUSE"))
    //        {

    //            Debug.Log("Pause!");
    //            music.PauseAudio();

    //        }
    //        if (GUI.Button(new Rect(120, 10, 100, 50), "Stop"))
    //        {

    //            Debug.Log("Stop!");
    //            music.StopAudio();

    //        }

    //    }


    //}

}
