using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PositionResetter : MonoBehaviour
{
    #region Singleton
    private static PositionResetter _instance;

    public static PositionResetter Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField] GameObject playerObject;
    [SerializeField] Transform positionResetObject;

    private GameObject[] bouncingObjects;

    private void Start()
    {
        bouncingObjects = GameObject.FindGameObjectsWithTag("BouncingObject");
    }

    void Update()
    {
        if(Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetPlayerPosition();
        }
    }

    public void ResetPlayerPosition()
    {
        playerObject.transform.position = positionResetObject.position;

        foreach(GameObject bouncingObject in bouncingObjects)
        {
            bouncingObject.SetActive(true);
        }
    }
}
