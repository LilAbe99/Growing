using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShoot : MonoBehaviour
{
    public GameObject teleportShot;
    public float speed = 100f;

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            GameObject instShot = Instantiate(teleportShot, transform.position, Quaternion.identity) as GameObject;
            Rigidbody instShotRB = instShot.GetComponent<Rigidbody>();
            instShotRB.AddForce(Vector3.forward * speed);
        }
    }
}
