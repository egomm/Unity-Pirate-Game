using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            //Debug.Log("Instance already exists");
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        offset += Time.deltaTime * speed;
    }

    public float GetWaveHeight(float _z) {
        if (length == 0) {
            return 0;
        }
        return amplitude * Mathf.Sin(_z / length + offset);
    }

    public float GetCos(float _z) {
        if (length == 0) {
            return 0;
        }
        return Mathf.Cos(_z/length + offset);
    }

    public float GetWaveLength() {
        return length;
    }

    public float GetAmplitude() {
        return amplitude;
    }

    public float GetSpeed() {
        return speed;
    }

    public float GetOffset() {
        return offset;
    }
}
