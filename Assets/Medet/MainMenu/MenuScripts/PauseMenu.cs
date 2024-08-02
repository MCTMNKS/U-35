using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionsScreen, pauseScreen, ControlScreen;
    public GameObject inGameCanvas;

    public string mainMenuScene;

    public GameObject inventoryCanvas; // Reference to the inventory canvas
    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseunPause();
            if (isPaused == true)
            {
                inGameCanvas.SetActive(false);

            }
        }

        if (isPaused == false)
        {
            inGameCanvas.SetActive(true);
        }
    }


    public void PauseunPause()
    {
        if (!isPaused)
        {
            pauseScreen.SetActive(true);
            inventoryCanvas.SetActive(false); // Disable the inventory canvas
            isPaused = true;
            Time.timeScale = 0f; // ekranı donduruyor.
        }
        else
        {
            pauseScreen.SetActive(false);
            inventoryCanvas.SetActive(true); // Enable the inventory canvas
            isPaused = false;
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
    public void openControls()
    {
        ControlScreen.SetActive(true);

    }

    public void closeControls()
    {
        ControlScreen.SetActive(false);
    }
    public void QuittoMain()
    {
        SceneManager.LoadScene(mainMenuScene);

        Time.timeScale = 1f;

    }
}