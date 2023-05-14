using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandManager : MonoBehaviour {
    public static IslandManager instance;
    public static List<Vector3> islandCoordinates = new List<Vector3>();
    public static Dictionary<Vector3, Dictionary<string, object>> islandInformation = new Dictionary<Vector3, Dictionary<string, object>>();
    public List<Vector3> activeIslands = new List<Vector3>(); // There shouldn't be any active when scenes change!!!
    public Dictionary<Vector3, List<GameObject>> activeIslandInformation = new Dictionary<Vector3, List<GameObject>>();
    // Information for regular island
    public GameObject island;
    public GameObject islandBottom;
    public GameObject dock;
    public GameObject palmTree;
    public GameObject coconutObject;
    public GameObject rockObject;
    public GameObject plantObject;
    public GameObject chestObject;
    // Pirates
    public GameObject[] pirates = new GameObject[4];
    public GameObject pirateCaptain;
    // Information for regular island with oceam
    public GameObject player;
    public GameObject ocean;
    public GameObject seaFloor;
    public GameObject boat;
    // Information for the boat/island
    public static Vector3 startingCoordinates; 
    public static Vector3 startingAngle;
    public static Vector3 currentCentre = new Vector3(0, 0, 0);
    // Pirate information
    public static List<GameObject> activePirates = new List<GameObject>();

    public Vector3 currentCoordinate;
    public int pirateIndex;


    private void Awake() { // Called before start 
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        if (SceneManager.GetActiveScene().name != "Island Scene") {
            activeIslands.Clear();
            activeIslandInformation.Clear();
        }
    }

    // Initialise information for the island
    public bool InitaliseIsland(float angle, float magnitude, bool spawnChests, bool spawnPirate) {
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
            List<int> chestGold = new List<int>();
            List<int> maxChestGold = new List<int>();
            if (spawnChests) {
                int chestCount = (int) Mathf.Round(Random.Range(radius / 10, radius / 5));
                for (int j = 0; j < chestCount; j++) { // Don't allow overlapping chests, allow chests to overlap plants 
                    float chestAngle = Random.Range(0, 2 * Mathf.PI);
                    float chestAngleMangitude = Random.Range(0, (radius / 2)); // must be somewhat close to the centre
                    float chestX = chestAngleMangitude * Mathf.Cos(chestAngle);
                    float chestZ = chestAngleMangitude * Mathf.Sin(chestAngle);
                    float chestChange = (Mathf.Pow(chestX, 2) + Mathf.Pow(chestZ, 2)) / Mathf.Pow(radius, 2);
                    float chestY = Mathf.Sqrt(1 - chestChange) * (height / 2) - 0.1f;
                    bool canPlaceChest = true;
                    // Iterate over all of the objects which the chests could potentially collide with
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
                        int chestsGold = (int) (Mathf.Pow(magnitude, 1.15f) * Random.Range(0.5f, 1f)) + 200; // Between 200 - 3000 gold per chest (depedent on distance)
                        chestAngles.Add(new Vector3(0, randomChestAngle, 0));
                        chestCoordinates.Add(new Vector3(chestX, chestY, chestZ));
                        chestGold.Add(chestsGold);
                        maxChestGold.Add(chestsGold);
                    }
                }
            }
            List<Vector3> pirateCoordinates = new List<Vector3>();
            List<Vector3> pirateAngles = new List<Vector3>();
            List<GameObject> piratesToSpawn = new List<GameObject>();
            List<float> piratesHealth = new List<float>();
            if (spawnPirate) {
                if (spawnChests) { // Spawn the pirates near to the chests (within a third of the island radius)
                    // Spawn between 0 and 2 pirates per chest
                    foreach (var chestCoordinate in chestCoordinates) { // Iterate over the chest coordinates
                        int pirateCount = Random.Range(1, 3); // Using Random.Range() with integers is exclusive of the upper bound (spawn 1 or 2 pirates)
                        for (int j = 0; j < pirateCount; j++) {
                            float pirateSpawnRadius = Random.Range(1f, radius / 3); // Spawn the pirate within 1 unit (prevents pirate being spawned in chest) and 1/3 island radii of the chest
                            float randomAngle = Random.Range(0, 2 * Mathf.PI); // Determine the angle from the chest at which to spawn the pirate
                            float addPirateSpawnX = pirateSpawnRadius * Mathf.Cos(randomAngle);
                            float addPirateSpawnZ = pirateSpawnRadius * Mathf.Sin(randomAngle);
                            Vector3 addPirateSpawn = new Vector3(addPirateSpawnX, 0.75f, addPirateSpawnZ); // Add on 0.75 units upwards to account for gradient etc (customise this later?)
                            Vector3 pirateSpawnCoordinate = chestCoordinate + addPirateSpawn;
                            Vector2 twoDimensionalPirateSpawnCoordinate = new Vector2(pirateSpawnCoordinate.x, pirateSpawnCoordinate.z);
                            bool canSpawnPirate = true;
                            foreach (var palmTreeCoordinate in palmTreeCoordinates) {
                                if (Vector2.Distance(twoDimensionalPirateSpawnCoordinate, new Vector2(palmTreeCoordinate.x, palmTreeCoordinate.z)) < 2.5f) {
                                    canSpawnPirate = false;
                                    j--;
                                    break;
                                }
                            }
                            if (canSpawnPirate) {
                                foreach (var rockCoordinate in rockCoordinates) {
                                    if (Vector2.Distance(twoDimensionalPirateSpawnCoordinate, new Vector2(rockCoordinate.x, rockCoordinate.z)) < 1.5f) {
                                        canSpawnPirate = false;
                                        j--;
                                        break;
                                    }
                                }
                                if (canSpawnPirate) {
                                    foreach (var pirateCoordinate in pirateCoordinates) {
                                        if (Vector2.Distance(twoDimensionalPirateSpawnCoordinate, new Vector2(pirateCoordinate.x, pirateCoordinate.z)) < 1.5f) {
                                            canSpawnPirate = false;
                                            j--;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (canSpawnPirate) { // Passed all test cases
                                float randomPirateAngle = Random.Range(0f, 360f);
                                GameObject pirateToSpawn;
                                float pirateHealth = 5; 
                                float healthAddon = Mathf.Log(magnitude/200, 2);
                                if (Random.Range(0, 100) >= 96) {
                                    pirateToSpawn = pirateCaptain;
                                    pirateHealth += 5 + 5*healthAddon; 
                                } else {
                                    int pirateSpawnIndex = Random.Range(0, 4); // Pick an index from [0, 1, 2, 3]
                                    pirateHealth += (pirateSpawnIndex + 1) + (pirateSpawnIndex + 1)*healthAddon; 
                                    pirateToSpawn = pirates[pirateSpawnIndex];
                                }
                                // Add the health of the players
                                piratesHealth.Add(pirateHealth); // The pirate health will range between 5-10 so that the player can kill the pirate within 2.5 - 5 seconds 
                                pirateAngles.Add(new Vector3(0, randomPirateAngle, 0));
                                pirateCoordinates.Add(pirateSpawnCoordinate);
                                piratesToSpawn.Add(pirateToSpawn);
                            }
                        }
                    }
                } else { // Spawn the pirates wherever possible
                    int pirateCount = (int) Mathf.Round(Random.Range(radius / 10, radius / 5)); // Same spawn rate as what chests would have been
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
            informationDictionary.Add("chestgold", chestGold);
            informationDictionary.Add("maxchestgold", maxChestGold);
            informationDictionary.Add("piratecoordinates", pirateCoordinates);
            informationDictionary.Add("pirateangles", pirateAngles);
            informationDictionary.Add("piratestospawn", piratesToSpawn);
            informationDictionary.Add("pirateshealth", piratesHealth);
            islandInformation.Add(islandCoordinate, informationDictionary);
            return true;
        } // Go back by an index if the island is in range of another island
        return false;
    }

    public void CreateIsland(Vector3 coordinate) {
        currentCoordinate = coordinate;
        activeIslands.Add(coordinate);
        List<GameObject> componentsList = new List<GameObject>();
        // Spawn the island if the island isn't already spawned
        GameObject spawnedIsland = Instantiate(island, coordinate, Quaternion.identity);
        componentsList.Add(spawnedIsland);
        float radius = (float) islandInformation[coordinate]["radius"];
        float height = (float) islandInformation[coordinate]["height"];
        spawnedIsland.transform.localScale = new Vector3(2*radius, height, 2*radius);
        // Spawn the island bottom at the correct position and rotation
        GameObject spawnedIslandBottom = Instantiate(islandBottom, new Vector3(coordinate.x, -1.5f, coordinate.z), Quaternion.identity);
        componentsList.Add(spawnedIslandBottom);
        spawnedIslandBottom.transform.localScale = new Vector3(1.98f*radius, 1.5f, 1.98f*radius);
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
        List<Vector3> pirateCoordinates = (List<Vector3>) islandInformation[currentCoordinate]["piratecoordinates"];
        activeIslandInformation.Add(coordinate, componentsList);
        if (pirateCoordinates.Count > 0) {
            pirateIndex = 0;
            activePirates.Clear();
            Invoke("SpawnPirates", 0.25f);
        }
    }

    public void SpawnPirates() {
        if (activeIslandInformation.ContainsKey(currentCoordinate)) { // Island has gone if this is false
            List<Vector3> pirateCoordinates = (List<Vector3>)islandInformation[currentCoordinate]["piratecoordinates"];
            List<Vector3> pirateAngles = (List<Vector3>)islandInformation[currentCoordinate]["pirateangles"]; // These will be the same size
            List<GameObject> piratesToSpawn = (List<GameObject>)islandInformation[currentCoordinate]["piratestospawn"];
            if (pirateIndex < pirateCoordinates.Count) { // Resolves out of bounds issues
                List<float> piratesHealth = (List<float>) IslandManager.islandInformation[currentCoordinate]["pirateshealth"];
                if (piratesHealth[pirateIndex] > 0) {  // Cannot spawn a dead pirate
                    GameObject spawnedPirate = Instantiate(piratesToSpawn[pirateIndex], currentCoordinate + pirateCoordinates[pirateIndex], Quaternion.Euler(pirateAngles[pirateIndex]));
                    activePirates.Add(spawnedPirate);
                    List<GameObject> componentsList = activeIslandInformation[currentCoordinate];
                    componentsList.Add(spawnedPirate);
                    activeIslandInformation[currentCoordinate] = componentsList;
                }
                if (pirateIndex < pirateCoordinates.Count - 1) { // Spawn the next pirate
                    pirateIndex++;
                    Invoke("SpawnPirates", 0.1f);
                }
            }
        }
    }

    public void CreateIslandAndOcean(Vector3 coordinate) {
        CreateIsland(coordinate);
        // Instantiate the ocean and the sea floor in a radius 
        float radius = (float) islandInformation[coordinate]["radius"];
        // Spawn prefabs within 4x the radius of the island, and disallow the player out of 1.75x the radius of the island
        // Ocean prefab is 10x10
        int min = (int) Mathf.Round(radius / 10) * -6;
        int max = (int) Mathf.Round(radius / 10) * 6;
        for (int x = min; x <= max; x++) {
            for (int z = min; z <= max; z++) {
                Instantiate(ocean, new Vector3(10 * x, 0, 10 * z) + coordinate, Quaternion.identity);
                Instantiate(seaFloor, new Vector3(10 * x, -3, 10 * z) + coordinate, Quaternion.identity);
            }
        }
        //Instantiate(island, new Vector3(250, 0, 250), Quaternion.identity);
        Vector3 dockCoordinates = coordinate + (Vector3) islandInformation[coordinate]["dockcoordinates"];
        Vector3 dockScale = (Vector3) islandInformation[coordinate]["dockscale"];
        float dockAngle = (float) islandInformation[coordinate]["dockangle"] * Mathf.PI / 180;
        float dockWidthZ = 5f * dockScale.z; // Adjust this?
        // At a scale of 1, the dock length is approx 13 
        float xAddon = (65 / 12) * dockScale.x * Mathf.Sin(dockAngle - (Mathf.PI / 2));
        float zAddon = (65 / 12) * dockScale.x * Mathf.Cos(dockAngle - (Mathf.PI / 2));
        Vector3 addon = new Vector3(xAddon, 0, zAddon);
        float xChange = dockWidthZ * Mathf.Sin(dockAngle - Mathf.PI);
        float zChange = dockWidthZ * Mathf.Cos(dockAngle - Mathf.PI);
        Vector3 change = new Vector3(xChange, 0, zChange);
        Vector3 newDockCoordinates = addon + dockCoordinates + change;
        Vector3 finalDockCoordinates = new Vector3(newDockCoordinates.x, 0, newDockCoordinates.z);
        // Need to spawn the boat -> width is ~5.5 when the island is at a z scale of 1
        float boatAngle = (dockAngle) * 180 / Mathf.PI;
        Instantiate(boat, finalDockCoordinates, Quaternion.Euler(0, boatAngle, 0));
        Vector3 playerCoordinates = dockCoordinates + addon + new Vector3(0, 0.5f, 0);
        Quaternion playerRotation = Quaternion.Euler(0, (dockAngle + 0.5f * Mathf.PI) * 180 / Mathf.PI, 0);
        Instantiate(player, playerCoordinates, playerRotation); // Instatiate the player
        startingCoordinates = finalDockCoordinates; // Set the starting coordinate
        startingAngle = new Vector3(0, boatAngle - 90, 0); // Set the starting angle
    }

    public void DeleteIsland(Vector3 coordinate, List<GameObject> componentsList) {
        foreach (GameObject component in componentsList) {
            Destroy(component);
        }
        activeIslands.Remove(coordinate);
        activeIslandInformation.Remove(coordinate);
    }

    // Update is called once per frame
    void Update() {

    }
}
