using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TextMeshProUGUI coordinateText;
    public TextMeshProUGUI healthText;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // Manage all the important statistics   
        healthText.text = "Health: " + (int) Mathf.Round((float) PlayerManager.playerHealth); // round this
        Vector3 playerCoordinates = PlayerManager.playerCoordinates;
        coordinateText.text = "x: " + (int) Mathf.Round(playerCoordinates.x) + ", y: " + (int) Mathf.Round(playerCoordinates.y) + ", z: " + (int) Mathf.Round(playerCoordinates.z);  
    }
}
