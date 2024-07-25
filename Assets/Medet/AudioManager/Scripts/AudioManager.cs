using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer;

    public AudioSource menuMusic,BossMusic;
    public AudioSource[] levelTracks;


    public AudioSource[] allSFX;


   void Awake()
   {
    if (instance == null)
    {
        SetupAudioManager();
    }
    else if(instance != this)
    {
        Destroy(gameObject); //birden fazla auidomanager kurulmasın diye

    }
   }

   public void SetupAudioManager()
   {

        instance = this;
        DontDestroyOnLoad(gameObject);
   }



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




    void StopMusic()
    {
        menuMusic.Stop();
        BossMusic.Stop();
        foreach(AudioSource track in levelTracks)
        {
            track.Stop();
        }
    }

    public void PlayMenuMusic()
    {
        StopMusic();
        menuMusic.Play();
    }

    public void PlayBossMusic()
    {
        StopMusic();
        BossMusic.Play();
    }

    public void PlayLevelMusic( int tracktoPlay)
    {
        StopMusic();
        levelTracks[tracktoPlay].Play();
    }

    public void PlaySFX(int sfxToPlay)
    {
        allSFX[sfxToPlay].Stop();
        allSFX[sfxToPlay].Play();

    }

    public void PlaySFXPitched(int sfxToPlay)
    {
        allSFX[sfxToPlay].Stop();
        allSFX[sfxToPlay].pitch = Random.Range(0.75f,1.25f); //aynı sesin farklı hz sesi sıkıcı olmasın diye.
        allSFX[sfxToPlay].Play();

    }
    
}
