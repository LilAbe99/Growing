using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShooterSystem : GameEventUserObject
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _teleportationProjectile;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    private void OnTeleportSeedCollision(Events.TeleportSeedCollisionEventArgs args)
    {
        transform.position = args.SeedPosition;
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.TeleportSeedCollisionEventArgs>(this, OnTeleportSeedCollision);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.TeleportSeedCollisionEventArgs>(this, OnTeleportSeedCollision);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Instantiate(_teleportationProjectile, _firePoint.position, _camera.transform.rotation);
        }
    }
}
