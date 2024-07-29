using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionsScreen,pauseScreen;

    public string mainMenuScene;


    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        
        Cursor.visible=true;
        Cursor.lockState = CursorLockMode.None;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PauseunPause();
        }
        
    }


    public void PauseunPause()
    {
        if(!isPaused)
        {
            pauseScreen.SetActive(true);
            isPaused=true;
            Time.timeScale=0f; // ekranı donduruyor.

        }
        else
        {
            pauseScreen.SetActive(false);
            isPaused=false;
            Time.timeScale = 1f; //ekranı normal hıza döndürüyor
        }



    }

    public void OpenOptions()
    {
        optionsScreen.SetActive(true);

    }
    public void closeOptions()
    {
        optionsScreen.SetActive(false);

    }
    public void QuittoMain()
    {
        SceneManager.LoadScene(mainMenuScene);

        Time.timeScale=1f;

    }
}
