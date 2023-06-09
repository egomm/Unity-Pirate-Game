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
        // Set the ocean prefab amplitude and speed to 0
        ocean.GetComponent<WaveManager>().amplitude = 0;
        ocean.GetComponent<WaveManager>().speed = 0;
        List<Vector3> islandCoordinates = IslandManager.islandCoordinates; // static
        List<GameObject> activePirates = IslandManager.activePirates;
        Dictionary<Vector3, Dictionary<string, object>> islandInformation = IslandManager.islandInformation; // static
        islandCoordinates.Clear();
        islandInformation.Clear();
        activePirates.Clear();
        float angle = Random.Range(0, 2*Mathf.PI); // Angle is based on from (0, 0) positive x direction 
        float magnitude = Random.Range(150f, 250f); // As this is the starting island, make sure the player spawns close to the centre, although not directly in the centre
        IslandManager.instance.InitaliseIsland(angle, magnitude, false, false); // initalise island
        Vector3 coordinate = IslandManager.islandCoordinates[0];
        IslandManager.currentCentre = coordinate;
        if (IslandManager.islandInformation.ContainsKey(coordinate)) {
            // Create the island and a surrounding ocean if the island manager contains this coordinate as a key
            IslandManager.instance.CreateIslandAndOcean(coordinate);
        }
    }
}
