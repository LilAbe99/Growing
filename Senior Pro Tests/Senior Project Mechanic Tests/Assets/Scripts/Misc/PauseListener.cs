using UnityEngine.Events;

public class PauseListener : GameEventUserObject
{
    public UnityEvent OnPaused;
    public UnityEvent OnUnpaused;

    public override void Subscribe()
    {
        EventManager.Instance.AddListener<Events.GamePausedArgs>(this, Paused);
        EventManager.Instance.AddListener<Events.GameUnpausedArgs>(this, Unpaused);
    }

    public override void Unsubscribe()
    {
        EventManager.Instance.RemoveListener<Events.GamePausedArgs>(this, Paused);
        EventManager.Instance.RemoveListener<Events.GameUnpausedArgs>(this, Unpaused);
    }

    private void Paused(Events.GamePausedArgs args) => OnPaused.Invoke();
    private void Unpaused(Events.GameUnpausedArgs args) => OnUnpaused.Invoke();
}
