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
    public static PirateShipManager instance;
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
        Dictionary<string, object> informationDictionary = new Dictionary<string, object>();
        float pirateShipAngle = Random.Range(0f, 360f);
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
    }

    public void CreatePirateShipAndOcean(Vector3 coordinate) {

    }

    public void DeletePirateShip(Vector3 coordinate, List<GameObject> componentsList) {
        foreach (GameObject component in componentsList) {
            Destroy(component);
        }
        activePirateShips.Remove(coordinate);
        activePirateShipInformation.Remove(coordinate);
    }
}
