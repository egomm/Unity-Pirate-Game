using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TextMeshProUGUI coordinateText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI killText;
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
        goldText.text = "Gold Looted: " + AbbreviatedForms(PlayerManager.goldLooted);
        killText.text = "Pirates Killed: " + PlayerManager.piratesKilled;
    }

    private float CalculateFps() {
        float totalTime = 0;
        foreach (float time in fpsTracker) {
            totalTime += time;
        }
        return fpsTracker.Length / totalTime;
    }

    private string AbbreviatedForms(int number) {
        int exponent = (int) Mathf.Floor(Mathf.Log10(number));
        switch (exponent) {
            case 3: case 4: case 5: // 1k, 10k, 100k
                return RoundToSignificantDigits(number/1e3, 4).ToString() + "k";
        }
        return number.ToString();
    }

    private float RoundToSignificantDigits(double input, int digits) { // Converts to a certain amount of significant figures
        if (input.ToString().Length < digits + 1) {
            return (float) input;
        }
        return float.Parse(input.ToString().Substring(0, digits + 1), CultureInfo.InvariantCulture.NumberFormat);
    }
}
