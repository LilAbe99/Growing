using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GrapplingHook : MonoBehaviour
{
    public Camera fpsCam;
    public Transform cam;
    private RaycastHit hit;
    private Rigidbody rb;
    public bool GrappleAttached = false;
    public bool GrabAttached = false;
    public float momentum;
    public float speed;
    private float step;
    public RigidbodyFirstPersonController cc;
    private LineRenderer Rope;
    public float Range = 50f;
    public float pullSpeed;
    public GameObject PullLimitObj;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent <Rigidbody> ();
        Rope = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown ("Fire1")){
            if (Physics.Raycast (cam.position, cam.forward, out hit))
            {
                if (hit.transform.gameObject.tag == "Grapple")
                {
                    Rope.enabled = true;
                    RaycastHit hit;
                    Rope.SetPosition(0, transform.position);


                    Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

                    if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, Range))
                    {
                        Rope.SetPosition(1, hit.point);
                    }
                    else
                    {
                        Rope.SetPosition(1, rayOrigin + (fpsCam.transform.forward * Range));
                    }
                    cc.mouseLook.XSensitivity = 0;
                    cc.mouseLook.YSensitivity = 0;
                    GrappleAttached = true;
                    rb.isKinematic = true;
                }

                if (hit.transform.gameObject.tag == "Grab")
                {
                    Rope.enabled = true;
                    RaycastHit hit;
                    Rope.SetPosition(0, transform.position);

                    Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

                    if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, Range))
                    {
                        Rope.SetPosition(1, hit.point);
                    }
                    else
                    {
                        Rope.SetPosition(1, rayOrigin + (fpsCam.transform.forward * Range));
                    }
                    GrabAttached = true;
                    //rb.isKinematic = true;
                }
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            cc.mouseLook.XSensitivity = 5;
            cc.mouseLook.YSensitivity = 5;
            GrappleAttached = false;
            GrabAttached = false;
            rb.isKinematic = false;
            rb.velocity = cam.forward * momentum;
            Rope.enabled = false;
        }

        if (GrappleAttached)
        {
            momentum += Time.deltaTime * speed;
            step = momentum * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, hit.point, step);
        }

        if (!GrappleAttached && momentum >= 0)
        {
            momentum -= Time.deltaTime * 5;
            step = 0;
        }

        if (cc.Grounded && momentum <= 0)
        {
            momentum = 0;
            step = 0;
        }

        if (GrabAttached)
        {
            step = pullSpeed;
            hit.transform.position = Vector3.MoveTowards(hit.transform.position, PullLimitObj.transform.position, step);
        }
    }
}
