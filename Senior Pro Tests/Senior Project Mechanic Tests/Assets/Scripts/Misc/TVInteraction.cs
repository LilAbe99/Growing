using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct GameOverEventArgs : IGameEvent { }
}

public class TVInteraction : MonoBehaviour
{
    public void OnUse()
    {
        EventManager.Instance.TriggerEvent(new Events.GameOverEventArgs());

        Destroy(this);
    }
}
