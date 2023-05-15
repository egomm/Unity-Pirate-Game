using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    // Update is called once per frame - Late Update is called after update is called
    void LateUpdate() {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0) {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            // Make sure that the camera is 10 units above the player (in the y direction)
            transform.position = player.transform.position + new Vector3(0, 10, 0);
            // Set the camera angle to be the same as the player angle
            transform.eulerAngles = new Vector3(90, player.transform.rotation.eulerAngles.y, 0);
        }
    }
}
