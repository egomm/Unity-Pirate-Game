using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovePlayer : MonoBehaviour {
    private Rigidbody playerRb;
    private Animator playerAnim;
    public float movementSpeed = 10f;
    private float turnSpeed = 50f;
    public bool isOnGround = true;
    private float lastSwimTime = 0;
    public static MovePlayer instance;
    public static GameObject player;
    Vector3 movement;

    void Awake() {
        player = gameObject;
    }

    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 centre = IslandManager.currentCentre;
        float zVelocity = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") : 0; // Get the z velocity 
        movement = new Vector3(0, 0, zVelocity);
        movePlayer(movement);
        float horizontalInput = Input.GetKey(KeyCode.D) ? 1 : 0; // Controls the player's right movement
        if (Input.GetKey(KeyCode.A)) { // Controls the player's left movement
            horizontalInput--;
        }
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        // Count the player so that it is like that they are on ground 
        // Player is underwater
        float jumpMultiplier = 3f;
        if (Input.GetKey(KeyCode.Space)) {
            float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.z);
            if (transform.position.y < waveHeight && (Time.time - lastSwimTime) > 0.3f) {
                // Allow the player to "jump" if they are underwater
                lastSwimTime = Time.time; // Based on the game run time
                jumpMultiplier *= 0.55f + (Mathf.Abs(transform.position.y - waveHeight) / 3);
                playerRb.AddForce(Vector3.up * jumpMultiplier, ForceMode.VelocityChange);
            } else if (isOnGround) { // If the player is on the ground, allow the player to jump
                playerRb.AddForce(Vector3.up * jumpMultiplier, ForceMode.VelocityChange);
                isOnGround = false;
            }
        }
        playerAnim.SetBool("moving", Input.GetAxis("Vertical") > 0); // Set animation if player is moving forward
        Vector2 playerTwoDimensional = new Vector2(transform.position.x, transform.position.z);
        if (SceneManager.GetActiveScene().name != "Pirate Ship") {
            float radius = (float)IslandManager.islandInformation[centre]["radius"];
            float multiplier = 2f * radius;
            Vector2 centreTwoDimensional = new Vector2(centre.x, centre.z);
            if (Vector2.Distance(playerTwoDimensional, centreTwoDimensional) > multiplier) { // If the player is more than 2x the radius from the centre 
                float angle = 0;
                if (playerRb.position.x != 0) { // To avoid dividing my 0 
                    angle = Mathf.Atan((playerRb.position.z - centre.z) / (playerRb.position.x - centre.x));
                    multiplier *= (playerRb.position.x - centre.x) / Mathf.Abs(playerRb.position.x - centre.x);
                }
                // Get the new player positions based on trig
                float newPlayerX = multiplier * Mathf.Cos(angle);
                float newPlayerZ = multiplier * Mathf.Sin(angle);
                playerRb.position = new Vector3(newPlayerX + centre.x, playerRb.position.y, newPlayerZ + centre.z);
            }
        } else {
            float multiplier = 15f;
            Vector3 pirateShipCoordinates = PirateSceneManager.currentPirateShipCoordinates;
            Vector2 pirateShipTwoDimensional = new Vector2(pirateShipCoordinates.x, pirateShipCoordinates.z);
            if (Vector2.Distance(playerTwoDimensional, pirateShipTwoDimensional) > multiplier) {
                float angle = 0;
                if (playerRb.position.x != 0) { // To avoid dividing my 0 
                    angle = Mathf.Atan((playerRb.position.z - pirateShipCoordinates.z) / (playerRb.position.x - pirateShipCoordinates.x));
                    multiplier *= (playerRb.position.x - pirateShipCoordinates.x) / Mathf.Abs(playerRb.position.x - pirateShipCoordinates.x);
                }
                // Get the new player positions based on trig
                float newPlayerX = multiplier * Mathf.Cos(angle);
                float newPlayerZ = multiplier * Mathf.Sin(angle);
                playerRb.position = new Vector3(newPlayerX + pirateShipCoordinates.x, playerRb.position.y, newPlayerZ + pirateShipCoordinates.z);
            }
        }
    }

    private void movePlayer(Vector3 direction) {
        direction = playerRb.rotation * direction; // make the direction relative to the Rigidbody
        playerRb.MovePosition(playerRb.position + direction * movementSpeed * Time.fixedDeltaTime); // using the fixed delta time as this is called from the FixedUpdate() method
    }

    private void OnCollisionEnter(Collision collision) { // If standing on boat/island/dock, allow the player to jump
        if (collision.gameObject.CompareTag("Island") || collision.gameObject.CompareTag("Boat") || collision.gameObject.CompareTag("Dock") || collision.gameObject.CompareTag("Pirate Ship")) {
            isOnGround = true;
        }
    }
}
