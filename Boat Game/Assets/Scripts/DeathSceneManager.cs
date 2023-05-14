using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour {
    public TextMeshPro goldText;
    public TextMeshPro killText;
    public Button mainMenuButton;
    // Start is called before the first frame update
    void Start() {
        // Set text from data from game manager/player manager
        Button play = mainMenuButton.GetComponent<Button>();
        play.onClick.AddListener(LoadMainMenu);
        // Show the statistics to the player
        goldText.text = "Gold Looted: " + GameManager.AbbreviatedForms(PlayerManager.goldLooted);
        killText.text = "Pirates Killed: " + GameManager.AbbreviatedForms(PlayerManager.piratesKilled);
    }

    public void LoadMainMenu() {
        if (SceneManager.GetActiveScene().name != "Main Menu") {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
