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
    public void PlayGame()
    {
        SceneManager.LoadScene("IntroScene");
    }
    
    public void Options()
    {
        SceneManager.LoadScene("OptionsScene");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    private void OnEnable()
    {
        // Ensure time scale is reset to 1 when the main menu is loaded
        Time.timeScale = 1f;
    }
}
