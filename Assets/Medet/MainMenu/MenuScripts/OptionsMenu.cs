using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public Toggle fullScreenTog,vSyncTog;

    public ResItem[] resolutions;
    public TMP_Text resolutionLabel;

    public AudioMixer audioMixer;
    public Slider masterSlider,musicSlider,sfxSlider;

    public AudioSource sxfLoop;


    private int selectedRes;

    // Start is called before the first frame update
    void Start()
    {


        //default ayarlar, oyun başladığında oluşan ayarlar
        fullScreenTog.isOn = Screen.fullScreen;
        if(QualitySettings.vSyncCount == 0)
        {
            vSyncTog.isOn = false;

        }
        else
        {
            vSyncTog.isOn = true;
        }

        //seaerch for res in unit
        bool foundRes = false;
        for(int i = 0; i< resolutions.Length; i++)
        {
            if(Screen.width == resolutions[i].horizontal && Screen.width == resolutions[i].vertical)
            {
                foundRes = true;

                selectedRes= i;
                updateResLabel();
            }
        } 
        if(!foundRes)
        {
            resolutionLabel.text = Screen.width.ToString() + " x " + Screen.height.ToString();
        }

        if(PlayerPrefs.HasKey("MasterVol"))
        {
            audioMixer.SetFloat("MasterVol",PlayerPrefs.GetFloat("MasterVol"));
            masterSlider.value = PlayerPrefs.GetFloat("MasterVol");
        }
        if(PlayerPrefs.HasKey("MusicVol"))
        {
            audioMixer.SetFloat("MusicVol",PlayerPrefs.GetFloat("MusicVol"));
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        }
        if(PlayerPrefs.HasKey("SFXVol"))
        {
            audioMixer.SetFloat("SFXVol",PlayerPrefs.GetFloat("SFXVol"));
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResLeft()
    {
        selectedRes--;
        if(selectedRes<0)
        {
            selectedRes = 0;
        }
        AudioManager.instance.PlaySFX(0);
        updateResLabel();

    }

    public void ResRight()
    {
        selectedRes++;
        if(selectedRes > resolutions.Length-1)
        {
            selectedRes = resolutions.Length-1;
        }
        AudioManager.instance.PlaySFX(0);
        updateResLabel();

    }

    public void updateResLabel()
    {
        resolutionLabel.text = resolutions[selectedRes].horizontal.ToString() + " x " + resolutions[selectedRes].vertical.ToString();
    }

    public void ApplyGraphics()
    {
        //fullscreen
        //Screen.fullScreen=fullScreenTog.isOn;

        //vsync
        if(vSyncTog.isOn)
        {
            QualitySettings.vSyncCount=1;
        }
        else
        {
            QualitySettings.vSyncCount=0;
        }
        AudioManager.instance.PlaySFX(0);

        //set the resoluiton
        Screen.SetResolution(resolutions[selectedRes].horizontal,resolutions[selectedRes].vertical,fullScreenTog.isOn);

    }


    public void SetMasterVolume()
    {
        audioMixer.SetFloat("MasterVol",masterSlider.value);
        PlayerPrefs.SetFloat("MasterVol",masterSlider.value);

    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("MusicVol",musicSlider.value);
        PlayerPrefs.SetFloat("MusicVol",musicSlider.value);

    }
    public void SetSFXVolume()
    {
        audioMixer.SetFloat("SFXVol",sfxSlider.value);
        PlayerPrefs.SetFloat("SFXVol",sfxSlider.value);

    }

    public void playSFXLoop()
    {
        sxfLoop.Play();

    }
    public void StopSFXLoop()
    {
        sxfLoop.Stop();

    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal,vertical;
}
