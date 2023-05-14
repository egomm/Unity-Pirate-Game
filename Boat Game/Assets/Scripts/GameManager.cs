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

    // Start is called when the script is loaded
    void Start() {
        fpsTracker = new float[50];
    }

    // Update is called once per frame
    void Update() {
        // Manage all the important statistics   
        if ((float) PlayerManager.playerHealth > 0) { // Never display the health as being in negatives
            healthText.text = "Health: " + (int) Mathf.Round((float) PlayerManager.playerHealth); // round this
        } else {
            healthText.text = "Health: 0";
        }
        Vector3 playerCoordinates = PlayerManager.playerCoordinates;
        coordinateText.text = "x: " + (int) Mathf.Round(playerCoordinates.x) + ", y: " + (int) Mathf.Round(playerCoordinates.y) + ", z: " + (int) Mathf.Round(playerCoordinates.z);  
        fpsTracker[lastFrame] = Time.deltaTime;
        lastFrame = (lastFrame + 1) % fpsTracker.Length; // use modulo for remainder
        fpsText.text = "FPS: " + Mathf.RoundToInt(CalculateFps()).ToString();
        goldText.text = "Gold Looted: " + AbbreviatedForms(PlayerManager.goldLooted);
        killText.text = "Pirates Killed: " + AbbreviatedForms(PlayerManager.piratesKilled);
    }

    private float CalculateFps() {
        // Method for calculating the player's fps by getting the fps over a period of time
        float totalTime = 0;
        foreach (float time in fpsTracker) {
            totalTime += time;
        }
        return fpsTracker.Length / totalTime;
    }

    public static string AbbreviatedForms(int number) {
        // Abbreviate a number (for example 1000 -> 1k, 10000 -> 10k)
        int exponent = (int) Mathf.Floor(Mathf.Log10(number));
        switch (exponent) {
            case 3: case 4: case 5: // 1k, 10k, 100k
                return RoundToSignificantDigits(number / 1e3, 4).ToString() + "k";
            case 6: case 7: case 8: // 1m, 10m, 100m
                return RoundToSignificantDigits(number / 1e6, 4).ToString() + "m";
            case 9: case 10: case 11:
                return RoundToSignificantDigits(number / 1e9, 4).ToString() + "b";
        }
        return number.ToString();
    }

    public static float RoundToSignificantDigits(double input, int digits) { // Converts an int to a certain amount of significant figures
        if (input.ToString().Length < digits + 1) {
            return (float) input;
        }
        // Split the string at a certain amount of characters (at digits + 1)
        return float.Parse(input.ToString().Substring(0, digits + 1), CultureInfo.InvariantCulture.NumberFormat);
    }
}
