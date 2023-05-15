using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    // This static instance allows other scripts to access non-static public methods inside of this script
    public static WaveManager instance;

    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;

    // Awake is called before any Start functions
    private void Awake() {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // The wave offset is determined by the time elapsed and the speed of the wave
        offset += Time.deltaTime * speed;
    }

    public float GetWaveHeight(float _z) {
        // Get the wave height at a specific z coordinate
        if (length == 0) {
            return 0;
        }
        return amplitude * Mathf.Sin(_z / length + offset);
    }

    public float GetCos(float _z) {
        // Get the angle at a specific z coordinate
        if (length == 0) {
            return 0;
        }
        return Mathf.Cos(_z/length + offset);
    }

    public float GetWaveLength() {
        // Method for getting the wave length
        return length;
    }

    public float GetAmplitude() {
        // Method for getting the wave amplitude
        return amplitude;
    }

    public float GetSpeed() {
        // Method for getting the wave speed
        return speed;
    }

    public float GetOffset() {
        // Method for getting the wave offset
        return offset;
    }
}
