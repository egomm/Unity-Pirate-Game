using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PirateShipManager : MonoBehaviour {
    // Start is called before the first frame update
    public static List<Vector3> pirateShipCoordinates = new List<Vector3>();
    public List<Vector3> activePirateShips = new List<Vector3>();
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
        pirateShipCoordinates.Add(pirateShipCoordinate);
        return true;
    }

    public void CreatePirateShip(Vector3 coordinate) {
        Instantiate(pirateShip, coordinate, Quaternion.identity); // customise the angle?
        activePirateShips.Add(coordinate);
    }
}
