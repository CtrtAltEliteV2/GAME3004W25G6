using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Temporary script to manage the main menu buttons, will probably stay as the main menu will be simple, and
//be upgraded/changed as necessary.

//There's a current bug, when quitting to the main menu from the game, the character will be unable to move until the pause
//menu is opened and closed again. This is due to the Time.timeScale being set to 0 when the main menu is loaded. Temporarily fixed it
//by setting the Time.timeScale to 1 in the OnEnable method. Also, the day and night cycle seems to obscure the whole view of the game instead
//of just the skybox if the game is loaded first from the MainMenu Scene.
public class MainMenuButtonManager : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip buttonClickSound;

    void Start()
    {
        // Initialize AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        buttonClickSound = Resources.Load<AudioClip>("Audio/ButtonPlate Click (Minecraft Sound) - Sound Effect for editing");

        if (buttonClickSound == null)
        {
            Debug.LogError("Button click sound not found. Check the path.");
        }
    }

    public void PlayGame()
    {
        PlayButtonSound(() => SceneManager.LoadScene("IntroScene"));
    }

    public void Options()
    {
        PlayButtonSound(() => SceneManager.LoadScene("OptionsScene"));
    }

    public void QuitGame()
    {
        PlayButtonSound(Application.Quit);
    }

    public void LoadMainMenu()
    {
        PlayButtonSound(() => SceneManager.LoadScene("MainMenu"));
    }

    private void PlayButtonSound(System.Action action)
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        action.Invoke();
    }

    private void OnEnable()
    {
        // Ensure time scale is reset to 1 when the main menu is loaded
        Time.timeScale = 1f;
    }
}
