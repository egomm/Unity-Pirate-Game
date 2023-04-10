using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depth = 1f;
    public float displacement = 3f;
    private float previousHeight = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate() // late update maybe?
    {
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.z);
        float angle = (Mathf.PI/4)*Mathf.Cos(Mathf.PI*waveHeight/2)*180/Mathf.PI; // convert radians -> degrees
        /*if (transform.position.y > previousHeight) { // height is increasing
            angle = -angle;
        }*/
        if (WaveManager.instance.GetSin(transform.position.z) > 0) {
            angle = -angle;
        }
        // if the player is going against the wave direction, change it
        Debug.Log(transform.rotation.eulerAngles.y);
        if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270) {
            angle = -angle;
        }
        //Debug.Log("WAVE HEIGHT: " + waveHeight + ", " + angle);
        //Debug.Log(WaveManager.instance.GetSin(transform.position.z));
        // Change the rotation accordingly
        // FIX THIS SO IT DOES Y = 90 DEGREES???
        transform.rotation = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        if (transform.position.y < waveHeight) {
            float displacementMult = Mathf.Clamp01((waveHeight-transform.position.y) / depth) * displacement;
            //Debug.Log("Displacement Multiplier: " + displacementMult);
            rigidBody.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacement, 0), ForceMode.Acceleration);
        }   
        previousHeight = transform.position.y;
    }
}
