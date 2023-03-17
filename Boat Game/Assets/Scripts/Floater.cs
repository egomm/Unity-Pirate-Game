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
    void LateUpdate() // late update maybe?
    {
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.z);
        //Debug.Log("WAVE HEIGHT: " + waveHeight + ", " + WaveManager.instance.amplitude*Mathf.Sin(waveHeight)*360/Mathf.PI);
        // Change the rotation accordingly
        // FIX THIS SO IT DOES Y = 90 DEGREES???
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -WaveManager.instance.amplitude*Mathf.Sin(waveHeight)*360/Mathf.PI);
        if (transform.position.y < waveHeight) {
            float displacementMult = Mathf.Clamp01((waveHeight-transform.position.y) / depth) * displacement;
            rigidBody.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacement, 0), ForceMode.Acceleration);
        }   
    }
}
