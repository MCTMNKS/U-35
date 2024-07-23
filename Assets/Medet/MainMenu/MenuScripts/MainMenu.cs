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
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartGame()
    {
        SceneManager.LoadScene(firstLevel);

    }

    public void OpenOptions()
    {
        optionsScreen.SetActive(true);

    }

    public void ClosedOptions()
    {
        optionsScreen.SetActive(false);

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

