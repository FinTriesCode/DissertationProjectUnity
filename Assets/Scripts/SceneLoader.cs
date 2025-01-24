using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameSceneWFC()
    {
        SceneManager.LoadScene("GameSceneWFC");
    }

    public void LoadGameSceneStatic()
    {
        SceneManager.LoadScene("GameSceneStatic");
    }

    public void LoadInstructionsScene()
    {
        SceneManager.LoadScene("InstructionsScene");
    }

    public void LoadWinScene()
    {
        SceneManager.LoadScene("WinScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

