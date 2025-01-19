using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public GameObject _player;
    public Vector3 _playerSpawn;

    private void Start()
    {
        _playerSpawn = new Vector3(0, 2, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) ResetSpawn();
    }

    public void ResetSpawn()
    {
        _player.transform.position = _playerSpawn;
    }
}
