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
    public static PirateShipManager instance;
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
        int pirateCount = Random.Range(1, 3); // Spawn one or two pirates
        for (int i = 0; i < pirateCount; i++) {
            // Need to make it so that the pirate spawns within range of the pirate ship
            float secondXAddon = 18.75f * 0.2f * Mathf.Sin(pirateShipAngle); // Replace the 0.2 with the pirate ship z scale 
            float secondZAddon = 18.75f * 0.2f * Mathf.Cos(pirateShipAngle);
            Vector2 secondAddon = new Vector2(secondXAddon, secondZAddon);
            float thirdXAddon = -18.75f * 0.2f * Mathf.Sin(pirateShipAngle);
            float thirdZAddon = -18.75f * 0.2f * Mathf.Cos(pirateShipAngle);
            Vector2 thirdAddon = new Vector2(thirdXAddon, thirdZAddon);
            float firstDistance = Vector2.Distance(normalisedPlayerPos, normalisedHitPos);
            float secondDistance = Vector2.Distance(normalisedPlayerPos, normalisedHitPos + secondAddon);
            float thirdDistance = Vector2.Distance(normalisedPlayerPos, normalisedHitPos + thirdAddon);
        }
        Dictionary<string, object> informationDictionary = new Dictionary<string, object>();
        pirateShipCoordinates.Add(pirateShipCoordinate);
        informationDictionary.Add("angle", pirateShipAngle);
        pirateShipInformation.Add(pirateShipCoordinate, informationDictionary);
        return true;
    }

    public void CreatePirateShip(Vector3 coordinate) {
        List<GameObject> componentsList = new List<GameObject>();
        float pirateShipAngle = (float) pirateShipInformation[coordinate]["angle"];
        GameObject pirateShipCreated = Instantiate(pirateShip, coordinate, Quaternion.Euler(0, pirateShipAngle, 0)); 
        componentsList.Add(pirateShipCreated);
        activePirateShips.Add(coordinate);
        activePirateShipInformation.Add(coordinate, componentsList);
        // Spawn the pirates now 
    }

    public void CreatePirateShipAndOcean(Vector3 coordinate) {
        CreatePirateShip(coordinate);
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
    }

    public void DeletePirateShip(Vector3 coordinate, List<GameObject> componentsList) {
        foreach (GameObject component in componentsList) {
            Destroy(component);
        }
        activePirateShips.Remove(coordinate);
        activePirateShipInformation.Remove(coordinate);
    }
}
