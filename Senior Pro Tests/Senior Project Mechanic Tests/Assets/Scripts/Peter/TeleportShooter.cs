using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShooter : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {

        }
    }
}
