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
                GameObject spawned = Instantiate(water, new Vector3(10*x, 0, 10*z), Quaternion.identity);
            }
        }
        Invoke("ManagePrefabs", 1);
    }
    
    void ManagePrefabs() {
        GameObject component = GameObject.Find("Ocean(Clone)");
        component.GetComponent<WaveManager>().amplitude += 0.05f;
        if (component.GetComponent<WaveManager>().amplitude <= 1) { 
            Invoke("ManagePrefabs", 1);
        } else {
            Debug.Log("Done");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject component = GameObject.Find("Ocean(Clone)");
        //component.GetComponent<WaveManager>().amplitude += Time.deltaTime/10;
    }
}
