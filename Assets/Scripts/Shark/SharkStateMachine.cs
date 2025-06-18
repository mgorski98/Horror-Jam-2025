using UnityEngine;

public enum ESharkStateType
{
    Hidden,
    Circling,
    Chase
}

public class SharkStateMachine : MonoBehaviour
{
    [SerializeField] private Transform shark;
    [SerializeField] private Transform player;
    public WavesManager wavesManager;

    private SharkState currentState = null;

    public float freq1;
    public float offset1;

    void Start()
    {
        //ChangeState(ESharkStateType.Hidden);
        ChangeState(ESharkStateType.Circling);
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
                currentState = new HiddenSharkState(this);
                break;
            case ESharkStateType.Circling:
                currentState = new CirclingSharkState(this, shark, player);
                break;
            case ESharkStateType.Chase:
                currentState = new ChaseSharkState(this);
                break;
        }

        currentState?.HandleEnter();
    }


#if UNITY_EDITOR
    [ContextMenu("ChangeState_Hidden")]
    private void ChangeState_Hidden()
    {
        ChangeState(ESharkStateType.Hidden);
    }

    [ContextMenu("ChangeState_Circling")]
    private void ChangeState_Circling()
    {
        ChangeState(ESharkStateType.Circling);
    }

    [ContextMenu("ChangeState_Chase")]
    private void ChangeState_Chase()
    {
        ChangeState(ESharkStateType.Chase);
    }
#endif

}
