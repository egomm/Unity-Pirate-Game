using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSceneGameManager : MonoBehaviour {
    public GameObject player;
    public GameObject ocean;
    public GameObject seaFloor;
    public GameObject boat;

    // Start is called before the first frame update
    void Start() { 
        List<Vector3> islandCoordinates = IslandManager.islandCoordinates; // static
        Dictionary<Vector3, Dictionary<string, object>> islandInformation = IslandManager.islandInformation; // static
        islandCoordinates.Clear();
        islandInformation.Clear();
        Debug.Log("STARTING SCENE"); 
        Debug.Log(islandCoordinates.Count);
        Debug.Log(islandInformation.Count);
        float angle = Random.Range(0, 2*Mathf.PI); // Angle is based on from (0, 0) positive x direction 
        float magnitude = Random.Range(150f, 250f); // As this is the centre island, keep it close to the centre
        Debug.Log(IslandManager.instance);
        IslandManager.instance.InitaliseIsland(angle, magnitude, false, false); // keep attempting to initalise island
        Vector3 coordinate = IslandManager.islandCoordinates[0];
        IslandManager.currentCentre = coordinate;
        if (IslandManager.islandInformation.ContainsKey(coordinate)) {
            IslandManager.instance.CreateIslandAndOcean(coordinate);
        } else {
            Debug.Log("Key not found!");
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
