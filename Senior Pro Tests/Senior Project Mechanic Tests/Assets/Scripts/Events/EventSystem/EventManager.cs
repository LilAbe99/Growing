using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectActionPair<T>
{
    public object Object { get; }
    public Action<T> Action { get; }

    public ObjectActionPair(object obj, Action<T> action)
    {
        Object = obj;
        Action = action;
    }
}

public class EventManager : MonoBehaviour
{
    private const int _TRIGGERED_EVENTS_PER_FRAME = 5;

    public static EventManager Instance { get; private set; }

    public Dictionary<Type, List<ObjectActionPair<IGameEvent>>> _eventListeners = new Dictionary<Type, List<ObjectActionPair<IGameEvent>>>();

    private Queue<IGameEvent> _triggeredEventQueue = new Queue<IGameEvent>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void LateUpdate()
    {
        int eventsToTrigger = _TRIGGERED_EVENTS_PER_FRAME;

        while(eventsToTrigger > 0 && _triggeredEventQueue.Count > 0)
        {
            TriggerEventImmediate(_triggeredEventQueue.Dequeue());
            eventsToTrigger--;
        }
    }

    public void AddListener<T>(object obj, Action<T> action) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners.Add(eventType, new List<ObjectActionPair<IGameEvent>>());
        }

        _eventListeners[eventType].Add(new ObjectActionPair<IGameEvent>(obj, new Action<IGameEvent>((eventArgs) => action.Invoke((T)eventArgs))));
    }

    public void TriggerEventImmediate(IGameEvent gameEventArgs)
    {
        Type eventType = gameEventArgs.GetType();

        if (_eventListeners.ContainsKey(eventType))
        {
            for (int i = 0; i < _eventListeners[eventType].Count; i++)
            {
                _eventListeners[eventType][i].Action.Invoke(gameEventArgs);
            }
        }
    }

    public void TriggerEvent(IGameEvent gameEventArgs)
    {
        _triggeredEventQueue.Enqueue(gameEventArgs);
    }

    public void RemoveListener<T>(object obj, Action<T> action) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (_eventListeners != null && _eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType] = _eventListeners[eventType].OfType<ObjectActionPair<IGameEvent>>().Where(x => x.Object != obj).ToList();
        }
    }
}
