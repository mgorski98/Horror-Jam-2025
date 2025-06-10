using UnityEngine;

public class ShipWavesSimulation : MonoBehaviour
{
    [SerializeField, Tooltip("Part of the ship that is not responsible for movement, but has all the ship objects that need to be simulated as a whole")]
    private Transform ShipWaveSimulationRoot;
    [SerializeField]
    private float WavesStrength_Horizontal;
    [SerializeField]
    private float WavesFrequency_Horizontal;
    [SerializeField]
    private float WavesStrength_Vertical;
    [SerializeField]
    private float WavesFrequency_Vertical;

    private void SimulateWavesMovement() {
        var pos = default(Vector3);
        pos.x = Mathf.Sin(Time.time * WavesFrequency_Horizontal) * WavesStrength_Horizontal;
        pos.z = Mathf.Cos(Time.time * WavesFrequency_Vertical) * WavesStrength_Vertical;
        ShipWaveSimulationRoot.localRotation = Quaternion.Euler(pos);
    }

    private void LateUpdate() {
        SimulateWavesMovement();
    }
}
