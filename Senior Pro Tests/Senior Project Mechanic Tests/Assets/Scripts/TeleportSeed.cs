using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct TeleportSeedCollisionEventArgs : IGameEvent
    {
        public Collision Collision { get; private set; }
        public Vector3 SeedPosition { get; private set; }

        public TeleportSeedCollisionEventArgs(Collision collision, Vector3 seedPosition)
        {
            Collision = collision;
            SeedPosition = seedPosition;
        }
    }
}

public class TeleportSeed : MonoBehaviour
{
    [SerializeField] private float _noTargetForwardVelocity;
    [SerializeField] private float _noTargetUpwardsVelocity;

    [SerializeField] private float _maxLifetime;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Destroy(gameObject, _maxLifetime);
    }

    private void Start()
    {
        //Needs to take into account camera rotation
        _rigidbody.velocity = transform.forward * _noTargetForwardVelocity + transform.up * _noTargetUpwardsVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EventManager.Instance.TriggerEvent(new Events.TeleportSeedCollisionEventArgs(collision, transform.position));
        
        Destroy(gameObject);
    }
}
