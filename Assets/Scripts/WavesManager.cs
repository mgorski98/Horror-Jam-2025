using UnityEngine;

[ExecuteInEditMode]
public class WavesManager : MonoBehaviour
{
    [SerializeField]
    private Material waveMaterial;

    [SerializeField] private Vector4 wave1;
    [SerializeField] private Vector4 wave2;
    [SerializeField] private Vector4 wave3;
    [SerializeField] private float speed;

    [SerializeField]
    private Transform buoy;

    private float customTime = 0f;

    void Start()
    {
    }

    void Update()
    {
        customTime = Time.timeSinceLevelLoad;
        waveMaterial.SetFloat("_CustomTime", customTime);
        waveMaterial.SetFloat("_Speed", speed);
        waveMaterial.SetVector("_WaveA", wave1);
        waveMaterial.SetVector("_WaveB", wave2);
        waveMaterial.SetVector("_WaveC", wave3);
        
        if (buoy)
        {
            buoy.position = new Vector3(buoy.position.x, GetVerticalPositionFromPoint(buoy.position), buoy.position.z);
        }

        //float Y = InverseGerstnerWave(wave1, startPos, speed, time);

        //buoy.position = new Vector3(buoy.position.x, Y, buoy.position.z);
    }

    public float GetVerticalPositionFromPoint(Vector3 position)
    {
        Vector3 firstPos1 = GerstnerWave(wave1, position, speed, customTime);
        Vector3 secondPos1 = GerstnerWave(wave1, position - firstPos1, speed, customTime);

        Vector3 firstPos2 = GerstnerWave(wave2, position, speed, customTime);
        Vector3 secondPos2 = GerstnerWave(wave2, position - firstPos2, speed, customTime);

        Vector3 firstPos3 = GerstnerWave(wave3, position, speed, customTime);
        Vector3 secondPos3 = GerstnerWave(wave3, position - firstPos3, speed, customTime);

        return secondPos1.y + secondPos2.y + secondPos3.y;
    }

    private Vector3 GerstnerWave(Vector4 wave, Vector3 position, float speed, float time)
    {
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = 2f * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(9.8f / k);
        Vector2 d = new Vector2(wave.x, wave.y).normalized;
        float f = k * (Vector2.Dot(d, new Vector2(position.x, position.z)) - c * speed * time);
        float a = steepness / k;
        
        return new Vector3(
            d.x * (a * Mathf.Cos(f)),
            a * Mathf.Sin(f),
            d.y * (a * Mathf.Cos(f))
        );
    }

    /*private float InverseGerstnerWave(Vector4 wave, Vector3 finalPosition, float speed, float time)
    {
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = 2f * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(9.8f / k);
        Vector2 d = new Vector2(wave.x, wave.y).normalized;
        float a = steepness / k;
        float f = Mathf.Acos(finalPosition.x / d.x / a);

        float dot = (f / k) + c * speed * time;

        return a * sinf;
    }*/
}
