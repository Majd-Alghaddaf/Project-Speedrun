using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuration")]
    [Header("References")]
    [SerializeField] private GameObject playerModelGameObject;
    [SerializeField] private GameObject frontGameObject;
    [SerializeField] private AudioSource movementAudioSource;
    [Header("Movement")]
    [SerializeField] private string horizontalAxisName = "Horizontal";
    [SerializeField] private string runningAnimationBoolName = "isRunning";
    [SerializeField] [Range(0f, 25f)] private float moveSpeed;
    [Header("Jumping")]
    [SerializeField] private string jumpButtonName = "Jump";
    [SerializeField] private string jumpingAnimationBoolName = "isJumping";
    [SerializeField] [Range(0f,50f)] private float jumpForce;
    [SerializeField] [Range(0f, 50f)] private float longJumpForce;
    [Header("Wall Sliding & Jumping")]
    [SerializeField] [Range(0f, 25f)] private float wallSlidingSpeed;
    [SerializeField] [Range(0f, 2f)] private float wallStickDuration;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] [Range(0f, 2f)] private float timeBetweenWallJumps = 0.5f;
    [SerializeField] [Range(0f, 2f)] private float wallJumpHorizontalLockDuration = 0.1f;
    [SerializeField] [Range(0.1f, 5f)] private float longJumpForceDividerWhenSliding = 2f;
    [Header("Dashing")]
    [SerializeField] private string dashButtonName = "Fire1";
    [SerializeField] [Range(0f, 50f)] private float dashForceValue = 0f;
    [SerializeField] [Range(0f, 5f)] private float dashDuration = 1f;
    [SerializeField] [Range(0f, 3f)] private float timeBetweenDashes = 0.5f;
    [Header("Audio")]
    [SerializeField] PlayerAudio _playerAudio;
    [SerializeField] private AudioClip[] dashAudioClips;
    [SerializeField] [Range(0f, 1f)] private float dashVolumeScale = 1f;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] [Range(0f, 1f)] private float jumpvolumeScale = 1f;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private float _horizontalInput = 0f;
    private bool _lockHorizontalMovement = false;
    private bool _isGrounded = true;
    private bool _isWallSliding = false;
    private bool _isStickingToWall = false;
    private float _wallStickTimer = 0f;
    private bool _canWallJump = true;
    private bool _canDoubleJump = true;
    private bool _canDash = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw(horizontalAxisName);

        HandleRun();
        HandleJump();
        HandleWallSlide();
        HandleDash();
    }

    private void FixedUpdate()
    { 
        HandleLongJump();
    }

    private void HandleRun()
    {
        if (_isStickingToWall == true && _isGrounded == false) return; // allows player to stick to wall - needs to jump or hold left/right to unstick

        if(_lockHorizontalMovement == false)
        {
            if (_isGrounded == false && _horizontalInput == 0) return; // prevents player from auto-stopping mid-air when being launched

            _rigidbody.velocity = new Vector2(_horizontalInput * moveSpeed, _rigidbody.velocity.y);
        }

        SetRunningAnimation();
    }

    private void SetRunningAnimation()
    {
        bool isRunning = Mathf.Abs(_rigidbody.velocity.x) > Mathf.Epsilon;
        _animator.SetBool(runningAnimationBoolName, isRunning);

        if(isRunning)
        {
            SetSpriteFlip();
        }
    }
    
    private void SetSpriteFlip()
    {
        playerModelGameObject.transform.localScale = new Vector2(Mathf.Sign(_rigidbody.velocity.x), 1f);
    }

    private void HandleJump()
    {
        if (_isGrounded && Input.GetButtonDown(jumpButtonName))
        {
            Jump();
        }
        else if(_isWallSliding && _canWallJump && Input.GetButtonDown(jumpButtonName))
        {
            WallJump();
        }
        else if(!_isGrounded && !_isWallSliding && _canDoubleJump && Input.GetButtonDown(jumpButtonName))
        {
            DoubleJump();
        }
    }

    private void Jump()
    {
        _rigidbody.AddForce(new Vector2(0f, (_rigidbody.velocity.y * -1) + jumpForce), ForceMode2D.Impulse);
        _playerAudio.PlayAudioClipOneShotFromMainSource(jumpAudioClip,jumpvolumeScale);
    }

    private void WallJump()
    {
        float wallJumpDirection = CalculateWallJumpDirection();

        StartCoroutine(LockHorizontalMovement(wallJumpHorizontalLockDuration));
        _rigidbody.velocity = new Vector2(wallJumpForce.x * wallJumpDirection, wallJumpForce.y);

        _playerAudio.PlayAudioClipOneShotFromMainSource(jumpAudioClip, jumpvolumeScale);
        SetCanDoubleJump(true);
        SetSpriteFlip();
        StartCoroutine(LockWallJumping());
    }

    private float CalculateWallJumpDirection()
    {
        if (_horizontalInput == 0)
        {
            Vector2 facingDirection = frontGameObject.transform.position - transform.position;
            facingDirection = facingDirection.normalized;
            return Mathf.Sign(facingDirection.x) * -1;
        }
        else
        {
            return _horizontalInput;
        }
    }

    private IEnumerator LockWallJumping()
    {
        _canWallJump = false;
        yield return new WaitForSeconds(timeBetweenWallJumps);
        _canWallJump = true;
    }

    private void DoubleJump()
    {
        Jump();
        SetCanDoubleJump(false);
    }

    private void HandleWallSlide()
    {
        if(_isWallSliding)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y, -wallSlidingSpeed, float.MaxValue));

            if (HoldingOppositeDirectionButton())
            {
                IncrementWallStickTimer();
                if (_wallStickTimer >= wallStickDuration)
                {
                    _isStickingToWall = false;
                }
            }
            else
            {
                _wallStickTimer = 0f;
            }
        }
    }

    private bool HoldingOppositeDirectionButton()
    {
        if(_horizontalInput == 0)
        {
            return false;
        }

        Vector2 facingDirection = frontGameObject.transform.position - transform.position;
        facingDirection = facingDirection.normalized;

        return !(Mathf.Sign(facingDirection.x) == _horizontalInput);
    }

    private void IncrementWallStickTimer()
    {
        _wallStickTimer += Time.deltaTime;
    }

    private void HandleLongJump()
    {
        if (!_isGrounded && Input.GetButton(jumpButtonName))
        {
            float force = _isWallSliding ? longJumpForce / longJumpForceDividerWhenSliding : longJumpForce; //to prevent player from spamming long jump on wall to get up
            _rigidbody.AddForce(new Vector2(0f, force), ForceMode2D.Force);
        }
    }

    private void HandleDash()
    {
        if (Input.GetButtonDown(dashButtonName) && _horizontalInput != 0 && _canDash)
        {
            Dash();
            _playerAudio.PlayOneShotRandomAudioClipFromArrayFromMainSource(dashAudioClips, dashVolumeScale);
            StartCoroutine(LockDashing());
        }
    }

    private void Dash()
    {
        float horizontalDashForce = _horizontalInput * dashForceValue;

        if (Mathf.Sign(_horizontalInput) != Mathf.Sign(_rigidbody.velocity.x)) //compensate force value if going in opposite direction of dash
        {
            horizontalDashForce += _rigidbody.velocity.x * -1;
        }

        StartCoroutine(LockHorizontalMovement(dashDuration));
        _rigidbody.AddForce(new Vector2(horizontalDashForce, 0f), ForceMode2D.Impulse);
    }

    private IEnumerator LockDashing()
    {
        _canDash = false;
        yield return new WaitForSeconds(timeBetweenDashes);
        _canDash = true;
    }

    private IEnumerator LockHorizontalMovement(float duration)
    {
        _lockHorizontalMovement = true;
        yield return new WaitForSeconds(duration);
        _lockHorizontalMovement = false;
    }

    public void SetIsGrounded(bool value)
    {
        _isGrounded = value;
        _animator.SetBool(jumpingAnimationBoolName, !_isGrounded);
    }

    public void OnWallSlidingEnter()
    {
        _isWallSliding = true;
        _isStickingToWall = true;
        _wallStickTimer = 0f;
    }

    public void OnWallSlidingExit()
    {
        _isWallSliding = false;
        _isStickingToWall = false;
        SetCanDoubleJump(true);
    }

    public void SetCanDoubleJump(bool value)
    {
        _canDoubleJump = value;
    }

    public void Launch(Vector2 launchForceVector, float horizontalMovementLockDurationAfterLaunch)
    {
        _rigidbody.velocity = Vector2.zero; // resets velocity right before launching for no interference

        StartCoroutine(LockHorizontalMovement(horizontalMovementLockDurationAfterLaunch));
        _rigidbody.AddForce(launchForceVector, ForceMode2D.Impulse);

        SetCanDoubleJump(true);
    }
}
