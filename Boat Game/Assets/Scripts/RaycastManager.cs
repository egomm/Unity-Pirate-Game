using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastManager : MonoBehaviour {

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(1)) { // If right click is pressed
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) { // using out as hit is for output only 
                if (Vector3.Distance(hit.transform.position, rb.transform.position) < 4 && hit.transform.name == "Boat") {
                    if (SceneManager.GetActiveScene().name != "Game") {
                        Debug.Log("Loading Game");
                        SceneManager.LoadScene("Game");
                    }
                    /*MeshRenderer renderer = hit.transform.GetComponent<MeshRenderer>();
                    Vector3 size = renderer.bounds.size;
                    Debug.Log(size);*/ // Use when more boats are added
                }
            }
        }
    }
}
