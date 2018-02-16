//This script allows you to toggle music to play and stop.
//Assign an AudioSource to a GameObject and attach an Audio Clip in the Audio Source. Attach this script to the GameObject.

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource Sound ;
    int playing = 1;
    int pause = 2;
    int stop = 3;
    public int state = 3;
    public GameObject file;
    void Start()
    {
        GameObject prefeb_Sound = GameObject.Find("Audio Source");
        file = GameObject.Instantiate<GameObject>(prefeb_Sound, new Vector3(0, 0, 0), Quaternion.identity);
        
    }
    public void Setup(string Description,GameObject ob) {
        StartCoroutine(wwwDownload(Description));
        file.transform.parent = ob.transform;
    }
    System.Collections.IEnumerator wwwDownload(string Description) {
        WWW www = new WWW("file://" + Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "//" + Description));
        yield return www;


        AudioClip clip = www.GetAudioClip();
        //clip = www.GetAudioClip(false,false,AudioType.MPEG);
        clip = www.GetAudioClip(false, false, AudioType.MPEG);
        clip.name = Description;
       
        file.name = "url:" + Description;
        file.GetComponent<AudioSource>().clip = clip;
        Sound = file.GetComponent<AudioSource>();
        //file = ob;
    }
    public void Play()
    {
        Sound.Play();
        state = playing;
    }
    public void PauseAudio() {
        Sound.Pause();
        state = pause;
    }
    public void StopAudio()
    {
        Sound.Stop();
        state = stop;
    }
    public void RestartAudio()
    {
        Sound.Stop();
        Sound.Play();
        state = playing;
    }

}