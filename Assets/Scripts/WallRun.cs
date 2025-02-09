using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    PlayerMovement _movement;
    Rigidbody _rb;

    public LayerMask _wallMask;

    public float _wallRunForce, _maxWallRunSpeed;
    public float _maxCamTilt, _camTilt;

    public bool _isWallRight, _isWallLeft, _isWallRunning;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        WallCheck();
        WallRunInput();
    }

    private void WallCheck()
    {
        _isWallRight = Physics.Raycast(transform.position, _movement.orientation.right, 1f, _wallMask);
        _isWallLeft = Physics.Raycast(transform.position, -_movement.orientation.right, 1f, _wallMask);

        if (!_isWallLeft && !_isWallRight) StopWallRun();
    }

    private void WallRunInput()
    {
        if (Input.GetKey(KeyCode.D) && _isWallRight) StartWallRun();
        if (Input.GetKey(KeyCode.A) && _isWallLeft) StartWallRun();
    }

    private void StartWallRun()
    {
        _rb.useGravity = false;
        _isWallRunning = true;

        if(_rb.velocity.magnitude <= _maxWallRunSpeed)
        {
            _rb.AddForce(_movement.orientation.forward * _wallRunForce * Time.deltaTime);

            //stick to wall
            if (_isWallRight) _rb.AddForce(_movement.orientation.right * _wallRunForce / 5 * Time.deltaTime);
            else _rb.AddForce(-_movement.orientation.right * _wallRunForce / 5 * Time.deltaTime);
        }
    }

    private void StopWallRun()
    {
        _rb.useGravity = true;
        _isWallRunning = false;
    }
}
