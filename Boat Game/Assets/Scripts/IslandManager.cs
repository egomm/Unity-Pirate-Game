using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour {
    //public static Dictionary<Vector2, float> islandCoordinates = new Dictionary<Vector2, float>();
    public static List<Vector2> islandCoordinates = new List<Vector2>();
    public static Dictionary<Vector2, float> islandInformation = new Dictionary<Vector2, float>();
    // Start is called before the first frame update
    void Start() {
        int count = 100-islandInformation.Count; 
        for (int i = 0; i < count; i++) {
            float angle = Random.Range(0, 2*Mathf.PI); // Angle is based on from (0, 0) positive x direction 
            float magnitude = Random.Range(150f, 1000f); // Effectively distance from centre (0, 0)
            float radius = 10+(10*Random.Range(0.8f, 1f)*magnitude/1000); // Generate an island radius between 10-20 based on randomisation and the magnitude
            // x coordinate = rcos(angle), y coordinate = rsin(angle)
            Vector2 islandCoordinate = new Vector2(magnitude*Mathf.Cos(angle), magnitude*Mathf.Sin(angle));
            bool inRange = true; // Check if the island is in range of any other islands
            foreach (var coordinate in islandCoordinates) {
                if (Vector2.Distance(coordinate, islandCoordinate) < (35 + radius + islandInformation[coordinate])) { // Use 35 as 25 for distance between and 10 for the docks
                    inRange = false;
                }
            }
            if (inRange) {
                islandCoordinates.Add(islandCoordinate);
                islandInformation.Add(islandCoordinate, radius);
            } else { // Go back by an index if the island is in range of another island
                i--;
            }
        }
    }

    // Update is called once per frame
    void Update() {
    }
}
