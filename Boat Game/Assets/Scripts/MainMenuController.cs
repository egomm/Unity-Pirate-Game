using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    public Button playButton;
    public Button informationButton;
    // Start is called before the first frame update
    void Start() {
        // Return to the default values (player may be replaying the game)
        PlayerManager.playerHealth = 100;
        PlayerManager.goldLooted = 0;
        PlayerManager.piratesKilled = 0;
        IslandManager.islandCoordinates.Clear();
        IslandManager.islandInformation.Clear();
        IslandManager.activePirates.Clear();
        // Reset all of the starting coordinates/angle
        IslandManager.startingCoordinates = new Vector3(0, 0, 0);
        IslandManager.startingAngle = new Vector3(0, 0, 0);
        IslandManager.currentCentre = new Vector3(0, 0, 0);
        PirateShipManager.pirateShipCoordinates.Clear();
        PirateShipManager.pirateShipInformation.Clear();
        PirateShipManager.activePirates.Clear();

        // Button Manager
        Button play = playButton.GetComponent<Button>();
        play.onClick.AddListener(LoadStartScene);
        Button information = informationButton.GetComponent<Button>();
        information.onClick.AddListener(LoadInformationScene);
    }

    public void LoadStartScene() {
        if (SceneManager.GetActiveScene().name != "Starting Scene") {
            SceneManager.LoadScene("Starting Scene");
        }
    }

    public void LoadGameScene() {
        if (SceneManager.GetActiveScene().name != "Game") {
            SceneManager.LoadScene("Game");
        }
    }

    public void LoadInformationScene() {
        if (SceneManager.GetActiveScene().name != "Information Scene") {
            SceneManager.LoadScene("Information Scene");
        }
    }
}
