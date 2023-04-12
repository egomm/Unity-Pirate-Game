using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveBoat : MonoBehaviour
{
    [SerializeField] float horsePower = 0;
    [SerializeField] float turnSpeed = 45;
    [SerializeField] float speed = 0;
    private Rigidbody playerRb;
    [SerializeField] GameObject centreOfMass;
    [SerializeField] float maxSpeed = 4.167f;
    [SerializeField] TextMeshProUGUI speedometerText;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = centreOfMass.transform.position;
    }

    // Update is called once per frame
    void Update()   
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Debug.Log(forwardInput);
        playerRb.AddRelativeForce(Vector3.forward * horsePower * forwardInput, ForceMode.Acceleration);
        Debug.Log(playerRb.velocity.x);
        if (forwardInput <= 0) {
             playerRb.velocity = Vector3.zero;
        }

        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        speed = Mathf.RoundToInt(playerRb.velocity.magnitude*3.6f);
        // turn the speed into average speed
        if (playerRb.velocity.magnitude > maxSpeed) {
            playerRb.velocity = playerRb.velocity.normalized * maxSpeed;
            speed = Mathf.RoundToInt(maxSpeed*3.6f);
        }
        speedometerText.SetText("Speed: " + speed + " kmph");
    }
}
