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

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    public bool InitalisePirateShip(float angle, float magnitude) {
        Vector3 pirateShipCoordinate = new Vector3(magnitude * Mathf.Cos(angle), 0, magnitude * Mathf.Sin(angle)); // y value will always be 0
        // Need to check if the pirate is in range is in range of the pirate ships/islands
        foreach (Vector3 pirateShip in pirateShipCoordinates) {
            if (Vector3.Distance(pirateShipCoordinate, pirateShip) < 15) { 
                return false; // Pirate ships must be at least 15 units apart 
            }
        }
        foreach (Vector3 islandCoordinate in IslandManager.islandCoordinates) {
            float islandRadius = (float) IslandManager.islandInformation[islandCoordinate]["radius"];
            if (Vector3.Distance(pirateShipCoordinate, islandCoordinate) < islandRadius + 15) { 
                return false; // Pirate ships must be at least 15 units from circumfrence of island
            }
        }
        float pirateShipAngle = Random.Range(0f, 360f);
        float pirateShipAngleRadians = Mathf.PI * pirateShipAngle / 180f;
        // Initalise pirate
        List<Vector3> pirateCoordinates = new List<Vector3>();
        List<Vector3> pirateAngles = new List<Vector3>();
        List<GameObject> piratesToSpawn = new List<GameObject>();
        List<float> piratesHealth = new List<float>();
        int pirateCount = 2; //Random.Range(1, 3); // Spawn one or two pirates
        for (int i = 0; i < pirateCount; i++) {
            // Make the values go between -18.75f and 18.75f at intervals of 4.6875f, so -18.75, -14.0625, 9.375, 4.6875, 0
            int randomCentreInt = Random.Range(-4, 5); 
            float randomCentreXAddon = 0;
            float randomCentreZAddon = 0;
            float randomCentreMultiplier = 4.6875f * 0.2f * randomCentreInt; 
            if (pirateShipCoordinate.z > 0) {
                randomCentreXAddon = randomCentreMultiplier * Mathf.Sin(pirateShipAngleRadians) * pirateShipCoordinate.x / Mathf.Abs(pirateShipCoordinate.x);
                randomCentreZAddon = randomCentreMultiplier * Mathf.Cos(pirateShipAngleRadians) * pirateShipCoordinate.z / Mathf.Abs(pirateShipCoordinate.z);
            } else {
                randomCentreXAddon = randomCentreMultiplier * Mathf.Sin(pirateShipAngleRadians) * -pirateShipCoordinate.x / Mathf.Abs(pirateShipCoordinate.x);
                randomCentreZAddon = randomCentreMultiplier * Mathf.Cos(pirateShipAngleRadians) * -pirateShipCoordinate.z / Mathf.Abs(pirateShipCoordinate.z);
            }
            Vector3 pirateCoordinate = new Vector3(randomCentreXAddon, 2, randomCentreZAddon);
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

        informationDictionary.Add("angle", pirateShipAngle);
        informationDictionary.Add("piratecoordinates", pirateCoordinates);
        informationDictionary.Add("pirateangles", pirateAngles);
        informationDictionary.Add("piratestospawn", piratesToSpawn);
        informationDictionary.Add("pirateshealth", piratesHealth);
        pirateShipInformation.Add(pirateShipCoordinate, informationDictionary);
        return true;
    }

    public void CreatePirateShip(Vector3 coordinate, bool spawnPirates) {
        List<GameObject> componentsList = new List<GameObject>();
        float pirateShipAngle = (float) pirateShipInformation[coordinate]["angle"];
        GameObject pirateShipCreated = Instantiate(pirateShip, coordinate, Quaternion.Euler(0, pirateShipAngle, 0)); 
        componentsList.Add(pirateShipCreated);
        activePirateShips.Add(coordinate);
        activePirateShipInformation.Add(coordinate, componentsList);
        // Spawn the pirates now 
        if (spawnPirates) {
            List<GameObject> pirates = (List<GameObject>) pirateShipInformation[coordinate]["piratestospawn"];
            List<Vector3> pirateCoordinates = (List<Vector3>) pirateShipInformation[coordinate]["piratecoordinates"];
            List<Vector3> pirateAngles = (List<Vector3>) pirateShipInformation[coordinate]["pirateangles"];
            float pirateShipAngleRadians = Mathf.PI * pirateShipAngle / 180f;
            Debug.Log("ANGLE: " + pirateShipAngleRadians);
            Debug.Log("COS: " + Mathf.Cos(pirateShipAngleRadians));
            Debug.Log("SIN: " + Mathf.Sin(pirateShipAngleRadians));
            for (int i = 0; i < pirates.Count; i++) {
                GameObject spawnedPirate = Instantiate(pirates[i], coordinate + pirateCoordinates[i], Quaternion.Euler(pirateAngles[i]));
                activePirates.Add(spawnedPirate);
                Debug.Log(pirateCoordinates[i]);
                Debug.Log("SPAWNED PIRATE " + (coordinate + pirateCoordinates[i]));
            }
        }
    }

    public void CreatePirateShipAndOcean(Vector3 coordinate) {
        CreatePirateShip(coordinate, true);
        Instantiate(player, coordinate + new Vector3(0, 2, 0), Quaternion.identity);
        int min = -6;
        int max = 6;
        for (int x = min; x <= max; x++) {
            for (int z = min; z <= max; z++) {
                Instantiate(ocean, new Vector3(10 * x, 0, 10 * z) + coordinate, Quaternion.identity);
                Instantiate(seaFloor, new Vector3(10 * x, -3, 10 * z) + coordinate, Quaternion.identity);
                //Debug.Log(new Vector3(10 * x, 0, 10 * z) + coordinate);
            }
        }
        // Need to spawn the player's boat next to the pirate ship 
        float boatAngleDegrees = (float) pirateShipInformation[coordinate]["angle"];
        float boatAngleRadians = boatAngleDegrees*Mathf.PI/180f;
        Debug.Log("ANGLE!: " + boatAngleDegrees);
        float xAddon = 6;
        float zAddon = 6;
        if (coordinate.x * coordinate.z >= 0) {
            xAddon = 6 * Mathf.Cos(boatAngleRadians - (Mathf.PI/2)); // Adjust this by the pirate ship scale
            zAddon = 6 * Mathf.Sin(boatAngleRadians - (Mathf.PI/2));
        } else {
            xAddon = -6 * Mathf.Cos(boatAngleRadians) * -coordinate.x/Mathf.Abs(coordinate.x); // Adjust this by the pirate ship scale
            zAddon = -6 * Mathf.Sin(boatAngleRadians) * -coordinate.z/Mathf.Abs(coordinate.z);
        }
        xAddon = 6 * Mathf.Cos(boatAngleRadians - (Mathf.PI/2)); // Adjust this by the pirate ship scale
        zAddon = 6 * Mathf.Sin(boatAngleRadians - (Mathf.PI/2));
        Debug.Log(boatAngleRadians);
        if (boatAngleRadians >= 0 && boatAngleRadians <= Mathf.PI/2) {
            Debug.Log("FIRST");
            xAddon = -Mathf.Abs(4 * Mathf.Cos(Mathf.PI/2 - boatAngleRadians));
            zAddon = Mathf.Abs(4 * Mathf.Sin(Mathf.PI/2 - boatAngleRadians));
        } else if (boatAngleRadians > Mathf.PI/2 && boatAngleRadians <= Mathf.PI) {
            Debug.Log("SECOND");
            xAddon = -Mathf.Abs(4 * Mathf.Cos(Mathf.PI/2 - (boatAngleRadians % Mathf.PI/2)));
            zAddon = -Mathf.Abs(4 * Mathf.Sin(Mathf.PI/2 - (boatAngleRadians % Mathf.PI/2)));
        } else if (boatAngleRadians > Mathf.PI && boatAngleRadians <= 3*Mathf.PI/2) {
            Debug.Log("THIRD");
            xAddon = Mathf.Abs(4 * Mathf.Cos((boatAngleRadians % Mathf.PI/2)));
            zAddon = Mathf.Abs(4 * Mathf.Sin((boatAngleRadians % Mathf.PI/2)));
        } else if (boatAngleRadians > 3*Mathf.PI/2 && boatAngleRadians <= 2*Mathf.PI) {
            Debug.Log("FOURTH");
            xAddon = Mathf.Abs(4 * Mathf.Cos(Mathf.PI/2 - (boatAngleRadians % Mathf.PI/2)));
            zAddon = Mathf.Abs(4 * Mathf.Sin(Mathf.PI/2 - (boatAngleRadians % Mathf.PI/2)));
        }
        Vector3 boatRight = new Vector3(Mathf.Cos(boatAngleRadians), 0f, Mathf.Sin(boatAngleRadians));
        Vector3 boatForward = new Vector3(-Mathf.Sin(boatAngleRadians), 0f, Mathf.Cos(boatAngleRadians));
        Vector3 spawnPosition = boatRight * 10 * Mathf.Cos(boatAngleRadians) + boatForward * 10 * Mathf.Sin(boatAngleRadians);
        Debug.Log("RIGHT: " + boatRight);
        Debug.Log("TEST: " + spawnPosition);
        Debug.Log("SIN!: " +  Mathf.Sin(boatAngleRadians));
        Debug.Log("COS!: " + Mathf.Cos(boatAngleRadians));
        Vector3 addon = new Vector3(xAddon, 0, zAddon); // Add this onto the pirate ship 
        Debug.Log("ADDON: " + addon);
        Vector3 boatCoordinate = addon + coordinate;
        Instantiate(boat, boatCoordinate, Quaternion.Euler(0, boatAngleDegrees + 90, 0));

    }

    public void DeletePirateShip(Vector3 coordinate, List<GameObject> componentsList) {
        foreach (GameObject component in componentsList) {
            Destroy(component);
        }
        activePirateShips.Remove(coordinate);
        activePirateShipInformation.Remove(coordinate);
    }
}
