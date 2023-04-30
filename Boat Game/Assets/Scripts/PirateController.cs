using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateController : MonoBehaviour {
    private bool playerInitalised = false;
    GameObject player;

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Pirate Spawned");
    }

    // Update is called once per frame
    void Update() {
        if (!playerInitalised) {
            player = MovePlayer.player;
            playerInitalised = true;
        }
    }

    void FixedUpdate() {
        if (player != null) {
            //checkDistance();
            //Debug.Log("DISTANCE: " + Vector3.Distance(player.transform.position, transform.position));
            //if (Vector3.Distance(player.transform.position, transform.position) < 10f) {
                //Debug.Log("DISTANCE: " + Vector3.Distance(player.transform.position, transform.position));
                //RaycastHit hit;
                //if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, 10)) {
                    
                //}
            //}
        }
    }

    void checkDistance() {
        //Debug.Log("DISTANCE: " + Vector3.Distance(player.transform.position, transform.position));
    }
}
