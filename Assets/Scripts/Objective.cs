using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public GameObject _player;
    public WFC_Builder _builder;
    private SceneLoader _sceneLoader;
    public DataManager _dataManager;

    public bool _isRandomlyPositioned = false;

    private void Start()
    {
        //this.transform.position = new Vector3(_builder._width, 1, _builder._height);
        RandomSpawn(_isRandomlyPositioned);

        _sceneLoader = FindAnyObjectByType<SceneLoader>();
        _builder = FindObjectOfType<WFC_Builder>();
        _dataManager = FindObjectOfType<DataManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //load win scene
            _dataManager._wfcScenesCompleted++;
            _sceneLoader.LoadWinScene();

            Debug.Log("Obj-Player collision");
        }
    }

    public void RandomSpawn(bool _pIsRandomlyPositioned)
    {
        if(_pIsRandomlyPositioned) this.transform.position = new Vector3(Random.Range((_builder._width / 2) * 4, (_builder._width * 4) - 4), 1, Random.Range((_builder._height / 2) * 4, (_builder._height * 4) - 4));
    }
}
