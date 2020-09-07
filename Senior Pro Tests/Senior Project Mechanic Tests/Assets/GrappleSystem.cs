using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

//For some reason the first grapple is different from the rest, this is a big issue that needs to be addressed
public class GrappleSystem : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _maxGrappleDistance;
    [SerializeField] private float _minGrappleTime;
    [SerializeField] private float _grapplingMovementSpeed;

    [Header("Velocity Adjustment")]
    [SerializeField] private float _launchMultiplier;
    [SerializeField] private float _minLaunchMagnitude;
    [SerializeField] private float _maxLaunchMagnitude;
    [SerializeField] private float _minStartingYVelocity;

    [Header("Spring Joint Adjustment")]
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _spring;
    [SerializeField] private float _damper;
    [SerializeField] private float _massScale;

    private Camera _camera;
    private Rigidbody _rigidbody;
    private Mover _mover;
    private AdvancedWalkerController _walkerController;
    private CharacterInput _characterInput;
    private CustomGravitySystem _gravitySystem;
    private LineRenderer _lineRenderer;

    private Vector3[] _lineRendererPoints = new Vector3[2];
    private RaycastHit _hit;

    private SpringJoint _springJoint;

    private Vector3 _momentum = Vector3.zero;

    private bool _hasCollided = false;
    private bool _canGrapple = true;
    private bool _ignoreCollision = false;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();

        _rigidbody = GetComponent<Rigidbody>();
        _mover = GetComponent<Mover>();
        _walkerController = GetComponent<AdvancedWalkerController>();
        _characterInput = GetComponent<CharacterInput>();
        _gravitySystem = GetComponent<CustomGravitySystem>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && _canGrapple)
        {
            if (RaycastForGrappable())
            {
                StartCoroutine(GrappleRoutine());
            }
        }
    }

    private void FixedUpdate()
    {
        if(!_walkerController.enabled)
        {
            _rigidbody.AddForce(CalculateMovementVelocity());
        }
    }

    //Calculate and return movement velocity based on player input, controller state, ground normal [...];
    protected Vector3 CalculateMovementVelocity()
    {
        //Calculate (normalized) movement direction;
        Vector3 velocity = CalculateMovementDirection();

        //Multiply (normalized) velocity with movement speed;
        velocity *= _grapplingMovementSpeed;

        return velocity;
    }

    protected virtual Vector3 CalculateMovementDirection()
    {
        //If no character input script is attached to this object, return;
        if (_characterInput == null)
            return Vector3.zero;

        Vector3 _velocity = Vector3.zero;

        //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
        if (_characterInput == null)
        {
            _velocity += transform.right * _characterInput.GetHorizontalMovementInput();
            _velocity += transform.forward * _characterInput.GetVerticalMovementInput();
        }
        else
        {
            //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
            //Project movement direction so movement stays parallel to the ground;
            _velocity += Vector3.ProjectOnPlane(_camera.transform.right, transform.up).normalized * _characterInput.GetHorizontalMovementInput();
            _velocity += Vector3.ProjectOnPlane(_camera.transform.forward, transform.up).normalized * _characterInput.GetVerticalMovementInput();
        }

        //If necessary, clamp movement vector to magnitude of 1f;
        if (_velocity.magnitude > 1f)
            _velocity.Normalize();

        return _velocity;
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

        _gravitySystem.GravityScale = Constants.GRAPPLING_RELEASE_GRAVITY_SCALE;

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
        float distanceFromPoint = Vector3.Distance(_firePoint.position, _hit.point);

        if(_rigidbody.velocity.y < _minStartingYVelocity)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _minStartingYVelocity, _rigidbody.velocity.z);
        }

        _springJoint = gameObject.AddComponent<SpringJoint>();

        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = _hit.point;

        _springJoint.maxDistance = distanceFromPoint * _maxDistance;
        _springJoint.minDistance = distanceFromPoint * _minDistance;

        _springJoint.spring = _spring;
        _springJoint.damper = _damper;
        _springJoint.massScale = _massScale;
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
        float timer = 0f;

        _hasCollided = false;
        _canGrapple = false;
        _ignoreCollision = true;

        StartGrapple();

        while (timer < _minGrappleTime)
        {
            DrawLine();

            yield return null;

            timer += Time.deltaTime;
        }

        _canGrapple = true;
        _ignoreCollision = false;

        while (!_hasCollided && !finishedInput)
        {
            DrawLine();

            finishedInput = !Input.GetButton("Fire1");

            yield return null;
        }

        StopGrapple();

        while (!_hasCollided)
        {
            yield return null;
        }

        _gravitySystem.GravityScale = Constants.DEFAULT_GRAVITY_SCALE;
        _walkerController.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!_ignoreCollision)
        {
            _hasCollided = true;
        }
    }
}
