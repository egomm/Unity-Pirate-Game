using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGameManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        int count = 100-IslandManager.islandInformation.Count; 
        Debug.Log(IslandManager.instance);
        for (int i = 0; i < count; i++) {
            float angle = Random.Range(0, 2*Mathf.PI); // Angle is based on from (0, 0) positive x direction 
            float magnitude = Random.Range(150f, 1000f); // Effectively distance from centre (0, 0)
            if (!IslandManager.instance.InitaliseIsland(angle, magnitude)) {
                i--;
            }
        }
        Debug.Log(IslandManager.islandInformation.Count);
    }

    // Update is called once per frame
    void Update() {
    Debug.Log(IslandManager.instance);
        GameObject boat = GameObject.FindGameObjectWithTag("Boat");
        foreach (Vector3 coordinate in IslandManager.islandCoordinates) {
            // 40 from the distance and 5 from the dock distance and 5 to be safe
            if (!IslandManager.instance.activeIslands.Contains(coordinate) && Vector3.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z)) < (50 + (float) IslandManager.islandInformation[coordinate]["radius"])) {
                IslandManager.instance.CreateIsland(coordinate);
            } else if (IslandManager.instance.activeIslands.Contains(coordinate) && Vector2.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z)) > (70 + (float) IslandManager.islandInformation[coordinate]["radius"])) {
                Debug.Log("NEED TO REMOVE!");
                // Remove the island (Destroy())
                List<GameObject> componentsList = IslandManager.instance.activeIslandInformation[coordinate];
                IslandManager.instance.DeleteIsland(coordinate, componentsList);
            }
        }
    }
}
