using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    public Button playButton;
    // Start is called before the first frame update
    void Start() {
        Button play = playButton.GetComponent<Button>();
        play.onClick.AddListener(LoadStartScene);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void LoadStartScene() {
        if (SceneManager.GetActiveScene().name != "Starting Scene") {
            Debug.Log("Loading Starting Scene");
            SceneManager.LoadScene("Starting Scene");
        }
    }

    public void LoadGameScene() {
        if (SceneManager.GetActiveScene().name != "Game") {
            Debug.Log("Loading Game");
            SceneManager.LoadScene("Game");
        }
    }
}
