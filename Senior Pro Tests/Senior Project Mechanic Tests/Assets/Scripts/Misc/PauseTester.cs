using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTester : MonoBehaviour
{
    [ContextMenu("Pause")]
    public void Pause() => EventManager.Instance.TriggerEventImmediate(new Events.GamePausedArgs());

    [ContextMenu("Unpause")]
    public void Unpause() => EventManager.Instance.TriggerEventImmediate(new Events.GameUnpausedArgs());

}
