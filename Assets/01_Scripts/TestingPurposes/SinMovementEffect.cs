using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMovementEffect : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    private Vector2 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    private void Update()
    {
        transform.position = _initialPosition + new Vector2(Mathf.Sin(Time.time), 0.0f) * speed;
    }
}