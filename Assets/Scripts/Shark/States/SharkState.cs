using UnityEngine;

public abstract class SharkState
{
    public virtual void HandleEnter() { }

    public virtual void HandleUpdate() { }

    public virtual void HandleFixedUpdate() { }

    public virtual void HandleExit() { }
}
