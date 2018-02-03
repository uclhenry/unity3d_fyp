//This script allows you to toggle music to play and stop.
//Assign an AudioSource to a GameObject and attach an Audio Clip in the Audio Source. Attach this script to the GameObject.

using System.IO;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource Sound ;
    int playing = 1;
    int pause = 2;
    int stop = 3;
    public int state = 3;
    
    public void Setup(string Description) {
        //clone prefeb
        WWW www = new WWW("file://" + Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "//" + Description));
        AudioClip clip = www.GetAudioClip();
        //clip = www.GetAudioClip(false,false,AudioType.MPEG);
        clip = www.GetAudioClip(false, false, AudioType.MPEG);
        clip.name = Description;
        GameObject prefeb_Sound = GameObject.Find("Audio Source");
        //GameObject prefeb_Sound = (GameObject)Resources.Load("Audio Source", typeof(GameObject));
       GameObject ob = GameObject.Instantiate<GameObject>(prefeb_Sound, new Vector3(0, 0, 0), Quaternion.identity);
        ob.name = "file:" + Description;
        //Sound = Instantiate(Sound);
        ob.GetComponent<AudioSource>().clip = clip;
        Sound = ob.GetComponent<AudioSource>();
       // Sound.clip = clip;// (AudioClip)Resources.Load(str, typeof(AudioClip));//调用Resources方法加载AudioClip资源
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