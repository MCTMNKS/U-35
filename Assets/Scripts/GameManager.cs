// In GameManager
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GateController FinalGateController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Remove or comment out this line
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


