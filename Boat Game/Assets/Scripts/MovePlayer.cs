using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {
    private Rigidbody playerRb;
    private Animator playerAnim;
    public float movementSpeed = 10f;
    private float turnSpeed = 50f;
    public bool isOnGround = true;
    Vector3 movement;

    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        float zVelocity = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") : 0; // Get the z velocity 
        movement = new Vector3(0, 0, zVelocity);
        movePlayer(movement);
        float horizontalInput = Input.GetKey(KeyCode.D) ? 1 : 0; // Controls the player's right movement
        if (Input.GetKey(KeyCode.A)) { // Controls the player's left movement
            horizontalInput--;
        }
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        if (Input.GetKey(KeyCode.Space) && isOnGround) { // If the player is on the ground, allow the player to jump
            playerRb.AddForce(Vector3.up * 3, ForceMode.VelocityChange);
            isOnGround = false;
        }
        playerAnim.SetBool("moving", Input.GetAxis("Vertical") > 0); // Set animation if player is moving forward
        if (distanceFromCentre() > 25) { // If the player is more than 25 units from the centre
            float angle = 0;
            float multiplier = 25;
            if (playerRb.position.x != 0) { // To avoid dividing my 0 
                angle = Mathf.Atan(playerRb.position.z/playerRb.position.x);
                multiplier = 25*playerRb.position.x/Mathf.Abs(playerRb.position.x);
            }
            // Get the new player positions based on trig
            float newPlayerX = multiplier*Mathf.Cos(angle);
            float newPlayerZ = multiplier*Mathf.Sin(angle);
            playerRb.position = new Vector3(newPlayerX, playerRb.position.y, newPlayerZ);
        }
    }

    private void movePlayer(Vector3 direction) {
        direction = playerRb.rotation * direction; // make the direction relative to the Rigidbody
        playerRb.MovePosition(playerRb.position + direction * movementSpeed * Time.fixedDeltaTime); // using the fixed delta time as this is called from the FixedUpdate() method
    }

    private float distanceFromCentre() { // Assuming centre is (0, 0) - temporary
        return Mathf.Sqrt(Mathf.Pow(playerRb.position.x, 2) + Mathf.Pow(playerRb.position.z, 2));
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Island")) {
            isOnGround = true;
        }
    }
}
