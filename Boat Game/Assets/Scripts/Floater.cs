using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depth = 1f;
    public float displacement = 3f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.z);
        float waveLength = WaveManager.instance.GetWaveLength();
        float waveAmplitude = WaveManager.instance.GetAmplitude();
        float waveCos = ConvertToDegrees(WaveManager.instance.GetCos(transform.position.z));
        float multiplier = Mathf.Atan(2*waveAmplitude/waveLength);
        // Mathf.PI/180 converts degrees to radians, gets the xAngle and zAngle based on the angle of the wave at the boats position
        float xAngle = -(Mathf.Cos(ConvertToRadians(transform.rotation.eulerAngles.y)))*multiplier*waveCos; 
        float zAngle = -(Mathf.Sin(ConvertToRadians(transform.rotation.eulerAngles.y)))*multiplier*waveCos;
        // Rotates the boat based on the waves 
        transform.rotation = Quaternion.Euler(xAngle, transform.rotation.eulerAngles.y, zAngle);
        if (transform.position.y < waveHeight-(waveAmplitude/5)) { // Need to improve + make use for the displacement multiplier 
            float displacementMult = Mathf.Clamp01((waveHeight-transform.position.y) / depth) * displacement;
            rigidBody.AddRelativeForce(new Vector3(0, -Physics.gravity.y*2.75f, 0), ForceMode.Acceleration);
        }   
    }

    private float ConvertToDegrees(float radians) { // This function converts radians into degrees (eg. pi = 180 degrees)
        return radians*180/Mathf.PI;
    }

    private float ConvertToRadians(float degrees) {
        return degrees*Mathf.PI/180;
    }
}
