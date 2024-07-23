using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("MasterVol"))
        {
            audioMixer.SetFloat("MasterVol",PlayerPrefs.GetFloat("MasterVol"));
            
        }
        if(PlayerPrefs.HasKey("MusicVol"))
        {
            audioMixer.SetFloat("MusicVol",PlayerPrefs.GetFloat("MusicVol"));
            
        }
        if(PlayerPrefs.HasKey("SFXVol"))
        {
            audioMixer.SetFloat("SFXVol",PlayerPrefs.GetFloat("SFXVol"));
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
