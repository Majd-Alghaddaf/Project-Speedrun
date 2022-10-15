using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] Transform positionResetObject;

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            playerObject.transform.position = positionResetObject.position;
        }
    }
}
