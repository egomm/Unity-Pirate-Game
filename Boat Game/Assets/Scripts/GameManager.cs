using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TextMeshProUGUI coordinateText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI fpsText;
    private int lastFrame = 0;
    private float[] fpsTracker;
    // Start is called before the first frame update
    void Start() {
        fpsTracker = new float[50];
    }

    // Update is called once per frame
    void Update() {
        // Manage all the important statistics   
        healthText.text = "Health: " + (int) Mathf.Round((float) PlayerManager.playerHealth); // round this
        Vector3 playerCoordinates = PlayerManager.playerCoordinates;
        coordinateText.text = "x: " + (int) Mathf.Round(playerCoordinates.x) + ", y: " + (int) Mathf.Round(playerCoordinates.y) + ", z: " + (int) Mathf.Round(playerCoordinates.z);  
        fpsTracker[lastFrame] = Time.deltaTime;
        lastFrame = (lastFrame + 1) % fpsTracker.Length; // use modulo for remainder
        fpsText.text = "FPS: " + Mathf.RoundToInt(CalculateFps()).ToString();
    }

    private float CalculateFps() {
        float totalTime = 0;
        foreach (float time in fpsTracker) {
            totalTime += time;
        }
        return fpsTracker.Length / totalTime;
    }
}
