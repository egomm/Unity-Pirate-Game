using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGameManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // Initalise 200 islands
        int islandCount = 200-IslandManager.islandInformation.Count; 
        for (int i = 0; i < islandCount; i++) {
            float angle = Random.Range(0, 2*Mathf.PI); // Angle is based on from (0, 0) positive x direction 
            // To calculate magnitude, randomise an area of pi*r^2 -> pi*1000^2 then calculate a new radius with r=sqrt(area/pi)
            float magnitude = Mathf.Sqrt(Random.Range(0f, Mathf.PI*Mathf.Pow(1000, 2))/Mathf.PI); // Distance from centre (0, 0)
            if (!IslandManager.instance.InitaliseIsland(angle, magnitude, true, true)) {
                i--; // Failed to initalise the island, don't increment the index
            }
        }
        // Initalise 1000 pirate ships
        int pirateShipCount = 1000 - PirateShipManager.pirateShipCoordinates.Count;
        for (int i = 0; i < pirateShipCount; i++) {
            float angle = Random.Range(0, 2 * Mathf.PI); // Angle is based on (0, 0) positive x direction
            float magnitude = Mathf.Sqrt(Random.Range(0f, Mathf.PI*Mathf.Pow(1000, 2))/Mathf.PI); // Distance from 0, 0
            if (!PirateShipManager.instance.InitalisePirateShip(angle, magnitude)) {
                i--; // Failed to initalise the pirate ship, don't increment the index
            }
        }
    }

    // Update is called once per frame
    void Update() {
        GameObject boat = GameObject.FindGameObjectWithTag("Boat"); // Find the boat
        // Iterate over the pirate ship coordinates to check if a pirate ship needs to be created or destroyed (depending on the distance from the player)
        foreach (Vector3 coordinate in PirateShipManager.pirateShipCoordinates) {
            float distance = Vector2.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z));
            if (!PirateShipManager.instance.activePirateShips.Contains(coordinate) && distance < 30) {
                PirateShipManager.instance.CreatePirateShip(coordinate, false); // Create the pirate ship
            } else if (PirateShipManager.instance.activePirateShips.Contains(coordinate) && distance > 50) {
                List<GameObject> componentsList = PirateShipManager.instance.activePirateShipInformation[coordinate];
                PirateShipManager.instance.DeletePirateShip(coordinate, componentsList); // Delete the pirate ship
            }
        }
        
        // Iterate over the island coordinates to check if an island needs to be created or destroyed (depending on the distance from the player and the radius of the island)
        foreach (Vector3 coordinate in IslandManager.islandCoordinates) {
            float distance = Vector2.Distance(new Vector2(coordinate.x, coordinate.z), new Vector2(boat.transform.position.x, boat.transform.position.z)); // distance between island coordinate and boat coordinate
            float radius = (float) IslandManager.islandInformation[coordinate]["radius"];
            if (!IslandManager.instance.activeIslands.Contains(coordinate) && distance < (15 + radius)) {
                IslandManager.instance.CreateIsland(coordinate); // Create the island
            } else if (IslandManager.instance.activeIslands.Contains(coordinate) && distance > (30 + radius)) {
                List<GameObject> componentsList = IslandManager.instance.activeIslandInformation[coordinate];
                IslandManager.instance.DeleteIsland(coordinate, componentsList); // Delete the island
            }
            // Check if the boat is within 1.05x the radius of the island - this is for disallowing the player to go onto the island with their boat
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

        // Check if there are any active islands
        GameObject[] islandPrefabs = GameObject.FindGameObjectsWithTag("Island");
        if (islandPrefabs.Length > 0) { 
            // If there are active islands, decrease the wave height to 0 and speed
            if (!InitalisePrefab.instance.runningDecreasing) {
                InitalisePrefab.instance.runningIncreasing = false;
                InitalisePrefab.instance.runningDecreasing = true;
                InitalisePrefab.instance.waveHeightTarget = 0; // target
                InitalisePrefab.instance.ManagePrefabs();
            }
        } else { 
            // If there are no active islands increase the wave height and speed to be propertional to the player's distance from the centre
            if (!InitalisePrefab.instance.runningIncreasing) {
                InitalisePrefab.instance.runningDecreasing = false;
                InitalisePrefab.instance.runningIncreasing = true;
                InitalisePrefab.instance.waveHeightTarget = Vector3.Distance(boat.transform.position, new Vector3(0, 0, 0))/1000; // Wave height target (propertional to the distance from (0, 0, 0)
                InitalisePrefab.instance.ManagePrefabs();
            }
        }
    }
}
