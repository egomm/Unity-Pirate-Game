using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
    private List<float> lastAttackTimes = new List<float>();
    private List<int> pirateTypes = new List<int>();
    public static double playerHealth = 100;
    public static Vector3 playerCoordinates = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start() {
        playerCoordinates = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate() {
        playerCoordinates = transform.position;
        if (SceneManager.GetActiveScene().name == "Island Scene") {
            Vector3 centre = IslandManager.currentCentre;
            float radius = (float) IslandManager.islandInformation[centre]["radius"];
            Vector2 centreTwoDimensional = new Vector2(centre.x, centre.z);
            float multiplier = 0.95f * radius;
            GameObject[] pirates = GameObject.FindGameObjectsWithTag("Pirate");
            for (int i = 0; i < pirates.Length; i++) { // This will not change until the scene unloads
                if (lastAttackTimes.Count < i + 1) { 
                    lastAttackTimes.Add(0);
                    string currentPirateName = pirates[i].name.Replace("(Clone)", "");
                    // Get the pirate type 
                    bool hasPirateType = false;
                    int pirateType = 0;
                    if (currentPirateName != IslandManager.instance.pirateCaptain.name) {
                        GameObject[] piratesArray = IslandManager.instance.pirates;
                        Debug.Log(piratesArray.Length);
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
                        Debug.Log("PIRATE TYPE: " + pirateType);
                        pirateTypes.Add(pirateType);
                    }
                }
                GameObject pirate = pirates[i];
                if (Vector3.Distance(pirate.transform.position, transform.position) < 10f) {
                    pirate.transform.position = Vector3.MoveTowards(pirate.transform.position, transform.position, 0.75f * Time.deltaTime); // make the speed depend on the pirate
                    if (Vector3.Distance(pirate.transform.position, transform.position) < 2f) { // can only attack once a second
                        // Can attack the player if the player is visible 
                        pirate.transform.LookAt(new Vector3(transform.position.x, pirate.transform.position.y, transform.position.z));
                        if ((Time.time - lastAttackTimes[i]) >= 1f) { 
                            lastAttackTimes[i] = Time.time; 
                            // Attack the player 
                            float pirateDamage = 1f + (0.1f * pirateTypes[i]); // between 0.5 and 1 for the damage
                            playerHealth -= pirateDamage;
                        }
                    }
                }
                if (playerHealth <= 0) { // If the player health is less than 0, player is dead
                    Debug.Log("PLAYER DEAD");
                }
            }
        }
    }
}
