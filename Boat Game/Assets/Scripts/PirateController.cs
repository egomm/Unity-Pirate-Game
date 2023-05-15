using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PirateController : MonoBehaviour {
    Animator pirateAnim;

    void Start() {
        pirateAnim = GetComponent<Animator>();
    }

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
                // Get the new player positions based on trigonometry (rcos(theta) and rsin(theta))
                float newPirateX = multiplier * Mathf.Cos(angle);
                float newPirateZ = multiplier * Mathf.Sin(angle);
                // Set the pirate's position to at the edge of the radius (prevents the pirate from going out of the island radius)
                transform.position = new Vector3(newPirateX + centre.x, transform.position.y, newPirateZ + centre.z);
            }
        }
    }
}
