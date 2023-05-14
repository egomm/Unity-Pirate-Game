using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSceneGameManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Vector3 coordinate = IslandManager.currentCentre;
        if (IslandManager.islandInformation.ContainsKey(coordinate)) {
            // Create the island and an ocean if the islandInformation contains the coordinate
            IslandManager.instance.CreateIslandAndOcean(coordinate);
        }
    }
}
