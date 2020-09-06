using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementDocument : MonoBehaviour
{
    public void OnUse()
    {
        EventManager.Instance.TriggerEvent(new Events.StateChangedEventArgs(2));
    }
}
