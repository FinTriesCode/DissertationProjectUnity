using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    DataManager _dataManager;

    public void Awake()
    {
        _dataManager = FindObjectOfType<DataManager>();
    }

    public void Update()
    {
        LoadMenuOnPress();
        DebugWinMenu();
    }

    public void DebugWinMenu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _dataManager._wfcScenesLoaded++;
            LoadWinScene();
        }
    }

    public void LoadMenuOnPress()
    {
        if(Input.GetKeyDown(KeyCode.M)) LoadMainMenu();
    }

    public void LoadGameSceneWFC()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _dataManager._wfcScenesLoaded++;
        SceneManager.LoadScene("GameSceneWFC");
    }

    public void LoadGameSceneStatic()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("GameSceneStatic");
    }

    public void LoadObjectiveScene()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("ObjectiveScene");
    }

    public void LoadInstructionsScene()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("InstructionsScene");
    }

    public void LoadWinScene()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _dataManager._wfcScenesCompleted++;
        SceneManager.LoadScene("WinScene");
    }

    public void LoadMainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

