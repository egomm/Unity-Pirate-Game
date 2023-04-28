using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveBoat : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 0;
    [SerializeField] float turnSpeed = 45;
    [SerializeField] float speed = 0;
    [SerializeField] float maxSpeed = 4.167f;
    private Rigidbody playerRb;
    [SerializeField] GameObject centreOfMass;
    [SerializeField] TextMeshProUGUI speedometerText;
    private bool deaccelerating = false;
    private bool finished = false;
    private bool xComplete = false;
    private bool zComplete = false;
    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
        Debug.Log("HERE: " + IslandManager.startingCoordinates);
        playerRb.transform.position = IslandManager.startingCoordinates;
        playerRb.transform.Rotate(IslandManager.startingAngle);
        playerRb.centerOfMass = centreOfMass.transform.position;
        Invoke("WaveVelocity", 0.5f);
    }

    void WaveVelocity() {
        float waveSpeed = WaveManager.instance.GetSpeed();
        playerRb.AddForce(new Vector3(0, 0, -2*waveSpeed), ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update() {
        // Set the position & rotation of the player riding the boat to the same as the boat
        GameObject child = gameObject.transform.Find("Idle Bandit").gameObject;
        child.transform.position = new Vector3(transform.position.x, transform.position.y+0.4f, transform.position.z);
        child.transform.rotation = transform.rotation;
        // Get information about the wave
        float waveSpeed = WaveManager.instance.GetSpeed();
        float waveAmplitude = WaveManager.instance.GetAmplitude();
        float forwardInput = Input.GetKey(KeyCode.W) ? 1 : 0; // Controls the boat's forward movement
        float horizontalInput = Input.GetKey(KeyCode.D) ? 1 : 0;// Controls the boat's right movement
        if (Input.GetKey(KeyCode.A)) { // Controls the boat's left movement
            horizontalInput--;
        }
        playerRb.AddRelativeForce(Vector3.forward * speedMultiplier * forwardInput, ForceMode.Acceleration); // Accelerate boat forward relative to the boat's direction
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput); // Rotate the boat
        float velocity = Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2)); // xz velocity
        // If the player isn't inputting any forward movement, deaccelerate the boat until it has stopped or the player inputs forward movement again
        if (forwardInput == 0 && !deaccelerating && velocity > 0 && !finished) {
            deaccelerating = true;
        }
        if (forwardInput != 0) {
            deaccelerating = false;
            finished = false;
        }
        // Deacceleration mechanism
        if (deaccelerating) {
            if (!xComplete) { // Deaccelerate the boat on the x component
                if (playerRb.velocity.x > (0.05*maxSpeed/4)) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x-(Time.deltaTime*maxSpeed/4), playerRb.velocity.y, playerRb.velocity.z);
                } else if (playerRb.velocity.x < -(0.05*maxSpeed/4)) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x+(Time.deltaTime*maxSpeed/4), playerRb.velocity.y, playerRb.velocity.z);
                } else { // Check if the boat has finished deaccelerating on the x component
                    playerRb.velocity = new Vector3(0, playerRb.velocity.y, playerRb.velocity.z);
                    xComplete = true;
                }
            }
            if (!zComplete) { // Deaccelerate the boat on the z component
                if (playerRb.velocity.z > -2*waveSpeed+(0.05*maxSpeed/4)) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, playerRb.velocity.z-(Time.deltaTime*maxSpeed/4));
                } else if (playerRb.velocity.z < -2*waveSpeed-(0.05*maxSpeed/4)) {
                    playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, playerRb.velocity.z+(Time.deltaTime*maxSpeed/4));
                } else { // Check if the boat has finished deaccelerating on the z component
                    playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, -2*waveSpeed);
                    zComplete = true;
                }
            }
            if (xComplete && zComplete) { // If finished deaccelerating on the x and z components, stop checking for deacceleration
                xComplete = false;
                zComplete = false;
                deaccelerating = false;
                finished = true;
            }
        }
        speed = Mathf.RoundToInt(playerRb.velocity.magnitude*3.6f); // boat speed
        if (playerRb.velocity.y > 2*waveAmplitude) { // limit the boat's y velocity (will help prevent the boat sinking too much)
            playerRb.velocity = new Vector3(playerRb.velocity.x, 2*waveAmplitude, playerRb.velocity.z);
        }
        velocity = Mathf.Sqrt(Mathf.Pow(playerRb.velocity.x, 2) + Mathf.Pow(playerRb.velocity.z, 2)); // new xz velocity
        // Ensure that the player isn't going over the max speed (determined by the boat, the waves, and the direction the boat is travelling)
        if (velocity+2*waveSpeed*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)) > maxSpeed) {
            playerRb.velocity = playerRb.velocity.normalized * (maxSpeed-2*waveSpeed*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)));
            speed = Mathf.RoundToInt((maxSpeed-2*waveSpeed*Mathf.Cos(ConvertToRadians(playerRb.rotation.eulerAngles.y)))*3.6f);
        }
        speedometerText.SetText("Speed: " + speed + " kmph");
    }

    private float ConvertToRadians(float degrees) {
        return degrees*Mathf.PI/180;
    }
}
