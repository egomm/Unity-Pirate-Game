using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSceneGameManager : MonoBehaviour {
    public GameObject player;
    public GameObject thePlayer;

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

        //Instantiate(island, new Vector3(250, 0, 250), Quaternion.identity);
        Vector3 dockCoordinates = coordinate + (Vector3) IslandManager.islandInformation[coordinate]["dockcoordinates"];
        thePlayer = Instantiate(player);
        thePlayer.transform.position = dockCoordinates + new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update() {

    }
}
