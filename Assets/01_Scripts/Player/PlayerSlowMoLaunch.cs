using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowMoLaunch : MonoBehaviour
{
    public bool canSlowMo { get; set; }
    public SlowMoObject nearbySlowMoObject;

    [Header("Time Variables")]
    [Tooltip("Determines howmuch time will be slowed down. 1 means that time will NOT be slowed.")]
    [SerializeField] [Range(0.001f, 1f)] float timeScaleValue = 0.5f;
    [SerializeField] [Range(0f, 2f)] private float horizontalMovementLockDurationAfterLaunch = 0.5f;

    [Header("Charging")]
    [Tooltip("The maximum amount of time the player can hold the charge button to launch afterwards")]
    [SerializeField] [Range(0.5f, 5f)] float maxLaunchChargeTime = 2f;
    [Tooltip("The time to be waited between two consecutive charges")]
    [SerializeField] [Range(0f, 3f)] float launchChargeCooldown = 1f;

    [Header("Slow Mo Arrow Config")]
    [SerializeField] GameObject arrowParentGameObject;
    [SerializeField] GameObject arrowChildGameObject;
    [SerializeField] Vector2 arrowInitialXPosition;
    [SerializeField] Vector2 arrowMaxXPosition;
    [SerializeField] float arrowXIncrementValue;

    [Header("Launching")]
    [Tooltip("The base force value for launching.")]
    [SerializeField] [Range(1f, 60f)] float baseLaunchForceValue = 5f;
    [Tooltip("The maximum force value for launching.")]
    [SerializeField] [Range(1f, 60f)] float maxLaunchForceValue = 30f;
    [SerializeField] [Range(0.1f, 5f)] float xLaunchForceMultiplier;
    [SerializeField] [Range(0.1f, 5f)] float yLaunchForceMultiplier;

    [Header("Audio")]
    [SerializeField] PlayerAudio _playerAudio;
    [SerializeField] private AudioClip launchAudioClip;
    [SerializeField] [Range(0f, 1f)] private float launchVolumeScale = 1f;
    [SerializeField] private AudioClip slowMoAudioClip;
    [SerializeField] [Range(0f, 1f)] private float slowMoVolumeScale = 1f;
    [SerializeField] private AudioSource slowMoAudioSource;

    private bool _hasEnteredSlowMo = false;
    private float _initialLaunchChargeTime = 0f;
    private float _currentLaunchChargeTime = 0f;

    private void Update()
    {
        if(canSlowMo)
        {
            if(Input.GetKey(KeyCode.Mouse1) && _currentLaunchChargeTime < maxLaunchChargeTime && nearbySlowMoObject.CanBeUsedToLaunch())
            {
                if (_hasEnteredSlowMo == false)
                {
                    _initialLaunchChargeTime = Time.time;
                    _playerAudio.PlayAudioClip(slowMoAudioSource, slowMoAudioClip, slowMoVolumeScale);
                    EnableSlowMoArrow();
                }
                _hasEnteredSlowMo = true;

                SlowDownTime();

                _currentLaunchChargeTime = (Time.time - _initialLaunchChargeTime) * (1 / timeScaleValue);
                UpdateArrowParentRotation();
                UpdateArrowChildPosition();
            }
            else
            {
                if (_hasEnteredSlowMo == true)
                {
                    HandleLaunch();
                }

                ResetTimeScale();
            }
        }
        else
        {
            if (_hasEnteredSlowMo == true)
            {
                HandleLaunch();
            }

            ResetTimeScale();
        }
    }

    private void EnableSlowMoArrow()
    {
        arrowParentGameObject.SetActive(true);
        arrowParentGameObject.transform.position = nearbySlowMoObject.transform.position;

        arrowChildGameObject.transform.localPosition = arrowInitialXPosition;
    }

    private void SlowDownTime()
    {
        Time.timeScale = timeScaleValue;
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
    }

    private void UpdateArrowParentRotation()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowParentGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void UpdateArrowChildPosition()
    {
        float chargePercentage = _currentLaunchChargeTime / maxLaunchChargeTime;
        chargePercentage = Mathf.Clamp(chargePercentage, 0, 1);
        
        Vector2 arrowUpdatedPosition = new Vector2(((arrowMaxXPosition.x - arrowInitialXPosition.x) * chargePercentage) + arrowInitialXPosition.x, 0f);
        arrowChildGameObject.transform.localPosition = arrowUpdatedPosition;
        //arrowChildGameObject.transform.localPosition = Vector2.Lerp((Vector2)arrowChildGameObject.transform.localPosition, arrowMaxXPosition, arrowXIncrementValue);
    }

    private void HandleLaunch()
    {
        CalculateAndApplyLaunchForce();

        StartCoroutine(nearbySlowMoObject.SetChargeCooldown(launchChargeCooldown));
        _playerAudio.PlayAudioClipOneShotFromMainSource(launchAudioClip, launchVolumeScale);
        _playerAudio.StopAudioSource(slowMoAudioSource);
        arrowParentGameObject.SetActive(false);

        _hasEnteredSlowMo = false;
        _currentLaunchChargeTime = 0f;
    }

    private void CalculateAndApplyLaunchForce()
    {
        float calculatedLaunchForceValue = baseLaunchForceValue + (((maxLaunchForceValue - baseLaunchForceValue) / maxLaunchChargeTime) * _currentLaunchChargeTime);
        Vector2 amplifiedLaunchForceVector = new Vector2(calculatedLaunchForceValue * xLaunchForceMultiplier, calculatedLaunchForceValue * yLaunchForceMultiplier);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 launchDirection = (mousePosition - (Vector2)transform.position).normalized;

        Vector2 finalLaunchForceVector = launchDirection.normalized * amplifiedLaunchForceVector;
        GetComponent<PlayerMovement>().Launch(finalLaunchForceVector, horizontalMovementLockDurationAfterLaunch);
    }

    private static void ResetTimeScale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public bool SetNearbySlowMoObject(SlowMoObject slowMoObject)
    {
        if(nearbySlowMoObject != null && canSlowMo)
        {
            Debug.LogError("Nearby Slowmo Object Already Assigned!");
            return false;
        }
        else
        {
            nearbySlowMoObject = slowMoObject;
            return true;
        }
    }
}
