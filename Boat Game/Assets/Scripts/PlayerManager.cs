using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour { 
    private List<float> lastAttackTimes = new List<float>();
    private List<int> pirateTypes = new List<int>();
    public static Vector3 lastCoordinate = new Vector3(0, 0, 0);
    public static double playerHealth = 100;
    public static Vector3 playerCoordinates = new Vector3(0, 0, 0);
    private float lastRegenerationTime = 0;
    private float lastDrowingTime = 0;
    private bool inWater = false;

    public static int goldLooted = 0;
    public static int piratesKilled = 0;

    // Start is called before the first frame update
    void Start() {
        playerCoordinates = transform.position;
        lastDrowingTime = Time.time;
        lastRegenerationTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate() {
        string activeSceneName = SceneManager.GetActiveScene().name;
        playerCoordinates = transform.position;
        // Will need to upgrade this for pirate ships

        // Health regeneration system - 1hp every 5 seconds 
        // Only allow for the player to regenerate if they aren't on the dock
        if (activeSceneName == "Island Scene") {
            Vector3 islandCentre = IslandManager.currentCentre;
            float islandRadius = (float) IslandManager.islandInformation[islandCentre]["radius"];
            float centreTwoDimensionalDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(islandCentre.x, islandCentre.z));
            if (centreTwoDimensionalDistance > islandRadius) { // Can't regenerate health because the player isn't in range
                lastRegenerationTime = Time.time;
            }
        }

        if (activeSceneName == "Game") {
            lastCoordinate = transform.position;
            IslandManager.startingCoordinates = lastCoordinate;
        }
        
        // Make it so that the player can't regenerate health if they are in the ocean -> make sure that the player drowns instead
        if (activeSceneName == "Island Scene" || activeSceneName == "Starting Scene" || activeSceneName == "Pirate Ship") {
            if (playerCoordinates.y < 0 && inWater) { // Player is underwater 
                lastRegenerationTime = Time.time; // Can't regenerate
                if ((Time.time - lastDrowingTime) > 0.5f) { // Decrease the player health by for every 0.5 seconds the player is in water
                    playerHealth--;
                    lastDrowingTime = Time.time;
                }
            } else if (playerCoordinates.y < 0) { // Just entered water
                inWater = true;
                lastDrowingTime = Time.time;
                lastRegenerationTime = Time.time; // Can't regenerate
            } else { // Player isn't in water
                inWater = false;
            }
        }

        if ((Time.time-lastRegenerationTime) > 2f) { 
            if (playerHealth < 99) { 
                playerHealth++;
            } else {
                playerHealth = 100; 
            }
            lastRegenerationTime = Time.time;
        }

        if (transform.position.y > 4 && activeSceneName == "Game") { // Prevent the player from flying, sometimes the player was able to glitch up box colliders
            transform.position = new Vector3(transform.position.x, WaveManager.instance.GetWaveHeight(transform.position.z), transform.position.z);
        }

        if (activeSceneName == "Island Scene" || activeSceneName == "Pirate Ship") { 
            GameObject[] pirates = GameObject.FindGameObjectsWithTag("Pirate");
            for (int i = 0; i < pirates.Length; i++) { // This will not change until the scene unloads
                if (lastAttackTimes.Count < i + 1) { 
                    lastAttackTimes.Add(0);
                    string currentPirateName = pirates[i].name.Replace("(Clone)", "");
                    int pirateType = 0;
                    if (currentPirateName != IslandManager.instance.pirateCaptain.name) {
                        GameObject[] piratesArray = IslandManager.instance.pirates;
                        for (int j = 0; j < piratesArray.Length; j++) {
                            if (currentPirateName == piratesArray[j].name) {
                                pirateType = j + 1;
                                break;
                            }
                        }
                    } else {
                        pirateType = 5;
                    }
                    if (pirateType > 0) { // It will always add 
                        pirateTypes.Add(pirateType);
                    }
                }
                GameObject pirate = pirates[i];
                if (activeSceneName == "Island Scene") {
                    if (Vector3.Distance(pirate.transform.position, transform.position) < 10f) {
                        pirate.transform.position = Vector3.MoveTowards(pirate.transform.position, transform.position, 0.75f * Time.deltaTime); // make the speed depend on the pirate
                        if (Vector3.Distance(pirate.transform.position, transform.position) < 2f) { // can only attack if within 2 units of the player
                            // Can attack the player if the player is visible 
                            pirate.transform.LookAt(new Vector3(transform.position.x, pirate.transform.position.y, transform.position.z));
                            if ((Time.time - lastAttackTimes[i]) >= 1f) { // can only attack once a second
                                lastAttackTimes[i] = Time.time; 
                                // Attack the player 
                                float pirateDamage = 3f + (0.6f * pirateTypes[i]); // between 3 and 6 for the damage
                                playerHealth -= pirateDamage;
                            }
                        }
                    }
                } else if (activeSceneName == "Pirate Ship") {
                    if (Vector3.Distance(pirate.transform.position, transform.position) < 20f && transform.position.y > 0) {
                        pirate.transform.position = Vector3.MoveTowards(pirate.transform.position, transform.position, 0.75f * Time.deltaTime); // make the speed depend on the pirate
                        if (Vector3.Distance(pirate.transform.position, transform.position) < 2f) { // can only attack if within 2 units of the player
                            // Can attack the player if the player is visible 
                            pirate.transform.LookAt(new Vector3(transform.position.x, pirate.transform.position.y, transform.position.z));
                            if ((Time.time - lastAttackTimes[i]) >= 1f) { // can only attack once a second
                                lastAttackTimes[i] = Time.time; 
                                // Attack the player 
                                float pirateDamage = 3f + (0.6f * pirateTypes[i]); // between 3 and 6 for the damage
                                playerHealth -= pirateDamage;
                            }
                        }
                    }
                }
            }
        }
        if (playerHealth <= 0) { // If the player health is less than 0, player is dead
            SceneManager.LoadScene("Death Scene");
        }
    }
}
