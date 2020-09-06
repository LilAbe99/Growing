using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");
    }

    void OnCollisionEnter(Collision col)
    {
        Player.transform.position = teleportTarget.transform.position;
        Destroy(this.gameObject);
    }
}
