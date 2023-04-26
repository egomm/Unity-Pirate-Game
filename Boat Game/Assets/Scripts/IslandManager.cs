using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour {
    public static IslandManager instance;
    public static List<Vector3> islandCoordinates = new List<Vector3>();
    public static Dictionary<Vector3, Dictionary<string, object>> islandInformation = new Dictionary<Vector3, Dictionary<string, object>>();
    public List<Vector3> activeIslands = new List<Vector3>(); // There shouldn't be any active when scenes change!!!
    public Dictionary<Vector3, List<GameObject>> activeIslandInformation = new Dictionary<Vector3, List<GameObject>>();
    public GameObject island;
    public GameObject dock;
    public GameObject palmTree;
    public GameObject coconutObject;
    public GameObject rockObject;
    public GameObject plantObject;
    public GameObject chestObject;
    public static Vector3 currentCentre = new Vector3(0, 0, 0);


    private void Awake() { // Called before start 
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        activeIslands.Clear();
        activeIslandInformation.Clear();
    }

    // Initialise information for the island
    public bool InitaliseIsland(float angle, float magnitude) {
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
            plant position(s) + rotation(s),
            chest position(s) + rotation(s)*/
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
            float dockAngle = islandAngle*180/Mathf.PI;
            Vector3 dockCoordinates = new Vector3(dockX, 0.75f, dockZ); 
            // Need to generate palm tree positions -> (x^2+z^2)/a^2 + y^2/b^2 = 1 -> b is 1/2 the height -> a = radius
            // Allow the tree to be on an angle, but must be facing outwards
            List<List<Vector3>> palmTreeCoconutCoordinates = new List<List<Vector3>>();
            List<List<Vector3>> palmTreeCoconutAngles = new List<List<Vector3>>();
            List<Vector3> palmTreeCoordinates = new List<Vector3>();
            List<Vector2> palmTreeAngles = new List<Vector2>();
            int palmTreeCount = (int) Mathf.Round(Random.Range((3*radius/4), (5*radius/4))); // On a island with radius of 10, generate 5-10 palm trees
            for (int j = 0; j < palmTreeCount; j++) {
                float palmTreeAngle = Random.Range(0, 2*Mathf.PI);
                float palmTreeMagnitude = Random.Range(0, (radius*0.95f)); // Generate a magnitude between 0 and 95% of the radius
                float palmTreeX = palmTreeMagnitude*Mathf.Cos(palmTreeAngle);
                float palmTreeZ = palmTreeMagnitude*Mathf.Sin(palmTreeAngle);
                float palmTreeChange = (Mathf.Pow(palmTreeX, 2) + Mathf.Pow(palmTreeZ, 2))/Mathf.Pow(radius, 2);
                float palmTreeConstantAngle = Random.Range(-Mathf.PI/6, Mathf.PI/6);
                float palmTreeConstant = Mathf.Abs(Mathf.Sin(palmTreeConstantAngle))*0.15f; // Will generate between 0 and 0.075
                float palmTreeY = Mathf.Sqrt(1-palmTreeChange)*(height/2) - palmTreeConstant;
                Vector3 palmTreeCoordinate = new Vector3(palmTreeX, palmTreeY, palmTreeZ);
                float palmTreeXAngle = palmTreeConstantAngle*180/Mathf.PI;
                float palmTreeYAngle = palmTreeAngle*180/Mathf.PI;
                // Add to list
                // FIRSTLY CHECK IF THERE ARE ANY OTHER PALM TREES NEARBY
                bool canPlacePalmTree = true;
                for (int k = 0; k < palmTreeCoordinates.Count; k++) {
                    float oldAngle = palmTreeAngles[k].x*Mathf.PI/180;
                    float maximum = 5;
                    if (Vector3.Distance(islandCoordinate, palmTreeCoordinate) > Vector3.Distance(islandCoordinate, palmTreeCoordinates[k])) { // New palm tree is further out
                        maximum = 5+3*(Mathf.Sin(oldAngle)-Mathf.Sin(palmTreeConstantAngle)); // 6+4(sin(old)-sin(new)) 
                    } else { // New palm tree is closer
                        maximum = 5+3*(Mathf.Sin(palmTreeConstantAngle)-Mathf.Sin(oldAngle));
                    }
                    if (Vector3.Distance(palmTreeCoordinate, palmTreeCoordinates[k]) < maximum) {
                        canPlacePalmTree = false;
                        j--; // go back one index
                        break;
                    }
                }
                if (canPlacePalmTree) {
                    // Chance of coconuts around the base of the palm tree
                    int coconutCount = Random.Range(0, 4); // Spawn between 0 and 3 coconuts (this function is inclusive)
                    List<Vector3> coconutCoordinates = new List<Vector3>();
                    List<Vector3> coconutAngles = new List<Vector3>();
                    for (int k = 0; k < coconutCount; k++) {
                        // Spawn the coconut 1-2 units from the tree at a random angle -> combined coconuts exist so it doesnt matter too much if they overlap (lodoicea)
                        // All of this is relative to the palm tree
                        float coconutAngle = Random.Range(0, 2*Mathf.PI);
                        float coconutMagnitude = Random.Range(0.5f, 1f);
                        float coconutX = coconutMagnitude*Mathf.Cos(coconutAngle);
                        float coconutZ = coconutMagnitude*Mathf.Sin(coconutAngle);
                        Vector3 coconutPosition = palmTreeCoordinate + new Vector3(coconutX, 0, coconutZ); // make the y depend on from the centre
                        if (Vector2.Distance(new Vector2(coconutPosition.x, coconutPosition.z), new Vector2(0, 0)) < 0.95f*radius) {
                            float randomCoconutAngle = Random.Range(0f, 360f);
                            coconutAngles.Add(new Vector3(0, randomCoconutAngle, 0));
                            coconutCoordinates.Add(coconutPosition);
                        } else {
                            k--;
                        }
                    }
                    if (coconutCoordinates.Count > 0) {
                        palmTreeCoconutCoordinates.Add(coconutCoordinates);
                        palmTreeCoconutAngles.Add(coconutAngles);
                    }
                    palmTreeCoordinates.Add(palmTreeCoordinate);
                    palmTreeAngles.Add(new Vector2(palmTreeXAngle, palmTreeYAngle));
                } 
            }
            // Rocks
            List<Vector3> rockCoordinates = new List<Vector3>();
            List<Vector3> rockAngles = new List<Vector3>();
            int rockCount = (int) Mathf.Round(Random.Range((3*radius/2), (3*radius)));
            for (int j = 0; j < rockCount; j++) { // Allow overlapping rocks (they look cool)
                float rockAngle = Random.Range(0, 2*Mathf.PI);
                float rockAngleMangitude = Random.Range(0, (radius*0.95f));
                float rockX = rockAngleMangitude*Mathf.Cos(rockAngle);
                float rockZ = rockAngleMangitude*Mathf.Sin(rockAngle);
                float rockChange = (Mathf.Pow(rockX, 2) + Mathf.Pow(rockZ, 2))/Mathf.Pow(radius, 2);
                float rockY = Mathf.Sqrt(1-rockChange)*(height/2) - 0.1f;
                bool canPlaceRock = true;
                foreach (var palmTreeCoordinate in palmTreeCoordinates) {
                    if (Vector2.Distance(new Vector2(rockX, rockZ), new Vector2(palmTreeCoordinate.x, palmTreeCoordinate.z)) < 1.5f) {
                        canPlaceRock = false;
                        j--; // go back one index
                        break;
                    }
                }
                if (canPlaceRock) {
                    float randomRockAngle = Random.Range(0f, 360f);
                    rockAngles.Add(new Vector3(0, randomRockAngle, 0));
                    rockCoordinates.Add(new Vector3(rockX, rockY, rockZ));
                }
            }
            // Plants
            List<Vector3> plantCoordinates = new List<Vector3>();
            List<Vector3> plantAngles = new List<Vector3>();
            int plantCount = (int) Mathf.Round(Random.Range((2*radius), (4*radius)));
            for (int j = 0; j < plantCount; j++) { // Don't allow directly overlapping plants 
                float plantAngle = Random.Range(0, 2*Mathf.PI);
                float plantAngleMangitude = Random.Range(0, (radius*0.95f));
                float plantX = plantAngleMangitude*Mathf.Cos(plantAngle);
                float plantZ = plantAngleMangitude*Mathf.Sin(plantAngle);
                float plantChange = (Mathf.Pow(plantX, 2) + Mathf.Pow(plantZ, 2))/Mathf.Pow(radius, 2);
                float plantY = Mathf.Sqrt(1-plantChange)*(height/2) - 0.05f;
                bool canPlacePlant = true;
                foreach (var palmTreeCoordinate in palmTreeCoordinates) {
                    if (Vector2.Distance(new Vector2(plantX, plantZ), new Vector2(palmTreeCoordinate.x, palmTreeCoordinate.z)) < 1.5f) {
                        canPlacePlant = false;
                        j--; // go back one index
                        break;
                    }
                }
                if (canPlacePlant) {
                    foreach (var rockCoordinate in rockCoordinates) {
                        if (Vector2.Distance(new Vector2(plantX, plantZ), new Vector2(rockCoordinate.x, rockCoordinate.z)) < 1f) {
                            canPlacePlant = false;
                            j--; // go back one index
                            break;
                        }
                    }
                    if (canPlacePlant) {
                        foreach (var plantCoordinate in plantCoordinates) {
                            if (Vector2.Distance(new Vector2(plantX, plantZ), new Vector2(plantCoordinate.x, plantCoordinate.z)) < 0.5f) {
                                canPlacePlant = false;
                                j--; // go back one index
                                break;
                            }
                        }
                    }
                }
                if (canPlacePlant) {
                    float randomPlantAngle = Random.Range(0f, 360f);
                    plantAngles.Add(new Vector3(0, randomPlantAngle, 0));
                    plantCoordinates.Add(new Vector3(plantX, plantY, plantZ));
                }
            }
            // Chests
            List<Vector3> chestCoordinates = new List<Vector3>();
            List<Vector3> chestAngles = new List<Vector3>();
            int chestCount = (int) Mathf.Round(Random.Range(radius/10, radius/5));
            for (int j = 0; j < chestCount; j++) { // Don't allow overlapping chests, allow chests to overlap plants 
                float chestAngle = Random.Range(0, 2*Mathf.PI);
                float chestAngleMangitude = Random.Range(0, (radius/2)); // must be somewhat close to the centre
                float chestX = chestAngleMangitude*Mathf.Cos(chestAngle);
                float chestZ = chestAngleMangitude*Mathf.Sin(chestAngle);
                float chestChange = (Mathf.Pow(chestX, 2) + Mathf.Pow(chestZ, 2))/Mathf.Pow(radius, 2);
                float chestY = Mathf.Sqrt(1-chestChange)*(height/2) - 0.1f;
                bool canPlaceChest = true;
                foreach (var palmTreeCoordinate in palmTreeCoordinates) {
                    if (Vector2.Distance(new Vector2(chestX, chestZ), new Vector2(palmTreeCoordinate.x, palmTreeCoordinate.z)) < 2.5f) {
                        canPlaceChest = false;
                        j--; // go back one index
                        break;
                    }
                }
                if (canPlaceChest) {
                    foreach (var rockCoordinate in rockCoordinates) {
                        if (Vector2.Distance(new Vector2(chestX, chestZ), new Vector2(rockCoordinate.x, rockCoordinate.z)) < 1.5f) {
                            canPlaceChest = false;
                            j--; // go back one index
                            break;
                        }
                    }
                    if (canPlaceChest) {
                        foreach (var chestCoordinate in chestCoordinates) {
                            if (Vector2.Distance(new Vector2(chestX, chestZ), new Vector2(chestCoordinate.x, chestCoordinate.z)) < 1.5f) {
                                canPlaceChest = false;
                                j--; // go back one index
                                break;
                            }
                        }
                    }
                }
                if (canPlaceChest) {
                    float randomChestAngle = Random.Range(0f, 360f);
                    chestAngles.Add(new Vector3(0, randomChestAngle, 0));
                    chestCoordinates.Add(new Vector3(chestX, chestY, chestZ));
                }
            }
            informationDictionary.Add("radius", radius);
            informationDictionary.Add("height", height);
            informationDictionary.Add("dockangle", dockAngle);
            informationDictionary.Add("dockscale", dockScale);
            informationDictionary.Add("dockcoordinates", dockCoordinates);
            informationDictionary.Add("palmtreecoordinates", palmTreeCoordinates);
            informationDictionary.Add("palmtreeangles", palmTreeAngles);
            informationDictionary.Add("coconutcoordinates", palmTreeCoconutCoordinates);
            informationDictionary.Add("coconutangles", palmTreeCoconutAngles);
            informationDictionary.Add("rockcoordinates", rockCoordinates);
            informationDictionary.Add("rockangles", rockAngles);
            informationDictionary.Add("plantcoordinates", plantCoordinates);
            informationDictionary.Add("plantangles", plantAngles);
            informationDictionary.Add("chestcoordinates", chestCoordinates);
            informationDictionary.Add("chestangles", chestAngles);
            islandInformation.Add(islandCoordinate, informationDictionary);
            return true;
        } // Go back by an index if the island is in range of another island
        return false;
    }

    public void CreateIsland(Vector3 coordinate) {
        Debug.Log("NEARBY ISLAND: " + coordinate.x + ", " + coordinate.y + ", " + coordinate.z);
        activeIslands.Add(coordinate);
        List<GameObject> componentsList = new List<GameObject>();
        // Spawn the island if the island isn't already spawned
        GameObject spawnedIsland = Instantiate(island, coordinate, Quaternion.identity);
        componentsList.Add(spawnedIsland);
        float radius = (float) islandInformation[coordinate]["radius"];
        float height = (float) islandInformation[coordinate]["height"];
        spawnedIsland.transform.localScale = new Vector3(2*radius, height, 2*radius);
        // Spawn the dock at the correct angle
        GameObject spawnedDock = Instantiate(dock, (coordinate + (Vector3) islandInformation[coordinate]["dockcoordinates"]), Quaternion.Euler(new Vector3(0, (float) islandInformation[coordinate]["dockangle"], 0)));
        componentsList.Add(spawnedDock);
        spawnedDock.transform.localScale = (Vector3) islandInformation[coordinate]["dockscale"];
        List<Vector3> palmTreeCoordinates = (List<Vector3>) islandInformation[coordinate]["palmtreecoordinates"];
        List<Vector2> palmTreeAngles = (List<Vector2>) islandInformation[coordinate]["palmtreeangles"]; // These will both be the same length 
        for (int i = 0; i < palmTreeCoordinates.Count; i++) { 
            Vector3 palmTreeCoordinate = palmTreeCoordinates[i];
            Vector2 palmTreeAngle = palmTreeAngles[i];
            GameObject spawnedPalmTree = Instantiate(palmTree, coordinate + palmTreeCoordinate, Quaternion.Euler(palmTreeAngle.x, palmTreeAngle.y, 0));
            componentsList.Add(spawnedPalmTree);
        }
        List<List<Vector3>> coconutCoordinates = (List<List<Vector3>>) islandInformation[coordinate]["coconutcoordinates"];
        List<List<Vector3>> coconutAngles = (List<List<Vector3>>) islandInformation[coordinate]["coconutangles"];
        for (int i = 0; i < coconutCoordinates.Count; i++) {
            for (int j = 0; j < coconutCoordinates[i].Count; j++) {
                GameObject spawnedCoconut = Instantiate(coconutObject, (coordinate + coconutCoordinates[i][j]), Quaternion.Euler(coconutAngles[i][j]));
                componentsList.Add(spawnedCoconut);
                spawnedCoconut.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
        List<Vector3> rockCoordinates = (List<Vector3>) islandInformation[coordinate]["rockcoordinates"];
        List<Vector3> rockAngles = (List<Vector3>) islandInformation[coordinate]["rockangles"];
        for (int i = 0; i < rockCoordinates.Count; i++) {
            GameObject spawnedRock = Instantiate(rockObject, (coordinate + rockCoordinates[i]), Quaternion.Euler(rockAngles[i]));
            componentsList.Add(spawnedRock);
        }
        List<Vector3> plantCoordinates = (List<Vector3>) islandInformation[coordinate]["plantcoordinates"];
        List<Vector3> plantAngles = (List<Vector3>) islandInformation[coordinate]["plantangles"]; // These will be the same size
        for (int i = 0; i < plantCoordinates.Count; i++) {
            GameObject spawnedPlant = Instantiate(plantObject, (coordinate + plantCoordinates[i]), Quaternion.Euler(plantAngles[i]));
            componentsList.Add(spawnedPlant);
        }
        List<Vector3> chestCoordinates = (List<Vector3>) islandInformation[coordinate]["chestcoordinates"];
        List<Vector3> chestAngles = (List<Vector3>) islandInformation[coordinate]["chestangles"]; // These will be the same size
        for (int i = 0; i < chestCoordinates.Count; i++) {
            GameObject spawnedChest = Instantiate(chestObject, (coordinate + chestCoordinates[i]), Quaternion.Euler(chestAngles[i]));
            componentsList.Add(spawnedChest);
        }
        activeIslandInformation.Add(coordinate, componentsList);
        Debug.Log("DONE?");
    }

    public void DeleteIsland(Vector3 coordinate, List<GameObject> componentsList) {
        foreach (var component in componentsList) {
        Destroy(component);
        }
        activeIslands.Remove(coordinate);
        activeIslandInformation.Remove(coordinate);
    }

    // Update is called once per frame
    void Update() {

    }
}
