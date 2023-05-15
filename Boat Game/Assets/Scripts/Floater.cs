using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {
    public Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        // Get information about the wave
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.z);
        float waveLength = WaveManager.instance.GetWaveLength();
        float waveAmplitude = WaveManager.instance.GetAmplitude();
        float waveSpeed = WaveManager.instance.GetSpeed();  
        float waveCos = ConvertToDegrees(WaveManager.instance.GetCos(transform.position.z));
        float multiplier = 0;
        if (waveLength > 0) {
            multiplier = Mathf.Atan(2*waveAmplitude/waveLength); // An approximation for the multiplier
        }
        // Mathf.PI/180 converts degrees to radians, gets the xAngle and zAngle based on the angle of the wave at the boats position
        float xAngle = -(Mathf.Cos(ConvertToRadians(transform.rotation.eulerAngles.y)))*multiplier*waveCos; 
        float zAngle = -(Mathf.Sin(ConvertToRadians(transform.rotation.eulerAngles.y)))*multiplier*waveCos;
        // Rotates the boat based on the waves 
        transform.rotation = Quaternion.Euler(xAngle, transform.rotation.eulerAngles.y, zAngle);
        if (transform.position.y < waveHeight-(waveAmplitude/5)) { // If the boat falls below 20% of the waves amplitude app the force
            rigidBody.AddForce(new Vector3(0, -Physics.gravity.y*2.75f, 0), ForceMode.Acceleration); // Add an upward force to float the boat
        }   
        // Move the boat upwards if it falls down too far below the waves 
        if (transform.position.y < waveHeight-(5*waveAmplitude) && waveAmplitude > 0) {
            transform.position = new Vector3(transform.position.x, waveHeight, transform.position.z);
        } else if (waveAmplitude == 0 && transform.position.y < -0.25) {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private float ConvertToDegrees(float radians) { // This function converts radians into degrees (eg. pi = 180 degrees)
        return radians*180/Mathf.PI;
    }

    private float ConvertToRadians(float degrees) { // This function converts degrees into radians (eg. 180 degrees = pi)
        return degrees*Mathf.PI/180;
    }
}
