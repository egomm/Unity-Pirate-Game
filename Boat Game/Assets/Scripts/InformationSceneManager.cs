using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InformationSceneManager : MonoBehaviour {
    public Button mainMenuButton;
    // Start is called before the first frame update
    void Start() {
        Button play = mainMenuButton.GetComponent<Button>();
        play.onClick.AddListener(LoadMainMenu);
    }

    public void LoadMainMenu() {
        if (SceneManager.GetActiveScene().name != "Main Menu") {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
