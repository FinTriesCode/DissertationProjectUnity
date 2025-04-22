using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public GameObject _player;
    public WFC_Builder _builder;
    public SceneLoader _sceneLoader;
    private DataManager _dataManager;

    public bool _isRandomlyPositioned = false;

    private void Start()
    {
        //this.transform.position = new Vector3(_builder._width, 1, _builder._height);
        RandomSpawn(_isRandomlyPositioned);

        //_sceneLoader = FindAnyObjectByType<SceneLoader>();
        _builder = FindObjectOfType<WFC_Builder>();
        _dataManager = FindObjectOfType<DataManager>();


        if (_sceneLoader == null) _sceneLoader.AddComponent<SceneLoader>();
        else _sceneLoader.GetComponent<SceneLoader>();

        if (_dataManager == null) _dataManager.AddComponent<DataManager>();
        else _dataManager.GetComponent<SceneLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //load win scene

            try 
            {
                _dataManager._wfcScenesCompleted++;
                _sceneLoader.LoadWinScene();

                Debug.Log("try accessed");
            }
            catch
            {
                _dataManager._wfcScenesCompleted++;
                _sceneLoader.LoadWinScene();

                Debug.Log("catch accessed");
            }

            
            Debug.Log("Obj-Player collision");
        }

        if (other.CompareTag("PlayerOnStaticWorld"))
        {
            try
            {
                _dataManager._wfcScenesCompleted--;
                _sceneLoader.LoadWinScene();
            }
            catch
            {
                _sceneLoader.LoadWinScene();
            }
            
        }
    }

    public void RandomSpawn(bool _isRandomlyPositioned)
    {
        if(_isRandomlyPositioned) this.transform.position = new Vector3(Random.Range((_builder._width / 2) * 4, (_builder._width * 4) - 4), 1f, Random.Range((_builder._height / 2) * 4, (_builder._height * 4) - 4));
    }
}
