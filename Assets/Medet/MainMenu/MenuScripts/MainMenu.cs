using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public string firstLevel;

    public GameObject optionsScreen;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible=true;
        Cursor.lockState = CursorLockMode.None;
        //menu müziği
        AudioManager.instance.PlayMenuMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartGame()
    {
        AudioManager.instance.PlaySFX(0);
        SceneManager.LoadScene(firstLevel);

    }

    public void OpenOptions()
    {
        AudioManager.instance.PlaySFX(0);
        optionsScreen.SetActive(true);

    }

    public void ClosedOptions()
    {
        AudioManager.instance.PlaySFX(0);
        
        optionsScreen.SetActive(false);

    }
    public void QuitGame()
    {
        AudioManager.instance.PlaySFX(0);
        Application.Quit();
    }
}

