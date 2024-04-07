using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void HowToPlay()
    {
        SceneManager.LoadSceneAsync("How to play");
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level 1");
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
