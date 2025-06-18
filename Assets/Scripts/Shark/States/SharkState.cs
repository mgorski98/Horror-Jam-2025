using UnityEngine;

public abstract class SharkState
{
    protected SharkStateMachine sharkStateMachine;

    public SharkState(SharkStateMachine sharkStateMachine)
    {
        this.sharkStateMachine = sharkStateMachine;
    }

    public virtual void HandleEnter() { }

    public virtual void HandleUpdate() { }

    public virtual void HandleFixedUpdate() { }

    public virtual void HandleExit() { }
}
