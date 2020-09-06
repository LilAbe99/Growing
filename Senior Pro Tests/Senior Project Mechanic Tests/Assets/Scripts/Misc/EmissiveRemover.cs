using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissiveRemover : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * 0);
    }
}
