using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuration")]
    [Header("References")]
    [SerializeField] private GameObject playerModelGameObject;
    [SerializeField] private GameObject frontGameObject;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [Header("Movement")]
    [SerializeField] private string runningAnimationBoolName = "isRunning";
    [SerializeField] [Range(0f, 25f)] private float moveSpeed;
    [SerializeField] private ParticleSystem runningParticleSystem;
    [Header("Jumping")]
    [SerializeField] private string jumpingAnimationBoolName = "isJumping";
    [Tooltip("When a player is no longer grounded, there will be a time window where they can still jump. This variable represents the duration of that time window.")]
    [SerializeField] [Range(0f, 0.5f)] private float timeBeforeDisablingJump;
    [SerializeField] [Range(0f, 2f)] private float recentlyGroundedMaxTime;
    [SerializeField] [Range(0f, 50f)] private float jumpForce;
    [SerializeField] [Range(0f, 50f)] private float longJumpForce;
    [SerializeField] [Range(0f, 2f)] private float longJumpMaxDuration;
    [SerializeField] [Range(0f, 3f)] private float doubleJumpMultiplier;
    [Header("Wall Sliding & Jumping")]
    [SerializeField] [Range(0f, 25f)] private float wallSlidingSpeed;
    [SerializeField] [Range(0f, 2f)] private float wallStickDuration;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] [Range(0f, 2f)] private float timeBetweenWallJumps = 0.5f;
    [SerializeField] [Range(0f, 2f)] private float wallJumpHorizontalLockDuration = 0.1f;
    [SerializeField] [Range(0.1f, 5f)] private float longJumpForceDividerWhenSliding = 2f;
    [Header("Dashing")]
    [SerializeField] [Range(0f, 50f)] private float dashForceValue = 0f;
    [SerializeField] [Range(0f, 5f)] private float dashDuration = 1f;
    [SerializeField] [Range(0f, 3f)] private float timeBetweenDashes = 0.5f;
    [Header("Audio")]
    [SerializeField] PlayerAudio _playerAudio;
    [SerializeField] private AudioClip[] dashAudioClips;
    [SerializeField] [Range(0f, 1f)] private float dashVolumeScale = 1f;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] [Range(0f, 1f)] private float jumpvolumeScale = 1f;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] [Range(0f, 1f)] private float deathVolumeScale = 1f;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private PlayerInput _playerInput;
    private TrailRenderer _trailRenderer;
    private Coroutine _lockLongJumpCoroutine;
    private Coroutine _lockHorizontalMovementCoroutine;

    private bool _actionsLocked = false;
    private float _horizontalInput = 0f;
    private bool _lockHorizontalMovement = false;
    private bool _isGrounded = true;
    private bool _canJump = true;
    private bool _isWallSliding = false;
    private bool _isStickingToWall = false;
    private float _wallStickTimer = 0f;
    private bool _canWallJump = true;
    private bool _canDoubleJump = true;
    private bool _canDash = true;
    private bool _canLongJump = true;
    private bool _lockLongJumpCoroutineStarted = false;
    private bool _wasRecentlyGrounded = true;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private void Update()
    {
        if(_actionsLocked) { return; }

        _horizontalInput = _playerInput.GetMovementInput();

        HandleRun();
        HandleJump();
        HandleWallSlide();
        HandleDash();
    }

    private void FixedUpdate()
    { 
        if(_actionsLocked) { return; }
        
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

        bool isRunning = Mathf.Abs(_rigidbody.velocity.x) > Mathf.Epsilon;
        UpdateRunningAnimation(isRunning);
        UpdateRunningParticleSystem(isRunning);
    }

    private void UpdateRunningAnimation(bool isRunning)
    {
        _animator.SetBool(runningAnimationBoolName, isRunning);

        if(isRunning)
        {
            UpdateSpriteFlipRotation();
        }
    }

    private void UpdateRunningParticleSystem(bool isRunning)
    {
        if (runningParticleSystem == null) return;

        if(_isGrounded && isRunning)
        {
            if(!runningParticleSystem.isPlaying)
                runningParticleSystem.Play();
            return;
        }

        runningParticleSystem.Stop();
    }


    private void UpdateSpriteFlipRotation()
    {
        playerModelGameObject.transform.localScale = new Vector2(Mathf.Sign(_rigidbody.velocity.x), 1f);
    }

    private void HandleJump()
    {
        if (_canJump && _playerInput.GetJumpPressedInput())
        {
            Jump();
        }
        else if(_isWallSliding && _canWallJump && _playerInput.GetJumpPressedInput())
        {
            WallJump();
        }
        else if(!_canJump && !_isWallSliding && _canDoubleJump && _playerInput.GetJumpPressedInput())
        {
            DoubleJump();
        }
    }

    private void Jump(bool isDoubleJump = false)
    {
        _canJump = false;

        float verticalJumpForceValue;
        if(_rigidbody.velocity.y < 0) // if player is falling, need to compensate by adding the equivalent of the gravity's force to the jump force
            verticalJumpForceValue = (_rigidbody.velocity.y * -1) + jumpForce; 
        else
            verticalJumpForceValue = jumpForce;
        if (isDoubleJump)
            verticalJumpForceValue *= doubleJumpMultiplier;

        _rigidbody.AddForce(new Vector2(0f, verticalJumpForceValue), ForceMode2D.Impulse);
        _playerAudio.PlayAudioClipOneShotFromMainSource(jumpAudioClip,jumpvolumeScale);
    }

    private void WallJump()
    {
        float wallJumpDirection = GetFacingDirection();
        if (_horizontalInput == 0)
            wallJumpDirection *= -1;

        StartCoroutine(LockHorizontalMovement(wallJumpHorizontalLockDuration));
        _rigidbody.velocity = new Vector2(wallJumpForce.x * wallJumpDirection, wallJumpForce.y);

        _playerAudio.PlayAudioClipOneShotFromMainSource(jumpAudioClip, jumpvolumeScale);

        SetCanDoubleJump(true);

        InterruptLongJumpLock();
        _canLongJump = true;

        UpdateSpriteFlipRotation();
        StartCoroutine(LockWallJumping());
    }

    private float GetFacingDirection()
    {
        if (_horizontalInput == 0)
        {
            Vector2 facingDirection = frontGameObject.transform.position - transform.position;
            facingDirection = facingDirection.normalized;
            return Mathf.Sign(facingDirection.x);
        }
        else
        {
            return Mathf.Sign(_horizontalInput);
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
        Jump(true);

        SetCanDoubleJump(false);

        InterruptLongJumpLock();
        _canLongJump = true;
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

        return !(Mathf.Sign(facingDirection.x) == Mathf.Sign(_horizontalInput));
    }

    private void IncrementWallStickTimer()
    {
        _wallStickTimer += Time.deltaTime;
    }

    private void HandleLongJump()
    {
        if(_isGrounded == true) { return; }

        if(_canLongJump == false) { return; }

        if (_playerInput.GetJumpHoldInput())
        {
            LongJump();
            if(_lockLongJumpCoroutineStarted == false) // so that we don't have multiple instances of the same coroutine running at the same time
            {
                _lockLongJumpCoroutine = StartCoroutine(LockLongJumpAfterDuration());
                _lockLongJumpCoroutineStarted = true;
            }
        }
        else
        {
            _canLongJump = false;
        }
    }

    private void LongJump()
    {
        float force = _isWallSliding ? longJumpForce / longJumpForceDividerWhenSliding : longJumpForce; //to prevent player from spamming long jump on wall to get up
        _rigidbody.AddForce(new Vector2(0f, force), ForceMode2D.Force);
    }

    private IEnumerator LockLongJumpAfterDuration()
    {
        yield return new WaitForSeconds(longJumpMaxDuration);
        _canLongJump = false;
        _lockLongJumpCoroutineStarted = false;
    }

    private void HandleDash()
    {
        if (_playerInput.GetDashInput() && _canDash)
        {
            Dash();
            _playerAudio.PlayOneShotRandomAudioClipFromArrayFromMainSource(dashAudioClips, dashVolumeScale);
            StartCoroutine(LockDashing());
        }
    }

    private void Dash()
    {
        float dashDirection = GetFacingDirection();

        float horizontalDashForce = dashDirection * dashForceValue;

        if (Mathf.Sign(dashDirection) != Mathf.Sign(_rigidbody.velocity.x)) //compensate force value if going in opposite direction of dash
        {
            horizontalDashForce += _rigidbody.velocity.x * -1;
        }

        StartCoroutine(ScreenShake());
        StartCoroutine(DisplayTrailRenderer());
        StartCoroutine(LockHorizontalMovement(dashDuration));
        _rigidbody.AddForce(new Vector2(horizontalDashForce, 0f), ForceMode2D.Impulse);
    }

    private IEnumerator LockDashing()
    {
        _canDash = false;
        yield return new WaitForSeconds(timeBetweenDashes);

        if(_isGrounded || _wasRecentlyGrounded)
            _canDash = true;
        else
        {
            yield return new WaitUntil(() => _isGrounded == true);
            _canDash = true;
        }
    }

    private IEnumerator LockHorizontalMovement(float duration)
    {
        _lockHorizontalMovement = true;
        yield return new WaitForSeconds(duration);
        _lockHorizontalMovement = false;
    }

    private IEnumerator ScreenShake()
    {
        Cinemachine.CinemachineBasicMultiChannelPerlin virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        if (virtualCameraNoise != null)
        {
            virtualCameraNoise.m_AmplitudeGain = 5;

            yield return new WaitForSeconds(dashDuration);

            virtualCameraNoise.m_AmplitudeGain = 0;
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator DisplayTrailRenderer()
    {
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = true;
            yield return new WaitForSeconds(dashDuration);
            _trailRenderer.enabled = false;
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator SetCanDoubleJumpAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        _canDoubleJump = true;
    }

    public void SetActionsLocked(bool value)

    {
        _actionsLocked = value;

        if(_actionsLocked)
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void SetIsGrounded(bool value)
    {
        _isGrounded = value;
        _animator.SetBool(jumpingAnimationBoolName, !_isGrounded);
    }

    public void SetCanJump(bool value)
    {
        _canJump = value;
    }

    public IEnumerator DisableFirstJumpAfterSeconds()
    {
        yield return new WaitForSeconds(timeBeforeDisablingJump);
        _canJump = false;
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

    public void SetCanDoubleJump(bool value) => _canDoubleJump = value;

    public void SetCanLongJump(bool value) => _canLongJump = value;

    public void InterruptLongJumpLock()
    {
        if(_lockLongJumpCoroutine == null) { return; }

        StopCoroutine(_lockLongJumpCoroutine);
        _lockLongJumpCoroutineStarted = false;
    }

    public void Launch(Vector2 launchForceVector, float horizontalMovementLockDurationAfterLaunch)
    {
        _rigidbody.velocity = Vector2.zero; // resets velocity right before launching for no interference

        _lockHorizontalMovementCoroutine = StartCoroutine(LockHorizontalMovement(horizontalMovementLockDurationAfterLaunch));
        _rigidbody.AddForce(launchForceVector, ForceMode2D.Impulse);

        SetCanDoubleJump(true);
    }

    public void Bounce(Vector2 bounceForceVector, float timeBeforeAllowingDoubleJump)
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(bounceForceVector, ForceMode2D.Impulse);
        StartCoroutine(SetCanDoubleJumpAfterSeconds(timeBeforeAllowingDoubleJump));
    }

    public void InterruptHorizontalMovementLock()
    {
        if(_lockHorizontalMovementCoroutine == null) { return; }

        StopCoroutine(_lockHorizontalMovementCoroutine);
    }

    public IEnumerator EnableWasRecentlyGroundedForDuration()
    {
        recentlyGroundedMaxTime = timeBetweenDashes; // only reason it's needed for now, might change later

        _wasRecentlyGrounded = true;
        yield return new WaitForSeconds(recentlyGroundedMaxTime);
        _wasRecentlyGrounded = false;
    }

    public void PlayDeathAnimation()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetTrigger("onDeath");
        _animator.SetBool(runningAnimationBoolName, false);
        _animator.SetBool(jumpingAnimationBoolName, false);
    }

    public void PlayDeathSFX()
    {
        _playerAudio.PlayAudioClipOneShotFromMainSource(deathAudioClip, deathVolumeScale);
    }
}
