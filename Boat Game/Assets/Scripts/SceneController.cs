using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    public void LoadGameScene() {
        if (SceneManager.GetActiveScene().name != "Game") {
            Debug.Log("Loading Game");
            SceneManager.LoadScene("Game");
        }
    }

    public void LoadMenuScene() {
        Debug.Log("Loading Main Menu");
        SceneManager.LoadScene("Main Menu");
    }
}
