using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSceneGameManager : MonoBehaviour {
    public GameObject player;
    public GameObject ocean;
    public GameObject seaFloor;
    public GameObject boat;
    private GameObject thePlayer;

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
        Debug.Log(IslandManager.instance.InitaliseIsland(angle, magnitude));
        IslandManager.instance.InitaliseIsland(angle, magnitude); // keep attempting to initalise island
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
        float dockWidthZ = 11f * dockScale.z;
        // Change needed for x coordinates from dock position !NEED TO FIX THIS!
        float xChange = (dockCoordinates.x/Mathf.Abs(dockCoordinates.x))*dockWidthZ*Mathf.Cos(dockAngle);
        float zChange = (dockCoordinates.z/Mathf.Abs(dockCoordinates.z))*dockWidthZ *Mathf.Sin(dockAngle);
        Debug.Log("HI");
        Debug.Log(dockAngle);
        Debug.Log(dockCoordinates);
        Debug.Log(new Vector3(xChange, 0, zChange));
        Debug.Log(dockCoordinates + new Vector3(xChange, 0, zChange));
        // Need to spawn the boat -> width is ~5.5 when the island is at a z scale of 1
        //Vector3 difference = 
        thePlayer = Instantiate(player);
        thePlayer.transform.position = dockCoordinates + new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update() {

    }
}
