using System.Collections;
using UnityEngine;

public class CirclingSharkState : SharkState
{
    private Transform playerTransform;
    private Transform sharkTransform;
    private Vector2 circlingRange = new Vector2(5f, 10f);
    private Vector2 submergeRange = new Vector2(-1f, .25f);

    private float sharkSpeed = 0.1f;

    public CirclingSharkState(SharkStateMachine sharkStateMachine, Transform sharkTransform, Transform playerTransform) : base(sharkStateMachine)
    {
        this.sharkTransform = sharkTransform;
        this.playerTransform = playerTransform;        
    }

    public override void HandleEnter() 
    {
        float distanceFromPlayer = Random.Range(circlingRange.x, circlingRange.y);
        Vector3 randomVector = new Vector3(Random.value * 2f - 1f, 0f, Random.value * 2f - 1f).normalized;

        sharkTransform.position = playerTransform.position + randomVector * distanceFromPlayer;
    }

    public override void HandleUpdate() 
    {
    }

    public override void HandleFixedUpdate() 
    {
        Vector3 fromPlayer = new Vector3(sharkTransform.position.x, 0f, sharkTransform.position.z) - new Vector3(playerTransform.position.x, 0f, playerTransform.position.z).normalized;
        Vector3 sharkDirection = Vector3.Cross(Vector3.up, fromPlayer);
        sharkTransform.forward = sharkDirection;

        sharkTransform.Translate(Vector3.forward * sharkSpeed);

        float waveVerticalPosition = sharkStateMachine.wavesManager.GetVerticalPositionFromPoint(sharkTransform.position);

        float Y = Mathf.Sin(Time.realtimeSinceStartup * sharkStateMachine.freq1 + sharkStateMachine.offset1);
        Y = Y * 0.5f - 0.25f; // (-1, 1) -> (-1, 0)

        sharkTransform.position = new Vector3(sharkTransform.position.x, waveVerticalPosition, sharkTransform.position.z);

        // get Y position from gerstner waves
        // hide and show shark when needed - ???
    }

    public override void HandleExit() { }
}
