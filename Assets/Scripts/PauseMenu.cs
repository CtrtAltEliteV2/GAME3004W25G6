using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    //Properties for the pause menu, button to open the pause menu, and the pause menu itself
    //This is more like a temporary implementation, will be replaced with a more robust system later and on a different script.
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    public static bool isPaused = false;
    
    private PlayerController playerController;
    // Start is called before the first frame update
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        isPaused = false;
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found.");
        }
        // Add onClick to the resume button and main menu button, this will be changed to a proper touch button input later
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
    }
    
    #region Pause and Menu Buttons

    public void TogglePauseMenu()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            pauseMenu.SetActive(true);
            playerController.UnlockCursor();
            isPaused = true;
            Time.timeScale = 0;
        }
    }
	
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        playerController.LockCursor();
        isPaused = false;
        Time.timeScale = 1;
    }
	
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
	
    #endregion
}
