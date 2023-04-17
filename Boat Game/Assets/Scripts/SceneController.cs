using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Start");
        //Invoke("LoadGameScene", 2);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void LoadGameScene() {
        Debug.Log("HI!");
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
