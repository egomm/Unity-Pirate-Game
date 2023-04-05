using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitalisePrefab : MonoBehaviour
{

    public GameObject water;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = -5; x < 5; x++) {
            for (int z = -5; z < 5; z++) {
                Instantiate(water, new Vector3(10*x, 0, 10*z), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
