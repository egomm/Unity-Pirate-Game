using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSceneGameManager : MonoBehaviour {

    public GameObject island;
    public GameObject player;
    // Start is called before the first frame update
    void Start() {
        Instantiate(island, new Vector3(250, 0, 250), Quaternion.identity);
        Instantiate(player, new Vector3(250, 2, 250), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
