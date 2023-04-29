using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastManager : MonoBehaviour {

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(1)) { // If right click is pressed
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Creates a ray from the from the camera through a screen point (in this case the mouse position)
            if (Physics.Raycast(ray, out hit)) { // using out because hit is for output only 
                Vector2 normalisedPlayerPos = new Vector2(rb.transform.position.x, rb.transform.position.z);
                Vector2 normalisedHitPos = new Vector2(hit.transform.position.x, hit.transform.position.z);
                string activeScene = SceneManager.GetActiveScene().name;
                float hitDistance = Vector2.Distance(normalisedHitPos, normalisedPlayerPos); // Get the distance between the hit objects position and the player position
                string hitTag = hit.transform.tag;
                if (hitDistance < 4 && hitTag == "Boat" && transform.position.y > hit.transform.position.y && activeScene != "Game") {
                    Debug.Log("Loading Game");
                    SceneManager.LoadScene("Game");
                    /*MeshRenderer renderer = hit.transform.GetComponent<MeshRenderer>();
                    Vector3 size = renderer.bounds.size;
                    Debug.Log(size);*/ // Use when more boats are added
                }
                if (hitTag == "Dock" && activeScene == "Game") {
                    // Check the distance now using 3 combined radii
                    // Need to find which coordinate this dock is connected to
                    Vector3 islandCoordinate = new Vector3(0, 0, 0);
                    Vector3 dockCoordinate = new Vector3(0, 0, 0);
                    bool hasDock = false;
                    foreach (var coordinate in IslandManager.islandCoordinates) {
                        Vector3 dockCoord = (Vector3) IslandManager.islandInformation[coordinate]["dockcoordinates"];
                        if ((coordinate + dockCoord) == hit.transform.position) {
                            dockCoordinate = dockCoord;
                            islandCoordinate = coordinate;
                            hasDock = true;
                            break;
                        }
                    }
                    if (hasDock) {
                        Debug.Log("Hit Dock!");
                        // At a scale of 1, the dock length is approx 13 
                        Vector3 dockCoordinates = islandCoordinate + (Vector3) IslandManager.islandInformation[islandCoordinate]["dockcoordinates"];
                        Vector3 dockScale = (Vector3) IslandManager.islandInformation[islandCoordinate]["dockscale"];
                        float dockAngle = (float) IslandManager.islandInformation[islandCoordinate]["dockangle"] * Mathf.PI / 180;
                        float secondXAddon = (13 / 6) * dockScale.x * Mathf.Sin(dockAngle - (Mathf.PI / 2));
                        float secondZAddon = (13 / 6) * dockScale.x * Mathf.Cos(dockAngle - (Mathf.PI / 2));
                        Vector2 secondAddon = new Vector2(secondXAddon, secondZAddon);
                        float secondHitDistance = Vector2.Distance(normalisedHitPos + secondAddon, normalisedPlayerPos);
                        float thirdXAddon = (13 / 3) * dockScale.x * Mathf.Sin(dockAngle - (Mathf.PI / 2));
                        float thirdZAddon = (13 / 3) * dockScale.x * Mathf.Cos(dockAngle - (Mathf.PI / 2));
                        Vector2 thirdAddon = new Vector2(thirdXAddon, thirdZAddon);
                        float thirdHitDistance = Vector2.Distance(normalisedHitPos + thirdAddon, normalisedPlayerPos);
                        Debug.Log("First distance: " + hitDistance);
                        Debug.Log(normalisedHitPos);
                        Debug.Log("Second distance: " + secondHitDistance);
                        Debug.Log(normalisedHitPos + secondAddon);
                        Debug.Log("Third distance: " + thirdHitDistance);
                        Debug.Log(normalisedHitPos + thirdAddon);
                        if (hitDistance < 4 || secondHitDistance < 4 || thirdHitDistance < 4) {
                            Debug.Log("Good");
                            // Load the island scene
                            IslandManager.currentCentre = islandCoordinate;
                            SceneManager.LoadScene("Island Scene");
                        }
                        /*MeshRenderer renderer = hit.transform.GetComponent<MeshRenderer>();
                        Vector3 size = renderer.bounds.size;
                        Vector3 extents = renderer.bounds.extents;
                        Vector3 centre = renderer.bounds.center;
                        Debug.Log(size);
                        Debug.Log(extents);
                        Debug.Log(centre);*/
                    } else {
                        Debug.Log("Dock could not be found");
                    }
                }
            }
        }
    }
}
