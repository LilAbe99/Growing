using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravitySystem : MonoBehaviour
{
    [SerializeField] private float _gravityScale = 1f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(Constants.GLOBAL_GRAVITY * _gravityScale * Vector3.up * Time.fixedDeltaTime);
    }
}
