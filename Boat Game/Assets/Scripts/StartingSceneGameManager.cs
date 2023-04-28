using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSceneGameManager : MonoBehaviour {
    public GameObject player;
    public GameObject ocean;
    public GameObject seaFloor;
    public GameObject boat;

    // Start is called before the first frame update
    void Start() { // WORK ON THIS!
        //islandManager = GetComponent<IslandManager>();
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
        IslandManager.instance.InitaliseIsland(angle, magnitude, false); // keep attempting to initalise island
        Vector3 coordinate = IslandManager.islandCoordinates[0];
        Debug.Log(coordinate.x + ", " + coordinate.y + ", " + coordinate.z);
        IslandManager.instance.CreateIsland(coordinate);
        IslandManager.currentCentre = coordinate;
        // Instantiate the ocean and the sea floor in a radius 
        float radius = (float) IslandManager.islandInformation[coordinate]["radius"];
        // Spawn prefabs within 3x the radius of the island, and disallow the player out of 2.5x the radius of the island
        // Ocean prefab is 10x10
        int min = (int) Mathf.Round(radius/10)*-3; 
        int max = (int) Mathf.Round(radius/10)*3;
        Debug.Log(min);
        Debug.Log(max);
        for (int x = min; x <= max; x++) {
            for (int z = min; z <= max; z++) {
                Instantiate(ocean, new Vector3(10*x, 0, 10*z) + coordinate, Quaternion.identity);
                Instantiate(seaFloor, new Vector3(10 * x, -3, 10 * z) + coordinate, Quaternion.identity);
                Debug.Log(new Vector3(10 * x, 0, 10 * z) + coordinate);
            }
        }
        //Instantiate(island, new Vector3(250, 0, 250), Quaternion.identity);
        Vector3 dockCoordinates = coordinate + (Vector3) IslandManager.islandInformation[coordinate]["dockcoordinates"];
        Vector3 dockScale = (Vector3) IslandManager.islandInformation[coordinate]["dockscale"];
        float dockAngle = (float) IslandManager.islandInformation[coordinate]["dockangle"]*Mathf.PI/180;
        float dockWidthZ = 5f * dockScale.z; // Adjust this?
        // Change needed for x coordinates from dock position !NEED TO FIX THIS!
        // At a scale of 1, the dock length is approx 13 
        float xAddon = (65/12) * dockScale.x * Mathf.Sin(dockAngle-(Mathf.PI/2));
        float zAddon = (65/12) * dockScale.x * Mathf.Cos(dockAngle-(Mathf.PI/2));
        Vector3 addon = new Vector3(xAddon, 0, zAddon);
        float xChange = dockWidthZ*Mathf.Sin(dockAngle-Mathf.PI);
        float zChange = dockWidthZ*Mathf.Cos(dockAngle-Mathf.PI);
        Vector3 change = new Vector3(xChange, 0, zChange);
        Vector3 newDockCoordinates = addon+dockCoordinates+change;
        Vector3 finalDockCoordinates = new Vector3(newDockCoordinates.x, 0, newDockCoordinates.z);
        // Need to spawn the boat -> width is ~5.5 when the island is at a z scale of 1
        float boatAngle = (dockAngle)*180/Mathf.PI;
        Instantiate(boat, finalDockCoordinates, Quaternion.Euler(0, boatAngle, 0));
        Vector3 playerCoordinates = dockCoordinates + addon + new Vector3(0, 0.5f, 0);
        Quaternion playerRotation = Quaternion.Euler(0, (dockAngle+Mathf.PI)*180/Mathf.PI, 0);
        Instantiate(player, playerCoordinates, playerRotation); // Instatiate the player
        IslandManager.startingCoordinates = finalDockCoordinates; // Set the starting coordinate
        IslandManager.startingAngle = new Vector3(0, boatAngle - 90, 0); // Set the starting angle
    }

    // Update is called once per frame
    void Update() {

    }
}
