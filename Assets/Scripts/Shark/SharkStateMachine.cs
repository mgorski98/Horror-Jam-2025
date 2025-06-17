using UnityEngine;

public enum ESharkStateType
{
    Hidden,
    Circling,
    Chase
}

public class SharkStateMachine : MonoBehaviour
{
    private SharkState currentState = null;

    void Start()
    {
        
    }

    private void Update()
    {
        currentState?.HandleUpdate();
    }

    private void FixedUpdate()
    {
        currentState?.HandleFixedUpdate();
    }

    private void ChangeState(ESharkStateType sharkStateType)
    {
        currentState?.HandleExit();

        switch (sharkStateType)
        {
            case ESharkStateType.Hidden:
                currentState = new HiddenSharkState();
                break;
            case ESharkStateType.Circling:
                currentState = new CirclingSharkState();
                break;
            case ESharkStateType.Chase:
                currentState = new ChaseSharkState();
                break;
        }

        currentState?.HandleEnter();
    }
}
