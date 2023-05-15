using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastManager : MonoBehaviour {

    private Rigidbody rb;
    private float lastAttackTime = 0;
    // Start is called before the first frame update
    void Start() {
        lastAttackTime = Time.time;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        string activeScene = SceneManager.GetActiveScene().name;
        Vector3 islandCoordinates = IslandManager.currentCentre;
        Vector3 pirateShipCoordinates = PirateSceneManager.currentPirateShipCoordinates;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Creates a ray from the from the camera through a screen point (in this case the mouse position)
        if (Input.GetMouseButtonDown(0)) { // If left click is pressed
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag == "Pirate") { 
                    if (Vector3.Distance(hit.transform.position, transform.position) < 2.5f) { // Player is within 2 1/2 of the pirate
                        if ((Time.time - lastAttackTime) >= 0.25f) { // The player can only attack once every 250ms
                            lastAttackTime = Time.time;
                            List<GameObject> pirates = new List<GameObject>();
                            List<float> piratesHealth = new List<float>();
                            if (activeScene == "Island Scene") { // Manage the pirates in the Island Scene
                                pirates = IslandManager.activePirates;
                                piratesHealth = (List<float>) IslandManager.islandInformation[islandCoordinates]["pirateshealth"];
                            } else if (activeScene == "Pirate Ship") { // Manage the pirates in the Pirate Ship scene
                                pirates = PirateShipManager.activePirates;
                                piratesHealth = (List<float>) PirateShipManager.pirateShipInformation[pirateShipCoordinates]["pirateshealth"];
                            }
                            // Find the pirate which the player just clicked on 
                            for (int i = 0; i < pirates.Count; i++) { 
                                if (pirates[i] == hit.transform.gameObject) {
                                    // Attack the pirate
                                    piratesHealth[i] -= (2+PlayerManager.piratesKilled*0.05f); // Decrease the pirate's health by 2 each time the player clicks the pirate, increases by 1 per every 20 pirates the player has killed
                                    if (piratesHealth[i] <= 0) {
                                        PlayerManager.piratesKilled++;
                                        if (activeScene == "Pirate Ship") {
                                            GameObject[] activePirates = GameObject.FindGameObjectsWithTag("Pirate");
                                            if (activePirates.Length <= 1) {
                                                PlayerManager.goldLooted += 250 + (int) Mathf.Pow(Vector3.Distance(pirateShipCoordinates, new Vector3(0, 0, 0)), 1.15f); // dependent on constant + distance from centre^1.15
                                            }
                                        }
                                        Destroy(pirates[i]); // Destory the pirate if the pirate is less than or equal to 0 
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) { // If right click is pressed
            if (Physics.Raycast(ray, out hit)) { // using out because hit is for output only 
                Vector2 normalisedPlayerPos = new Vector2(rb.transform.position.x, rb.transform.position.z);
                Vector2 normalisedHitPos = new Vector2(hit.transform.position.x, hit.transform.position.z);
                float hitDistance = Vector2.Distance(normalisedHitPos, normalisedPlayerPos); // Get the distance between the hit objects position and the player position
                string hitTag = hit.transform.tag;
                if (hitDistance < 4 && hitTag == "Boat" && transform.position.y > hit.transform.position.y && activeScene != "Game") {
                    SceneManager.LoadSceneAsync("Game");
                }
                if (hitDistance < 4 && hitTag == "Chest" && activeScene == "Island Scene") {
                    // Find the specific chest 
                    Vector3 islandCoordinate = IslandManager.currentCentre;
                    List<Vector3> chestCoordinates = (List<Vector3>) IslandManager.islandInformation[islandCoordinate]["chestcoordinates"];
                    List<int> chestGold = (List<int>) IslandManager.islandInformation[islandCoordinate]["chestgold"];
                    List<int> maxChestGold = (List<int>) IslandManager.islandInformation[islandCoordinate]["maxchestgold"];
                    float shortestDistance = 1000;
                    int index = 0;
                    if (chestCoordinates.Count > 0) {
                        for (int i = 0; i < chestCoordinates.Count; i++) { // Do it this way as there are different parts of the chest (therefore small differences which arent 0 units)
                            Vector3 actualChestCoordinate = islandCoordinate + chestCoordinates[i];
                            float chestDistance = Vector3.Distance(actualChestCoordinate, hit.transform.position);
                            if (chestDistance < shortestDistance) {
                                shortestDistance = chestDistance;
                                index = i;
                            }
                        }
                        int goldToSubtract = (int) (0.1f * maxChestGold[index]);
                        if (chestGold[index] > goldToSubtract) {
                            PlayerManager.goldLooted += goldToSubtract;
                            chestGold[index] -= goldToSubtract; // Take 10% of the maximum gold which the chest can store
                        } else if (chestGold[index] > 0) {
                            PlayerManager.goldLooted += chestGold[index];
                            chestGold[index] = 0;
                        }
                        IslandManager.islandInformation[islandCoordinate]["chestgold"] = chestGold;
                    }
                }
                if (hitTag == "Pirate Ship" && activeScene == "Game") {
                    // If the user right clicked on the pirate ship
                    bool hasPirateShip = false;
                    foreach (Vector3 coordinate in PirateShipManager.pirateShipCoordinates) {
                        if (coordinate == hit.transform.position) {
                            hasPirateShip = true;
                            break;
                        }
                    }
                    if (hasPirateShip) {
                        // Create circles around points on the pirate ship to determine if the player is within range of the pirate ship
                        float pirateShipAngle = Mathf.PI * (float) PirateShipManager.pirateShipInformation[hit.transform.position]["angle"]/180;
                        // 18.75 is an approximate for a portion of the dock at full scale (multiply by 0.2 to account for the actual scale)
                        float secondXAddon = 18.75f * 0.2f * Mathf.Sin(pirateShipAngle); 
                        float secondZAddon = 18.75f * 0.2f * Mathf.Cos(pirateShipAngle);
                        Vector2 secondAddon = new Vector2(secondXAddon, secondZAddon);
                        float thirdXAddon = -18.75f * 0.2f * Mathf.Sin(pirateShipAngle);
                        float thirdZAddon = -18.75f * 0.2f * Mathf.Cos(pirateShipAngle);
                        Vector2 thirdAddon = new Vector2(thirdXAddon, thirdZAddon);
                        float firstDistance = Vector2.Distance(normalisedPlayerPos, normalisedHitPos);
                        float secondDistance = Vector2.Distance(normalisedPlayerPos, normalisedHitPos + secondAddon);
                        float thirdDistance = Vector2.Distance(normalisedPlayerPos, normalisedHitPos + thirdAddon);
                        if (firstDistance < 5 || secondDistance < 5 || thirdDistance < 5) {
                            PirateSceneManager.currentPirateShipCoordinates = hit.transform.position;
                            SceneManager.LoadScene("Pirate Ship");
                        }
                    }
                }
                if (hitTag == "Dock" && activeScene == "Game") {
                    // Check the distance now using 3 combined radii
                    // Find which coordinate this dock is connected to
                    Vector3 islandCoordinate = new Vector3(0, 0, 0);
                    Vector3 dockCoordinate = new Vector3(0, 0, 0);
                    bool hasDock = false;
                    foreach (Vector3 coordinate in IslandManager.islandCoordinates) {
                        Vector3 dockCoord = (Vector3) IslandManager.islandInformation[coordinate]["dockcoordinates"];
                        if ((coordinate + dockCoord) == hit.transform.position) {
                            dockCoordinate = dockCoord;
                            islandCoordinate = coordinate;
                            hasDock = true;
                            break;
                        }
                    }
                    if (hasDock) {
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
                        if (hitDistance < 4 || secondHitDistance < 4 || thirdHitDistance < 4) {
                            // Load the island scene
                            IslandManager.currentCentre = islandCoordinate;
                            SceneManager.LoadScene("Island Scene");
                        }
                    }
                }
            }
        }
    }
}
