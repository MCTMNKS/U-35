using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmLoader : MonoBehaviour
{
    public AudioManager theAM;

    private void Awake()
    {
        if(AudioManager.instance == null)
        {
            Instantiate(theAM).SetupAudioManager();
        }
    }
}
