using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//an altered version of Dani's movement script
//Original Github Repo can be found here:
//https://github.com/DaniDevy/FPS_Movement_Rigidbody

public class PlayerMovement : MonoBehaviour
{
    WallRun _wallRun;

    //references
    public Transform playerCam;
    public Transform orientation;
    private Rigidbody _rb;

    //orientation
    private float _xRot;
    private float _sens = 50f;
    private float _sensMultiplier = 1f;

    public float _speed = 4500;
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

        _wallRun = GetComponent<WallRun>();
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        GetInput();
        Look();
    }

    //user input
    private void GetInput()
    {
        //get axis
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        //set additional movement binds
        _jumping = Input.GetButton("Jump");
        _crouching = Input.GetKey(KeyCode.LeftControl);

        //crouch state
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        //scale player to that of the crouch scale
        //update the player's position to fit above
        transform.localScale = _crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        //adjust rigid body so that if above a certain velocity - apply a slide force
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
        //set scale and position to player-origin
        transform.localScale = _playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        //additional gravity to enhance movement "feeling"
        _rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //find player velocity reletive to player orientation
        Vector2 _mag = FindVelRelativeToLook();
        float _xMag = _mag.x, yMag = _mag.y; //get/set magnitudes

        //add friction to slide to enhace sldie feel
        MovementFriction(x, y, _mag);

        //allows player to hold jump button for continuous jumping
        if (_canJump && _jumping) Jump();

        //Set max speed
        float maxSpeed = this._maxSpeed;

        //sliding down ramps speeds up player by adding force
        if (_crouching && _isGrounded && _canJump)
        {
            _rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //clamp player velocity
        if (x > 0 && _xMag > maxSpeed) x = 0;
        if (x < 0 && _xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float _mp = 1f, _mpV = 1f;

        //player aero-movement
        if (!_isGrounded)
        {
            _mp = 0.5f;
            _mpV = 0.5f;
        }

        //slide movement
        if (_isGrounded && _crouching) _mpV = 0f;

        //apply force to make actual movements
        _rb.AddForce(orientation.transform.forward * y * _speed * Time.deltaTime * _mp * _mpV);
        _rb.AddForce(orientation.transform.right * x * _speed * Time.deltaTime * _mp);
    }

    private void Jump()
    {
        if (_isGrounded && _canJump) //if player can jump
        {
            _canJump = false; //update state

            //apply jump by adding forces
            _rb.AddForce(Vector2.up * _jumpForce * 1.5f);
            _rb.AddForce(_normalVector * _jumpForce * 0.5f);

            //if jumping while falling, reset y velocity to make jumping responsive
            Vector3 _vel = _rb.velocity;

            if (_rb.velocity.y < 0.5f)
                _rb.velocity = new Vector3(_vel.x, 0, _vel.z);
            else if (_rb.velocity.y > 0)
                _rb.velocity = new Vector3(_vel.x, _vel.y / 2, _vel.z);

            Invoke(nameof(ResetJump), _jumpCooldown);
        }

        if(_wallRun._isWallRunning)
        {
            //normal jump
            if(_wallRun._isWallLeft && Input.GetKey(KeyCode.D) || _wallRun._isWallRight && Input.GetKey(KeyCode.A))
            {
                _rb.AddForce(Vector2.up * _jumpForce * 1.5f);
                _rb.AddForce(_normalVector * _jumpForce * 0.5f);
            }

            //side wallhop
            if (_wallRun._isWallRight || _wallRun._isWallLeft && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) _rb.AddForce(-orientation.up * _jumpForce * 1f);
            if (_wallRun._isWallRight && Input.GetKey(KeyCode.A)) _rb.AddForce(-orientation.right * _jumpForce * 3.2f);
            if (_wallRun._isWallLeft && Input.GetKey(KeyCode.D)) _rb.AddForce(orientation.right * _jumpForce * 3.2f);

            //add forward force
            _rb.AddForce(orientation.forward * _jumpForce * 1f);

            //reset vel
            _rb.velocity = Vector3.zero;

            Invoke(nameof(ResetJump), _jumpCooldown);
        }


    }

    private void ResetJump()
    {
        _canJump = true; //update state
    }

    private float _desX; //desired x

    private void Look()
    {
        //read mouse imput and apply multipliers/settings
        float mouseX = Input.GetAxis("Mouse X") * _sens * Time.fixedDeltaTime * _sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * _sens * Time.fixedDeltaTime * _sensMultiplier;

        //find current look rot
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        _desX = rot.y + mouseX;

        //apply and clamp rotation to enhance responsiveness
        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);

        //preform the rots
        playerCam.transform.localRotation = Quaternion.Euler(_xRot, _desX, _wallRun._camTilt);
        orientation.transform.localRotation = Quaternion.Euler(0, _desX, 0);

        //cam tilt
        if (Math.Abs(_wallRun._camTilt) < _wallRun._maxCamTilt && _wallRun._isWallRunning && _wallRun._isWallRight) _wallRun._camTilt += Time.deltaTime * _wallRun._maxCamTilt * 2;
        if (Math.Abs(_wallRun._camTilt) < _wallRun._maxCamTilt && _wallRun._isWallRunning && _wallRun._isWallLeft) _wallRun._camTilt -= Time.deltaTime * _wallRun._maxCamTilt * 2;

        if (_wallRun._camTilt > 0 && !_wallRun._isWallRight && !_wallRun._isWallLeft) _wallRun._camTilt -= Time.deltaTime * _wallRun._maxCamTilt * 2;
        if (_wallRun._camTilt < 0 && !_wallRun._isWallRight && !_wallRun._isWallLeft) _wallRun._camTilt += Time.deltaTime * _wallRun._maxCamTilt * 2;
    }

    private void MovementFriction(float x, float y, Vector2 mag)
    {
        if (!_isGrounded || _jumping) return;

        //Slow down sliding
        if (_crouching)
        {
            _rb.AddForce(_speed * Time.deltaTime * -_rb.velocity.normalized * _slideFriction);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > _Threshold && Math.Abs(x) < 0.05f || (mag.x < -_Threshold && x > 0) || (mag.x > _Threshold && x < 0))
        {
            _rb.AddForce(_speed * orientation.transform.right * Time.deltaTime * -mag.x * _friction);
        }
        if (Math.Abs(mag.y) > _Threshold && Math.Abs(y) < 0.05f || (mag.y < -_Threshold && y > 0) || (mag.y > _Threshold && y < 0))
        {
            _rb.AddForce(_speed * orientation.transform.forward * Time.deltaTime * -mag.y * _friction);
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
