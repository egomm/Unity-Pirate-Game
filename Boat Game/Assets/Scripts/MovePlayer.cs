using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private float speed = 5;
    private Rigidbody playerRb;
    private Animator playerAnim;
    public float movementSpeed = 1f;
    Vector3 movement;

    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
    }

    void Update() {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    }

    // Update is called once per frame
    void FixedUpdate() {
        /*Vector3 Vec = transform.localPosition;  
        Vec.x += Input.GetAxis("Horizontal") * Time.deltaTime * speed;  
        Vec.z += Input.GetAxis("Vertical") * Time.deltaTime * speed;  
        transform.localPosition = Vec*/
        movePlayer(movement);
        playerAnim.SetBool("moving", Input.GetAxis("Vertical") > 0);
    }

    void movePlayer(Vector3 direction) {
        direction = playerRb.rotation * direction;
        playerRb.MovePosition(playerRb.position + direction * movementSpeed * Time.fixedDeltaTime);
    }
}
