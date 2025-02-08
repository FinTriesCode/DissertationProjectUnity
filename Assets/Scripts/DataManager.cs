using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance;

    public int _wfcScenesLoaded, _wfcScenesCompleted;

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
        _wfcScenesLoadedContent = _wfcScenesLoaded.ToString();
        _wfcScenesCompletedContent = _wfcScenesCompleted.ToString();

        _wfcScenesLoadedText.text = "Generated Scenes Loaded: " + _wfcScenesLoadedContent;
        _wfcScenesCompletedText.text = "Generated Scenes Completed: " + _wfcScenesCompletedContent;


        if (Input.GetKeyDown(KeyCode.O))
        {
            Console.WriteLine("---------------------");
            Console.WriteLine(_wfcScenesLoaded);
            Console.WriteLine(_wfcScenesCompleted);
            Console.WriteLine("---------------------");
        }
    }
}
