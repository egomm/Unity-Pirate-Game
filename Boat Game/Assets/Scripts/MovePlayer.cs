using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private float speed = 5;
    private Rigidbody playerRb;
    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 Vec = transform.localPosition;  
        Vec.x += Input.GetAxis("Horizontal") * Time.deltaTime * speed;  
        Vec.z += Input.GetAxis("Vertical") * Time.deltaTime * speed;  
        transform.localPosition = Vec;  
    }
}
