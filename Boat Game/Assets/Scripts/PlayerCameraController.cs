using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
    private Vector3 offset = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start() {
        test();
    }

    void test() {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0) {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            offset = transform.position - player.transform.position;
        } else {
            Invoke("test", 0.05f);
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0 && offset != null) {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            transform.position = player.transform.position + new Vector3(0, 10, 0);
            transform.eulerAngles = new Vector3(90, player.transform.rotation.eulerAngles.y, 0);
            /*transform.position = player.transform.position + offset;
            transform.eulerAngles = new Vector3(90, player.transform.rotation.eulerAngles.y, 0);*/
        }
    }
}
