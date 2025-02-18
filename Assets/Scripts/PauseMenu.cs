using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    private AudioSource audioSource;
    private AudioClip buttonClickSound;

    public static bool isPaused = false;
    private PlayerController playerController;

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

        // Initialize AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        buttonClickSound = Resources.Load<AudioClip>("Audio/ButtonPlate Click (Minecraft Sound) - Sound Effect for editing");

        if (buttonClickSound == null)
        {
            Debug.LogError("Button click sound not found. Check the path.");
        }

        // Add onClick listeners with sound
        resumeButton.onClick.AddListener(() => PlayButtonSound(ResumeGame));
        mainMenuButton.onClick.AddListener(() => PlayButtonSound(LoadMainMenu));
    }

    private void PlayButtonSound(System.Action action)
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        action.Invoke();
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
