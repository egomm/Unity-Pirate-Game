using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PirateController : MonoBehaviour {

    void FixedUpdate() {
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
