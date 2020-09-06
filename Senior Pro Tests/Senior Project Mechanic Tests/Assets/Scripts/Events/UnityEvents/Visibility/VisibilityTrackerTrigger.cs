using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class VisibilityTrackerState
{
    public VisibilityTracker[] ObjectsToTrack;
    public UnityEvent OnTrackedObjectsInvisible;
}

public class VisibilityTrackerTrigger : VisibilityTracker
{
    [SerializeField] private bool _loop = false;
    [SerializeField] private float _mustBeVisibleTime = 1f;

    public VisibilityTrackerState[] States = new VisibilityTrackerState[1];

    private Queue<VisibilityTrackerState> _stateQueue;

    private bool _beenSeen;
    private bool _checkRoutineRunning;
    private bool _seenRoutineRunning;

    private VisibilityTrackerState _currentState;

    protected override void Start()
    {
        _stateQueue = new Queue<VisibilityTrackerState>(States);

        Reset();

        base.Start();
    }

    private void FixedUpdate()
    {
        if (IsCurrentlySeen)
        {
            if (!_seenRoutineRunning) StartCoroutine(SeenRoutine());
        }
        else
        {
            if (_beenSeen && !_checkRoutineRunning) StartCoroutine(CheckTrackerRoutine());
        }
    }

    private void Reset()
    {
        StartCoroutine(WaitForActionRoutine(.2f, () => 
        {
            if (_stateQueue.Count <= 0)
            {
                if (_loop) _stateQueue = new Queue<VisibilityTrackerState>(States);
                else Destroy(this);
            }

            _beenSeen = false;
            _checkRoutineRunning = false;

            _currentState = _stateQueue.Dequeue();
        }));
    }

    private IEnumerator CheckTrackerRoutine()
    {
        _checkRoutineRunning = true;

        bool allTrackedObjectsOutOfView = false;

        while(!allTrackedObjectsOutOfView)
        {
            allTrackedObjectsOutOfView = _currentState.ObjectsToTrack.OfType<VisibilityTracker>().All(x => !x.IsCurrentlySeen) && !IsCurrentlySeen;

            yield return null;
        }

        _currentState.OnTrackedObjectsInvisible.Invoke();

        Reset();
    }

    private IEnumerator SeenRoutine()
    {
        _seenRoutineRunning = true;

        yield return new WaitForSeconds(_mustBeVisibleTime);

        if (IsCurrentlySeen) _beenSeen = true;
        _seenRoutineRunning = false;
    }

    private IEnumerator WaitForActionRoutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);

        action.Invoke();
    }
}
