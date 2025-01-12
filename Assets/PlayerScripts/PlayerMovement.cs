using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //references
    public Transform playerCam;
    public Transform orientation;
    private Rigidbody _rb;

    //orientation
    private float _xRot;
    private float _sens = 50f;
    private float _sensMultiplier = 1f;

    public float _speedpeed = 4500;
    public float _maxSpeed = 20;
    public bool _isGrounded;
    public LayerMask _isGroundLayer;

    public float _friction = 0.175f;
    private float _Threshold = 0.01f;
    public float _maxSlopeAngle = 35f;

    //crouch & slide
    private Vector3 _crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 _playerScale;
    public float _slideForce = 400;
    public float _slideFriction = 0.2f;

    //Jump
    private bool _canJump = true;
    private float _jumpCooldown = 0.25f;
    public float _jumpForce = 550f;

    //Inputs and states
    float x, y;
    bool _jumping, _sprinting, _crouching;

    //sliding
    private Vector3 _normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //set scale
        _playerScale = transform.localScale;

        //lock and show cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    //user input
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        _jumping = Input.GetButton("Jump");
        _crouching = Input.GetKey(KeyCode.LeftControl);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = _crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (_rb.velocity.magnitude > 0.5f)
        {
            if (_isGrounded)
            {
                _rb.AddForce(orientation.transform.forward * _slideForce);
            }
        }
    }

    private void StopCrouch()
    {
        transform.localScale = _playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        //Extra gravity
        _rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (_canJump && _jumping) Jump();

        //Set max speed
        float maxSpeed = this._maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (_crouching && _isGrounded && _canJump)
        {
            _rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!_isGrounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (_isGrounded && _crouching) multiplierV = 0f;

        //Apply forces to move player
        _rb.AddForce(orientation.transform.forward * y * _speedpeed * Time.deltaTime * multiplier * multiplierV);
        _rb.AddForce(orientation.transform.right * x * _speedpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (_isGrounded && _canJump)
        {
            _canJump = false;

            //Add jump forces
            _rb.AddForce(Vector2.up * _jumpForce * 1.5f);
            _rb.AddForce(_normalVector * _jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = _rb.velocity;
            if (_rb.velocity.y < 0.5f)
                _rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (_rb.velocity.y > 0)
                _rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sens * Time.fixedDeltaTime * _sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * _sens * Time.fixedDeltaTime * _sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(_xRot, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!_isGrounded || _jumping) return;

        //Slow down sliding
        if (_crouching)
        {
            _rb.AddForce(_speedpeed * Time.deltaTime * -_rb.velocity.normalized * _slideFriction);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > _Threshold && Math.Abs(x) < 0.05f || (mag.x < -_Threshold && x > 0) || (mag.x > _Threshold && x < 0))
        {
            _rb.AddForce(_speedpeed * orientation.transform.right * Time.deltaTime * -mag.x * _friction);
        }
        if (Math.Abs(mag.y) > _Threshold && Math.Abs(y) < 0.05f || (mag.y < -_Threshold && y > 0) || (mag.y > _Threshold && y < 0))
        {
            _rb.AddForce(_speedpeed * orientation.transform.forward * Time.deltaTime * -mag.y * _friction);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(_rb.velocity.x, 2) + Mathf.Pow(_rb.velocity.z, 2))) > _maxSpeed)
        {
            float fallspeed = _rb.velocity.y;
            Vector3 n = _rb.velocity.normalized * _maxSpeed;
            _rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = _rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < _maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (_isGroundLayer != (_isGroundLayer | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                _isGrounded = true;
                cancellingGrounded = false;
                _normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        _isGrounded = false;
    }

}
