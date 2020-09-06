using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animation))]
public class AnimationSwitcher : MonoBehaviour
{
    public enum State
    {
        Open,
        Closed
    }

    [SerializeField] AnimationClip _openClip;
    [SerializeField] AnimationClip _closeClip;

    public UnityEvent OnAnimationStarted;
    public UnityEvent OnAnimationEnded;

    private Animation _animation;
    private State _currentState;

    private AnimationClip _queuedClip = null;

    protected void Awake()
    {
        _animation = GetComponent<Animation>();
        _currentState = State.Closed;

        _openClip.legacy = true;
        _closeClip.legacy = true;

        StartCoroutine(ClipPlayer());
    }

    public void Switch()
    {
        if (_queuedClip != null) return;

        switch (_currentState)
        {
            case State.Closed:
                _currentState = State.Open;
                _queuedClip =_openClip;
                break;
            case State.Open:
                _currentState = State.Closed;
                _queuedClip = _closeClip;
                break;
        }
    }

    public void Switch(bool open)
    {
        switch (open)
        {
            case false:
                _currentState = State.Closed;
                _queuedClip = _closeClip;
                break;
            case true:
                _currentState = State.Open;
                _queuedClip = _openClip;
                break;
        }
    }

    protected IEnumerator ClipPlayer()
    {
        bool animationPlayingLastFrame = false;

        while (true)
        {
            if(!_animation.isPlaying)
            {
                if (animationPlayingLastFrame) OnAnimationEnded?.Invoke();

                if (_queuedClip != null)
                {
                    OnAnimationStarted?.Invoke();
                    animationPlayingLastFrame = true;

                    _animation.clip = _queuedClip;
                    _animation.Play();

                    _queuedClip = null;
                }
            }

            yield return new WaitForSeconds(.1f);
        }
    }
}
