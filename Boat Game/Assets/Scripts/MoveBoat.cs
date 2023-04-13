using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveBoat : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 0;
    [SerializeField] float turnSpeed = 45;
    [SerializeField] float speed = 0;
    private Rigidbody playerRb;
    [SerializeField] GameObject centreOfMass;
    [SerializeField] float maxSpeed = 4.167f;
    [SerializeField] TextMeshProUGUI speedometerText;
    private bool test = false;
    private bool finished = false;
    private bool xComplete = false;
    private bool zComplete = false;
    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = centreOfMass.transform.position;
        Invoke("WaveVelocity", 0.5f);
    }

    void WaveVelocity() {
        playerRb.AddForce(new Vector3(0, 0, -2f), ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()   
    {
        //Debug.Log(playerRb.velocity.z);
        float waveAmplitude = WaveManager.instance.GetAmplitude();
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        playerRb.AddRelativeForce(Vector3.forward * speedMultiplier * forwardInput, ForceMode.Acceleration);
        float velocity = Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2));
        if (forwardInput <= 0 && !test && velocity > 0 && !finished) {
            test = true;
            Debug.Log("TEST!");
        }
        if (forwardInput > 0) {
            test = false;
            finished = false;
        }
        if (test) {
            Debug.Log(playerRb.velocity.x + ", " + playerRb.velocity.z);
            if (!zComplete) {
                if (playerRb.velocity.z > -1.95) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, playerRb.velocity.z-Time.deltaTime);
                } else if (playerRb.velocity.z < -2.05) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, playerRb.velocity.z+Time.deltaTime);
                } else {
                    playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, -2);
                    zComplete = true;
                }
            }
            if (!xComplete) {
                Debug.Log("X NOT COMPLETE!");
                if (playerRb.velocity.x > 0.05) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x-(Time.deltaTime), playerRb.velocity.y, playerRb.velocity.z);
                } else if (playerRb.velocity.x < -0.05) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x+(Time.deltaTime), playerRb.velocity.y, playerRb.velocity.z);
                } else {
                    playerRb.velocity = new Vector3(0, playerRb.velocity.y, playerRb.velocity.z);
                    xComplete = true;
                }
            }

            if (xComplete && zComplete) {
                xComplete = false;
                zComplete = false;
                test = false;
                finished = true;
                Debug.Log("DONE!");
            }
        }

        /*if (forwardInput <= 0 && !test && Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2)) > 0) {
             test = true;
             Debug.Log("CALLED TEST");
             Debug.Log("OG: " + Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2)));
             //playerRb.AddRelativeForce(Vector3.forward * speedMultiplier * forwardInput, ForceMode.Acceleration);
        }
        if (test) {
            Debug.Log("FINAL: " + Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2)));
            Debug.Log(Time.deltaTime);
            if (playerRb.velocity.z > 0) {
                playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, playerRb.velocity.z-Time.deltaTime);
            } else {
                playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, 0);
                test = false;
            }
        }*/
        //playerRb.velocity = new Vector3(0, 0, -2f);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        speed = Mathf.RoundToInt(playerRb.velocity.magnitude*3.6f);
        if (playerRb.velocity.y > 2*waveAmplitude) {
            playerRb.velocity = new Vector3(playerRb.velocity.x, 2*waveAmplitude, playerRb.velocity.z);
        }
        velocity = Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2));
        //Debug.Log(2*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)));
        if (velocity+2*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)) > maxSpeed) {
            playerRb.velocity = playerRb.velocity.normalized * (maxSpeed-2*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)));
            speed = Mathf.RoundToInt((maxSpeed-2*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)))*3.6f);
        }
        speedometerText.SetText("Speed: " + speed + " kmph");
    }

    private float ConvertToRadians(float degrees) {
        return degrees*Mathf.PI/180;
    }
}
