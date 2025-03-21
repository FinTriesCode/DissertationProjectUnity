using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance;

    public int _wfcScenesLoaded, _wfcScenesCompleted;

    public int _recievedLoadedLevels, _recievedCompletedLevels;

    public Text _wfcScenesLoadedText, _wfcScenesCompletedText;
    public string _wfcScenesLoadedContent, _wfcScenesCompletedContent;


    private void Awake()
    {
        _instance = this; 

        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        ShowInfo();
    }

    public void ShowInfo()
    {
        _wfcScenesLoadedContent = GetLoadedLevels().ToString();
        _wfcScenesCompletedContent = GetCompletedLevels().ToString();

        _wfcScenesLoadedText.text = "Generated Scenes Loaded: " + _wfcScenesLoadedContent;
        _wfcScenesCompletedText.text = "Generated Scenes Completed: " + _wfcScenesCompletedContent;
    }

    private int GetCompletedLevels()
    {
        _recievedCompletedLevels = WfcLoadedScenesInformaiton._completedLevels;

        return _recievedCompletedLevels;
    }

    private int GetLoadedLevels()
    {
        _recievedLoadedLevels = WfcLoadedScenesInformaiton._LoadedLevels;

        return _recievedLoadedLevels;
    }
}

public class WfcLoadedScenesInformaiton : MonoBehaviour
{
    public static int _completedLevels, _LoadedLevels;

}
