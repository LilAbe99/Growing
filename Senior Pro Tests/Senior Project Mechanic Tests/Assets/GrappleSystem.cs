using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class GrappleSystem : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _maxGrappleDistance;

    [Header("Velocity Adjustment")]
    [SerializeField] private float _launchMultiplier;
    [SerializeField] private float _minLaunchMagnitude;
    [SerializeField] private float _maxLaunchMagnitude;


    private Camera _camera;
    private Rigidbody _rigidbody;
    private AdvancedWalkerController _walkerController;
    private CustomGravitySystem _gravitySystem;
    private LineRenderer _lineRenderer;

    private Vector3[] _lineRendererPoints = new Vector3[2];
    private RaycastHit _hit;

    private SpringJoint _springJoint;

    private bool _hasCollided = false;
    private bool _canGrapple = true;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();

        _rigidbody = GetComponent<Rigidbody>();
        _walkerController = GetComponent<AdvancedWalkerController>();
        _gravitySystem = GetComponent<CustomGravitySystem>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.enabled = false;

        //For some reason the first grapple is different
        StartCoroutine(GrappleRoutine());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (RaycastForGrappable())
            {
                StartCoroutine(GrappleRoutine());
            }
        }
    }

    private bool RaycastForGrappable() => Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hit, _maxGrappleDistance, _raycastMask);

    private void StartGrapple()
    {
        _lineRenderer.enabled = true;
        _walkerController.enabled = false;

        _gravitySystem.GravityScale = Constants.GRAPPLING_GRAVITY_SCALE;

        InitializeSpringJoint();
        DrawLine();
    }

    private void StopGrapple()
    {
        _lineRenderer.enabled = false;

        RemoveSpringJoint();
        
        _rigidbody.velocity *= _launchMultiplier;

        if(_rigidbody.velocity.magnitude < _minLaunchMagnitude)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _minLaunchMagnitude;
        }
        else if (_rigidbody.velocity.magnitude > _maxLaunchMagnitude)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxLaunchMagnitude;
        }
    }

    private void InitializeSpringJoint()
    {
        float distanceFromPoint = Vector3.Distance(transform.position, _hit.point);

        _springJoint = PlayerRef.Instance.gameObject.AddComponent<SpringJoint>();

        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = _hit.point;

        _springJoint.maxDistance = distanceFromPoint * .6f;
        _springJoint.minDistance = distanceFromPoint * .25f;

        _springJoint.spring = 6f;
        _springJoint.damper = 7f;
        _springJoint.massScale = 4.5f;
    }

    private void RemoveSpringJoint()
    {
        Destroy(_springJoint);
    }

    private void DrawLine()
    {
        _lineRendererPoints[0] = _firePoint.position;
        _lineRendererPoints[1] = _hit.point;

        _lineRenderer.SetPositions(_lineRendererPoints);
    }

    private IEnumerator GrappleRoutine()
    {
        bool finishedInput = false;

        _hasCollided = false;
        _canGrapple = false;

        StartGrapple();

        while (!_hasCollided && !finishedInput)
        {
            DrawLine();

            finishedInput = Input.GetButtonUp("Fire1");

            yield return null;
        }

        StopGrapple();

        while (!_hasCollided)
        {
            yield return null;
        }

        _gravitySystem.GravityScale = Constants.DEFAULT_GRAVITY_SCALE;
        _walkerController.enabled = true;

        _canGrapple = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _hasCollided = true;
    }
}
