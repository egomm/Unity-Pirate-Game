using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PirateShipManager : MonoBehaviour {
    // Start is called before the first frame update
    public static List<Vector3> pirateShipCoordinates = new List<Vector3>();
    public static Dictionary<Vector3, Dictionary<string, object>> pirateShipInformation = new Dictionary<Vector3, Dictionary<string, object>>();
    public List<Vector3> activePirateShips = new List<Vector3>();
    public Dictionary<Vector3, List<GameObject>> activePirateShipInformation = new Dictionary<Vector3, List<GameObject>>();
    public GameObject pirateShip;
    public GameObject player;
    public GameObject ocean;
    public GameObject seaFloor;
    public GameObject boat;
    public static PirateShipManager instance;
    public static List<GameObject> activePirates = new List<GameObject>();
    // Pirates
    public GameObject[] pirates = new GameObject[4];
    public GameObject pirateCaptain;
    void Awake() {
        instance = this;
    }

    public bool InitalisePirateShip(float angle, float magnitude) {
        Vector3 pirateShipCoordinate = new Vector3(magnitude * Mathf.Cos(angle), 0, magnitude * Mathf.Sin(angle)); // y value will always be 0
        // Need to check if the pirate is in range is in range of the pirate ships/islands
        foreach (Vector3 pirateShip in pirateShipCoordinates) {
            if (Vector3.Distance(pirateShipCoordinate, pirateShip) < 20) { 
                return false; // Pirate ships must be at least 20 units apart 
            }
        }
        foreach (Vector3 islandCoordinate in IslandManager.islandCoordinates) {
            float islandRadius = (float) IslandManager.islandInformation[islandCoordinate]["radius"];
            if (Vector3.Distance(pirateShipCoordinate, islandCoordinate) < islandRadius + 15) { 
                return false; // Pirate ships must be at least 15 units from the circumfrence of any island
            }
        }
        float pirateShipAngle = Random.Range(0f, 360f); // Randomise the angle of the pirate ship
        float pirateShipAngleRadians = Mathf.PI * pirateShipAngle / 180f;
        // Initalise pirate 
        List<Vector3> pirateCoordinates = new List<Vector3>();
        List<Vector3> pirateAngles = new List<Vector3>();
        List<GameObject> piratesToSpawn = new List<GameObject>();
        List<float> piratesHealth = new List<float>();
        int pirateCount = 2; //Random.Range(1, 3); // Spawn one or two pirates
        for (int i = 0; i < pirateCount; i++) {
            // Make the values go between -18.75f and 18.75f at intervals of 4.6875f, so -18.75, -14.0625, 9.375, 4.6875, 0
            int firstRandom = Random.Range(-4, 0); // {-4, -3, -2, -1}
            int secondRandom = Random.Range(1, 5); // {1, 2, 3, 4}
            int randomForRandom = Random.Range(0, 2); // {0, 1}
            int randomCentreInt;
            if (randomForRandom == 0) {
                randomCentreInt = firstRandom;
            } else {
                randomCentreInt = secondRandom;
            }
            float randomCentreXAddon = 0;
            float randomCentreZAddon = 0;
            float randomCentreMultiplier = 4.6875f * 0.2f * randomCentreInt; 
            // Determine the centre based off rsin(theta) and rcos(theta) -> they are reversed due to the rotation of the pirate ship
            randomCentreXAddon = randomCentreMultiplier * Mathf.Sin(pirateShipAngleRadians);
            randomCentreZAddon = randomCentreMultiplier * Mathf.Cos(pirateShipAngleRadians);

            Vector3 pirateCoordinate = new Vector3(randomCentreXAddon, 1, randomCentreZAddon);
            bool validPirate = true;
            foreach (Vector3 pirateCoord in pirateCoordinates) {
                if (Vector3.Distance(pirateCoord, pirateCoordinate) < 1f) {
                    validPirate = false;
                    break;
                }
            }
            if (validPirate) {
                float pirateAngle = Random.Range(0f, 360f); // Random pirate angle in degrees
                GameObject pirateToSpawn;
                float pirateHealth = 5; 
                if (Random.Range(0, 100) >= 96) {
                    pirateToSpawn = pirateCaptain;
                    pirateHealth += 5;
                } else {
                    int pirateSpawnIndex = Random.Range(0, 4); // Pick an index from [0, 1, 2, 3]
                    pirateHealth += (pirateSpawnIndex + 1); 
                    pirateToSpawn = pirates[pirateSpawnIndex];
                }
                pirateCoordinates.Add(pirateCoordinate);
                pirateAngles.Add(new Vector3(0, pirateAngle, 0));
                piratesHealth.Add(pirateHealth);
                piratesToSpawn.Add(pirateToSpawn);
            } else {
                i--;
            }
        }
        Dictionary<string, object> informationDictionary = new Dictionary<string, object>();
        pirateShipCoordinates.Add(pirateShipCoordinate);
        // Add all the information to the information dictionary 
        informationDictionary.Add("angle", pirateShipAngle);
        informationDictionary.Add("piratecoordinates", pirateCoordinates);
        informationDictionary.Add("pirateangles", pirateAngles);
        informationDictionary.Add("piratestospawn", piratesToSpawn);
        informationDictionary.Add("pirateshealth", piratesHealth);
        // Add this information dictionary to the pirate ship information
        pirateShipInformation.Add(pirateShipCoordinate, informationDictionary);
        return true;
    }

    public void CreatePirateShip(Vector3 coordinate, bool spawnPirates) {
        // Method for creating a pirate ship given a coordinate and whether pirates should be spawned or not 
        List<GameObject> componentsList = new List<GameObject>();
        float pirateShipAngle = (float) pirateShipInformation[coordinate]["angle"];
        GameObject pirateShipCreated = Instantiate(pirateShip, coordinate, Quaternion.Euler(0, pirateShipAngle, 0)); 
        componentsList.Add(pirateShipCreated);
        activePirateShips.Add(coordinate);
        activePirateShipInformation.Add(coordinate, componentsList);
        // Spawn the pirates 
        if (spawnPirates) {
            activePirates.Clear();
            List<GameObject> pirates = (List<GameObject>) pirateShipInformation[coordinate]["piratestospawn"];
            List<Vector3> pirateCoordinates = (List<Vector3>) pirateShipInformation[coordinate]["piratecoordinates"];
            List<Vector3> pirateAngles = (List<Vector3>) pirateShipInformation[coordinate]["pirateangles"];
            List<float> piratesHealth = (List<float>) PirateShipManager.pirateShipInformation[coordinate]["pirateshealth"];
            float pirateShipAngleRadians = Mathf.PI * pirateShipAngle / 180f;
            for (int i = 0; i < pirates.Count; i++) {
                if (piratesHealth[i] > 0) {  // Cannot spawn a dead pirate
                    GameObject spawnedPirate = Instantiate(pirates[i], coordinate + pirateCoordinates[i], Quaternion.Euler(pirateAngles[i]));
                    activePirates.Add(spawnedPirate);
                }
            }
        }
    }

    public void CreatePirateShipAndOcean(Vector3 coordinate) {
        // Method for creating a pirate ship and a surrounding ocean given a coordinate
        CreatePirateShip(coordinate, true); 
        Instantiate(player, coordinate + new Vector3(0, 2, 0), Quaternion.identity);
        // Iterate between the bounds of -6, and 6 -> this determines the size of the ocean around the pirate ship
        int min = -6;
        int max = 6;
        for (int x = min; x <= max; x++) {
            for (int z = min; z <= max; z++) {
                Instantiate(ocean, new Vector3(10 * x, 0, 10 * z) + coordinate, Quaternion.identity);
                Instantiate(seaFloor, new Vector3(10 * x, -3, 10 * z) + coordinate, Quaternion.identity);
            }
        }
        // Need to spawn the player's boat next to the pirate ship 
        float boatAngleDegrees = (float) pirateShipInformation[coordinate]["angle"];
        float boatAngleRadians = boatAngleDegrees*Mathf.PI/180f;
        Vector3 boatCoordinate = PlayerManager.lastCoordinate;
        Vector3 boatRight = new Vector3(Mathf.Cos(boatAngleRadians), 0f, Mathf.Sin(boatAngleRadians));
        Vector3 boatForward = new Vector3(-Mathf.Sin(boatAngleRadians), 0f, Mathf.Cos(boatAngleRadians));
        Vector3 spawnPosition = boatRight * 10 * Mathf.Cos(boatAngleRadians) + boatForward * 10 * Mathf.Sin(boatAngleRadians);
        IslandManager.startingAngle = new Vector3(0, boatAngleDegrees, 0);
        // Instatantiate the boat, add 90 degrees to account for the boat's inherit rotation
        Instantiate(boat, boatCoordinate, Quaternion.Euler(0, boatAngleDegrees + 90, 0)); 
    }

    public void DeletePirateShip(Vector3 coordinate, List<GameObject> componentsList) {
        // Method for deleting a pirate ship given a coordinate and the pirate ship's component list
        foreach (GameObject component in componentsList) {
            // Iterate over all of the components and destory them
            Destroy(component);
        }
        activePirateShips.Remove(coordinate);
        activePirateShipInformation.Remove(coordinate);
    }
}
