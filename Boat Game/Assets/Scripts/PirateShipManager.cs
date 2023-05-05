using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PirateShipManager : MonoBehaviour {
    // Start is called before the first frame update
    public static List<Vector3> pirateShipCoordinates = new List<Vector3>();
    public GameObject pirateShip;
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool initalisePirateShip(float angle, float magnitude) {
        Vector3 pirateShipCoordinate = new Vector3(magnitude * Mathf.Cos(angle), 0, magnitude * Mathf.Sin(angle)); // y value will always be 0
        // Need to check if the pirate is in range is in range of the pirate ships/islands
        return false;
    }
}
