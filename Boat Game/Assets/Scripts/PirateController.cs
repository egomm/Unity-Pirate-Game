using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PirateController : MonoBehaviour {
    //private bool playerInitalised = false;
    //GameObject player;

    // Start is called before the first frame update
    /*void Start() {
        Debug.Log("Pirate Spawned");
    }*/

    // Update is called once per frame
    /*void Update() {
        if (!playerInitalised) {
            player = MovePlayer.player;
            playerInitalised = true;
        }
    }*/

    void FixedUpdate() {
        //if (player != null) {
        //checkDistance();
        //Debug.Log("DISTANCE: " + Vector3.Distance(player.transform.position, transform.position));
        //if (Vector3.Distance(player.transform.position, transform.position) < 10f) {
        //Debug.Log("DISTANCE: " + Vector3.Distance(player.transform.position, transform.position));
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, 10)) {

        //}
        //}
        //}
        // Check if the pirate is out of the island radius 
        if (SceneManager.GetActiveScene().name == "Island Scene") {
            Vector3 centre = IslandManager.currentCentre;
            float radius = (float) IslandManager.islandInformation[centre]["radius"];
            Vector2 centreTwoDimensional = new Vector2(centre.x, centre.z);
            float multiplier = 0.95f * radius;
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), centreTwoDimensional) > multiplier) {
                float angle = 0;    
                if ((transform.position.x - centre.x) != 0) { // To avoid dividing my 0 
                    angle = Mathf.Atan((transform.position.z - centre.z) / (transform.position.x - centre.x));
                    multiplier *= (transform.position.x - centre.x) / Mathf.Abs(transform.position.x - centre.x);
                }
                // Get the new player positions based on trig
                float newPirateX = multiplier * Mathf.Cos(angle);
                float newPirateZ = multiplier * Mathf.Sin(angle);
                transform.position = new Vector3(newPirateX + centre.x, transform.position.y, newPirateZ + centre.z);
            }
        }
    }
}
