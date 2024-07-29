using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public string firstLevel;

    public string[] LevelNames;

    public GameObject optionsScreen;
    public GameObject levelScreen;
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

    public void OpenLevels()
    {
        AudioManager.instance.PlaySFX(0);
        levelScreen.SetActive(true);

    }
    public void CloseLevels()
    {
        AudioManager.instance.PlaySFX(0);
        levelScreen.SetActive(false);
    }
    public void QuitGame()
    {
        AudioManager.instance.PlaySFX(0);
        Application.Quit();
    }

    public void Level1()
    {
        AudioManager.instance.PlaySFX(0);
        SceneManager.LoadScene(LevelNames[0]);
        
    }
    public void Level2()
    {
         AudioManager.instance.PlaySFX(0);
        SceneManager.LoadScene(LevelNames[1]);

    }
    public void Level3()
    {
         AudioManager.instance.PlaySFX(0);
        SceneManager.LoadScene(LevelNames[2]);
    }
}

