using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class DemoEndedListener : GameEventUserObject
{
    public UnityEvent unityEvent;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.GameOverEventArgs>(this, (args) => unityEvent.Invoke());
    }
}
