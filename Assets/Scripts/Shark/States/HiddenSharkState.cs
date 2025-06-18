using UnityEngine;

public class HiddenSharkState : SharkState
{
    public HiddenSharkState(SharkStateMachine sharkStateMachine) : base(sharkStateMachine) { }

    public override void HandleEnter() { }

    public override void HandleUpdate() { }

    public override void HandleFixedUpdate() { }

    public override void HandleExit() { }
}
