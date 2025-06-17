using UnityEngine;

public class WavePosition : MonoBehaviour
{
    [SerializeField]
    private Material waveMaterial;

    [SerializeField] private Vector4 wave1;
    [SerializeField] private Vector4 wave2;
    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform buoy;
    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = buoy.position;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.timeSinceLevelLoad;

        waveMaterial.SetFloat("_CustomTime", time);

        Vector3 firstPos1 = GerstnerWave(wave1, startPos, speed, time);
        Vector3 secondPos1 = GerstnerWave(wave1, startPos - firstPos1, speed, time);

        Vector3 firstPos2 = GerstnerWave(wave2, startPos, speed, time);
        Vector3 secondPos2 = GerstnerWave(wave2, startPos - firstPos2, speed, time);

        buoy.position = new Vector3(buoy.position.x, secondPos1.y + secondPos2.y, buoy.position.z);

        //float Y = InverseGerstnerWave(wave1, startPos, speed, time);

        //buoy.position = new Vector3(buoy.position.x, Y, buoy.position.z);
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
        

        //return a * Mathf.Sin(f);
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
