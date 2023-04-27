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
    //Debug.Log(IslandManager.instance);
        GameObject boat = GameObject.FindGameObjectWithTag("Boat");
        foreach (Vector3 coordinate in IslandManager.islandCoordinates) {
            // 40 from the distance and 5 from the dock distance and 5 to be safe
            float distance = Vector2.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z)); // distance between island coordinate and boat coordinate
            float radius = (float) IslandManager.islandInformation[coordinate]["radius"];
            if (!IslandManager.instance.activeIslands.Contains(coordinate) && distance < (50 + radius)) {
                IslandManager.instance.CreateIsland(coordinate);
            } else if (IslandManager.instance.activeIslands.Contains(coordinate) && distance > (70 + radius)) {
                Debug.Log("NEED TO REMOVE!");
                // Remove the island (Destroy())
                List<GameObject> componentsList = IslandManager.instance.activeIslandInformation[coordinate];
                IslandManager.instance.DeleteIsland(coordinate, componentsList);
            }
            // Check if the boat is within 1.05x the radius of the island
            if (distance <= radius*1.05f) {
                float gradient = (coordinate.z - boat.transform.position.z)/(coordinate.x - boat.transform.position.x);
                float angle = Mathf.Atan(gradient);
                Vector3 newCoordinate;
                float newXPos = radius*1.05f*Mathf.Cos(angle);
                float newZPos = radius*1.05f*Mathf.Sin(angle);
                if (boat.transform.position.x < coordinate.x) { // -- if the boat pos is less than the island centre x pos, else ++
                    newCoordinate = coordinate + new Vector3(-newXPos, 0, -newZPos);
                } else {
                    newCoordinate = coordinate + new Vector3(newXPos, 0, newZPos);
                }
                boat.transform.position = new Vector3(newCoordinate.x, 0, newCoordinate.z);
            } 
        }
    }
}
