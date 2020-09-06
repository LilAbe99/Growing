using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class VisibilityBounds
{
    public float XFromCenter = .6f;
    public float YFromCenter = .6f;
}

public class VisibilityTracker : MonoBehaviour
{
    [SerializeField] private VisibilityBounds _visibilityBounds;
    [SerializeField] private float _maxSeeableDistance = 50;
    [SerializeField] private bool _useLinecast = true;

    public UnityEvent OnSeen;
    public UnityEvent OnUnseen;

    public bool IsCurrentlySeen { get; private set; } = false;

    private Camera _camera;

    private float _xBoundsPos;
    private float _xBoundsNeg;
    private float _yBoundsPos;
    private float _yBoundsNeg;

    private void Awake()
    {
        _camera = Camera.main;

        _xBoundsPos = .5F + _visibilityBounds.XFromCenter;
        _xBoundsNeg = .5F - _visibilityBounds.XFromCenter;
        _yBoundsPos = .5F + _visibilityBounds.YFromCenter;
        _yBoundsNeg = .5F - _visibilityBounds.YFromCenter;
    }

    protected virtual void Start()
    {
        StartCoroutine(VisibilityCheck());
    }

    private IEnumerator VisibilityCheck()
    {
        while(true)
        {
            Vector3 viewPos = _camera.WorldToViewportPoint(transform.position);

            bool seenCurrentFrame;
            float coolDown = .1f;

            if (viewPos.x > _xBoundsNeg && viewPos.x < _xBoundsPos && 
                viewPos.y > _yBoundsNeg && viewPos.y < _yBoundsPos &&  
                viewPos.z > 0 && viewPos.z < _maxSeeableDistance)
            {
                if (_useLinecast) seenCurrentFrame = !Physics.Linecast(transform.position, _camera.transform.position, LayerMask.NameToLayer("Player"));
                else seenCurrentFrame = true;
            }
            else
                seenCurrentFrame = false;

            if (IsCurrentlySeen && !seenCurrentFrame)
            {
                coolDown = .5f;
                OnUnseen?.Invoke();
            }
            else if (!IsCurrentlySeen && seenCurrentFrame)
            {
                coolDown = .5f;
                OnSeen?.Invoke();
            }

            IsCurrentlySeen = seenCurrentFrame;
            
            yield return new WaitForSeconds(coolDown);
        }
    }
}
