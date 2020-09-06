using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CameraFacingBillboard : GameEventUserObject
{
    private Camera _camera;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _camera = Camera.main;

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
            _camera.transform.rotation * Vector3.up);
    }

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.GamePausedArgs>(this, (args) => _spriteRenderer.enabled = false);
        EventManager.Instance.AddListener<Events.GameUnpausedArgs>(this, (args) => _spriteRenderer.enabled = true);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.GamePausedArgs>(this, (args) => _spriteRenderer.enabled = false);
        EventManager.Instance.RemoveListener<Events.GameUnpausedArgs>(this, (args) => _spriteRenderer.enabled = true);
    }
}
