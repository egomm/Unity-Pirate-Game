using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateSceneManager : MonoBehaviour {
    public static Vector3 currentPirateShipCoordinates = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start() {
        if (PirateShipManager.pirateShipCoordinates.Contains(currentPirateShipCoordinates)) {
            PirateShipManager.instance.CreatePirateShipAndOcean(currentPirateShipCoordinates);
        }
    }

    // Update is called once per frame
    void Update() {
    }
}
