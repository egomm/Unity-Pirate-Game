using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour {
    public static List<Vector3> islandCoordinates = new List<Vector3>();
    public List<Vector3> activeIslands = new List<Vector3>(); // There shouldn't be any active when scenes change!!!
    public static Dictionary<Vector3, Dictionary<string, object>> islandInformation = new Dictionary<Vector3, Dictionary<string, object>>();
    public GameObject island;
    public GameObject dock;
    // Start is called before the first frame update
    void Start() {
        // Need to generate a starting island sometime?
        int count = 100-islandInformation.Count; 
        for (int i = 0; i < count; i++) {
            float angle = Random.Range(0, 2*Mathf.PI); // Angle is based on from (0, 0) positive x direction 
            float magnitude = Random.Range(150f, 1000f); // Effectively distance from centre (0, 0)
            float radius = 10+(10*Random.Range(0.8f, 1f)*magnitude/1000); // Generate an island radius between 10-20 based on randomisation and the magnitude
            float height = Random.Range(radius/5, radius/3); // Generate a random island height betwen 1 and a fifth of the island radius (Between 1 and 4) -> make height between 1/5 and 1/3
            // x coordinate = rcos(angle), z coordinate = rsin(angle)
            Vector3 islandCoordinate = new Vector3(magnitude*Mathf.Cos(angle), 0, magnitude*Mathf.Sin(angle)); // y value will always be 0
            bool inRange = true; // Check if the island is in range of any other islands
            foreach (var coordinate in islandCoordinates) {
                // Use 35 as 25 for distance between and 10 for the docks
                if (Vector2.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(islandCoordinate.x, islandCoordinate.z)) < (35 + radius + (float) islandInformation[coordinate]["radius"])) {
                    inRange = false;
                }
            }
            if (inRange) {
                islandCoordinates.Add(islandCoordinate); 
                Dictionary<string, object> informationDictionary = new Dictionary<string, object>();
                /* Need to add information about:
                island radius,
                island height,
                dock position + rotation + scale, 
                palm tree position(s) + rotation(s), 
                coconut positions(s) + rotation(s) - must be near palm tree,
                rock position(s) + rotation(s), 
                plant position(s) + rotation(s)*/
                float islandAngle = Random.Range(0, 2*Mathf.PI);
                // At a scale of 1, the dock needs to be 4 blocks out 
                float dockScaleX = Random.Range(0.8f, 1f) * radius/15f; // Maximum will be 1.33 - Depend on randomisation + the radius
                float dockScaleY = 0.8f + (Random.Range(0.8f, 1f) * height/20f); // Maximum will be 1 (Min must be 0.8) - Depend on randomisation + height of island
                float dockScaleZ = Random.Range(0.8f, 1f) * radius/22.5f; // Maximum will be 0.88 - Depend on randomisation + the radius - ensure the width is smaller than the length
                Vector3 dockScale = new Vector3(dockScaleX, dockScaleY, dockScaleZ);
                /* Consider case where dock scale generated is (0.6, 0.8, 0.4) with angle = pi/2 and radius = 10 and height = 1 (island spawns at (x, 1, z))
                x = rcos(x), y = rsin(x) -> x = 0, y = 10 -> should be (0, 12.25) with x scale = 0.6
                if x scale was hypothetically = 1 -> should be (0, 13.9)
                if x scale was hypothetically 0.2 -> should be (0, 10.6)
                Based on this x = 10+(xScale*4.125)
                As the dock is on an angle of pi/2, minus pi/2 
                Although if this is on an angle, lets say 3pi/4 -> x = (10+)*sqrt(2)/2 same as z
                If angle = 2pi/3 -> x = (10+(xScale*4.125))*sin(pi/6), z = (10+(xScale*4.125))*cos(pi/6)
                */
                /* CHANGE THIS SYSTEM SO IT BASES OFF ISLAND SCALE
                At 2: use 8 - 1.4 diff
                At 3: use 11 - 2.4 diff
                At 4: use 12 - 2.7 diff
                This will approach 13 (expected) as it get steeper
                seems to be a 2/3 split??
                0.3 at a y dock scale of 1

                For 2d -> x = radius*sqrt(1-((0.75+(0.3*dockScaleY))/(height/2))^2))
                */
                //float dockX = (10+(dockScaleX*4.125f))*Mathf.Sin(angle-(Mathf.PI/2));
                //float dockZ = (10+(dockScaleX*4.125f))*Mathf.Cos(angle-(Mathf.PI/2));
                float adjustedDockHeight = 0.75f + (0.3f*dockScaleY) - 0.06f; // this MUST be less than 1
                float dockMagnitude = radius*Mathf.Sqrt(1-Mathf.Pow(2*adjustedDockHeight/height, 2f)); // Formula is derived from equation of ellipise -> (x/a)^2 + (y/b)^2 = 1 where y = adjustedDockHeight
                float dockConstant = 13*dockScaleX/3;
                float dockX = (dockMagnitude+dockConstant)*Mathf.Sin(islandAngle-(Mathf.PI/2));
                float dockZ = (dockMagnitude+dockConstant)*Mathf.Cos(islandAngle-(Mathf.PI/2));
                Debug.Log(islandAngle*180/Mathf.PI);
                float dockAngle = islandAngle*180/Mathf.PI;
                Vector3 dockCoordinates = new Vector3(dockX, 0.75f, dockZ); 
                informationDictionary.Add("radius", radius);
                informationDictionary.Add("height", height);
                informationDictionary.Add("dockangle", dockAngle);
                informationDictionary.Add("dockscale", dockScale);
                informationDictionary.Add("dockcoordinates", dockCoordinates);

                islandInformation.Add(islandCoordinate, informationDictionary);
            } else { // Go back by an index if the island is in range of another island
                i--;
            }
        }
        Debug.Log(islandInformation.Count);
    }

    // Update is called once per frame
    void Update() {
        GameObject boat = GameObject.FindGameObjectWithTag("Boat");
        foreach (var coordinate in islandCoordinates) {
            // 40 from the distance and 5 from the dock distance and 5 to be safe
            if (!activeIslands.Contains(coordinate) && Vector3.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z)) < (50 + (float) islandInformation[coordinate]["radius"])) {
                Debug.Log("NEARBY ISLAND: " + coordinate.x + ", " + coordinate.y + ", " + coordinate.z);
                Debug.Log("DOCK: " + (Vector3) islandInformation[coordinate]["dockcoordinates"]);
                Debug.Log("ANGLE: " + (float) islandInformation[coordinate]["dockangle"]);
                activeIslands.Add(coordinate);
                Debug.Log("HAS?" + activeIslands.Contains(coordinate));
                // Spawn the island if the island isn't already spawned
                GameObject spawnedIsland = Instantiate(island, coordinate, Quaternion.identity);
                float radius = (float) islandInformation[coordinate]["radius"];
                float height = (float) islandInformation[coordinate]["height"];
                spawnedIsland.transform.localScale = new Vector3(2*radius, height, 2*radius);
                // Spawn the dock at the correct angle
                GameObject spawnedDock = Instantiate(dock, (coordinate + (Vector3) islandInformation[coordinate]["dockcoordinates"]), Quaternion.Euler(new Vector3(0, (float) islandInformation[coordinate]["dockangle"], 0)));
                spawnedDock.transform.localScale = (Vector3) islandInformation[coordinate]["dockscale"];
            } else if (activeIslands.Contains(coordinate) && Vector2.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z)) > (70 + (float) islandInformation[coordinate]["radius"])) {
                Debug.Log("NEED TO REMOVE!");
                activeIslands.Remove(coordinate);
                // Remove the island (Destory())
            }
        }
    }
}
