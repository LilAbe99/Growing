using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemFreezer : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private float _particleSystemLength;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemLength = _particleSystem.main.duration;
    }

    private void OnEnable()
    {
        StartCoroutine(ParticleFreezer());
    }

    private IEnumerator ParticleFreezer()
    {
        float timer = Time.deltaTime;

        while(timer < _particleSystemLength)
        {
            yield return 0;

            timer += Time.deltaTime;
        }

        _particleSystem.Pause();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
