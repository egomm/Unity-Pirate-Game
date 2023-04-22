using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject player;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start() {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate() {
        transform.position = player.transform.position + offset;
        transform.eulerAngles = new Vector3(90, player.transform.rotation.eulerAngles.y, 0);
    }
}
