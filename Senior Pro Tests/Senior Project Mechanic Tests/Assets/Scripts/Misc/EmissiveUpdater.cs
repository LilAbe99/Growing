using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissiveUpdater : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private float _scale = 1f;

    private Material _material;

    private Color _startingColor;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        _startingColor = _light.color;

        StartCoroutine(UpdateEmissives());
    }

    private IEnumerator UpdateEmissives()
    {
        while(true)
        {
            if(_light.isActiveAndEnabled) _material.SetColor("_EmissionColor", _startingColor * _light.intensity * _scale);
            else _material.SetColor("_EmissionColor", _startingColor * 0);

            yield return null;
        }
    }
}
