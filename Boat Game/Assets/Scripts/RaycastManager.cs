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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Creates a ray from the from the camera through a screen point (in this case the mouse position)
            if (Physics.Raycast(ray, out hit)) { // using out because hit is for output only 
                string activeScene = SceneManager.GetActiveScene().name;
                float hitDistance = Vector3.Distance(hit.transform.position, rb.transform.position);
                string hitTag = hit.transform.tag;
                if (hitDistance < 4 && hitTag == "Boat" && transform.position.y > hit.transform.position.y && activeScene != "Game") {
                    Debug.Log("Loading Game");
                    SceneManager.LoadScene("Game");
                    /*MeshRenderer renderer = hit.transform.GetComponent<MeshRenderer>();
                    Vector3 size = renderer.bounds.size;
                    Debug.Log(size);*/ // Use when more boats are added
                }
                if (hitTag == "Dock" && activeScene == "Game") {
                    Debug.Log("Hit Dock!");
                    MeshRenderer renderer = hit.transform.GetComponent<MeshRenderer>();
                    Vector3 size = renderer.bounds.size;
                    Vector3 extents = renderer.bounds.extents;
                    Vector3 centre = renderer.bounds.center;
                    Debug.Log(size);
                    Debug.Log(extents);
                    Debug.Log(centre);
                }
            }
        }
    }
}
