using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitalisePrefab : MonoBehaviour {
	public GameObject ocean; // For the ocean
	public GameObject seafloor; // For the sea floor
	public List<xzCoordinates> centres = new List<xzCoordinates>();
	public Dictionary<xzCoordinates, GameObject[]> prefabs = new Dictionary<xzCoordinates, GameObject[]>();
	public Dictionary<xzCoordinates, GameObject[]> seaFloorPrefabs = new Dictionary<xzCoordinates, GameObject[]>();
	public float waveHeightTarget = 0;
	public static InitalisePrefab instance;
	public bool runningDecreasing = false;
	public bool runningIncreasing = false;

	void Awake() {
		instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		ocean.GetComponent<WaveManager>().amplitude = 0; // Initalise the prefab information
		ocean.GetComponent<WaveManager>().offset = 0;
		for (int x = -2; x <= 2; x++) { // Initially spawn a 80x80 square 
			for (int z = -2; z <= 2; z++) {
				spawnBigSquare(20*x, 20*z);
				centres.Add(new xzCoordinates(20*x, 20*z));
			}
		}
		Invoke("checkNearestCentres", 0.25f);
		Invoke("destoryFarCentres", 0.375f); // add 125ms later to reduce lag
	}
	
	// CLEAN THIS UP
	public void ManagePrefabs() { // In this case, the ampltidue of the waves are decreasing from 1 to 0
		GameObject[] components = GameObject.FindGameObjectsWithTag("Ocean"); // Find ALL the ocean prefabs (using tags)
		if (ocean.GetComponent<WaveManager>().amplitude > waveHeightTarget && runningDecreasing) { // Decreasing
			Debug.Log("DECREASING!!!");
			if (ocean.GetComponent<WaveManager>().amplitude > waveHeightTarget) { 
				foreach (GameObject component in components) {
					component.GetComponent<WaveManager>().amplitude -= 0.05f;
				}
				ocean.GetComponent<WaveManager>().amplitude -= 0.05f;
				if (ocean.GetComponent<WaveManager>().amplitude > waveHeightTarget) { 
					Invoke("ManagePrefabs", 1);
				} else {
					foreach (GameObject component in components) {
						component.GetComponent<WaveManager>().amplitude = waveHeightTarget;
					}
					ocean.GetComponent<WaveManager>().amplitude = waveHeightTarget;
					runningDecreasing = false;
				}
			} else {
				foreach (GameObject component in components) {
					component.GetComponent<WaveManager>().amplitude = waveHeightTarget;
				}
				ocean.GetComponent<WaveManager>().amplitude = waveHeightTarget;
				runningDecreasing = false;
			}
		} else if (ocean.GetComponent<WaveManager>().amplitude < waveHeightTarget && runningIncreasing) { // Increasing
			Debug.Log("INCREASING!");
			if (ocean.GetComponent<WaveManager>().amplitude < waveHeightTarget) { 
				foreach (GameObject component in components) {
					component.GetComponent<WaveManager>().amplitude += 0.05f;
				}
				ocean.GetComponent<WaveManager>().amplitude += 0.05f;
				if (ocean.GetComponent<WaveManager>().amplitude < waveHeightTarget) { 
					Invoke("ManagePrefabs", 1);
				} else {
					foreach (GameObject component in components) {
						component.GetComponent<WaveManager>().amplitude = waveHeightTarget;
					}
					ocean.GetComponent<WaveManager>().amplitude = waveHeightTarget;
					runningIncreasing = false;
				}
			} else {
				foreach (GameObject component in components) {
					component.GetComponent<WaveManager>().amplitude = waveHeightTarget;
				}
				ocean.GetComponent<WaveManager>().amplitude = waveHeightTarget;
				runningIncreasing = false;
			}
		}
	}

	// Update is called once per frame
	void Update() {
		float waveOffset = WaveManager.instance.GetOffset(); 
		ocean.GetComponent<WaveManager>().offset = waveOffset; // Set the wave offset of the prefab to the wave offset of the active prefabs
	}

	// Method for spawning a 2x2 prefab square (4 prefabs)
	void spawnBigSquare(int centreX, int centreZ) {
		GameObject[] components = new GameObject[4];
		GameObject[] seafloorcomponents = new GameObject[4];
		int i = 0;
		for (int x = -1; x <= 1; x+=2) {
			for (int z = -1; z <= 1; z+=2) {
				GameObject component = Instantiate(ocean, new Vector3(centreX+5*x, 0, centreZ+5*z), Quaternion.identity);
				components[i] = component; // Add to an array of all of the prefabs in this square
				GameObject seafloorcomponent = Instantiate(seafloor, new Vector3(centreX+5*x, -3, centreZ+5*z), Quaternion.identity);
				seafloorcomponents[i] = seafloorcomponent;
				i++;
			}
		}
		prefabs.Add(new xzCoordinates(centreX, centreZ), components); // Add information about the big square to the dictionary
		seaFloorPrefabs.Add(new xzCoordinates(centreX, centreZ), seafloorcomponents);
	}

	// Method for destroying squares more than x units away from the boat
	void destoryFarCentres() {
		GameObject boat = GameObject.Find("Boat"); 
		Vector2 boatXZ = new Vector2(boat.transform.position.x, boat.transform.position.z);
		destroyCentres(boatXZ, prefabs); // Ocean prefabs
		destroyCentres(boatXZ, seaFloorPrefabs); // Sea floor
		Invoke("destoryFarCentres", 0.25f); // Recur the method
	}

	void destroyCentres(Vector2 boatCoordinates, Dictionary<xzCoordinates, GameObject[]> prefabsToDestory) {
		List<xzCoordinates> toRemove = new List<xzCoordinates>();
		foreach (var item in prefabsToDestory) { // Iterate over the prefabs
			if (Vector2.Distance(boatCoordinates, new Vector2(item.Key.x, item.Key.z)) > 60) {	
				toRemove.Add(item.Key); // remove this after (otherwise will cause issues if iterating while removing)
				for (int i = 0; i < item.Value.Length; i++) {
					Destroy(item.Value[i]); // Destory the prefabs (of the big square) if the big square is more than x units away from the 
				}
			}
		}
		foreach (xzCoordinates key in toRemove) { // Remove the key from the dictionary and the array
			prefabsToDestory.Remove(key);
			int index = 0;
			foreach (xzCoordinates centre in centres) {
				if (centre.x == key.x && centre.z == key.z) { // .Contains() won't work here as I am comparing custom constructors
					centres.RemoveAt(index);
					break;
				}
				index++;
			}
		}
	}

	void checkNearestCentres() {
		GameObject boat = GameObject.Find("Boat");
		float boatX = boat.transform.position.x;
		float boatZ = boat.transform.position.z;
		int normalisedX = (int) Mathf.Round(boatX/20); // Divide by 20 as the dimensions of the big square are 20*20
		int normalisedZ = (int) Mathf.Round(boatZ/20);
		for (int x = (normalisedX-2); x <= (normalisedX+2); x++) {
			for (int z = (normalisedZ-2); z <= (normalisedZ+2); z++) {
				int centreX = 20*x;
				int centreZ = 20*z;
				if (twoDimensionalDistance(boatX, boatZ, centreX, centreZ) < 50) { // If the boat is less than 40 units from the centre initalise the square 
					bool contains = false;
					foreach (xzCoordinates centre in centres) { // .Contains() will not work here as I am comparing custom constructors
						if (centre.x == centreX && centre.z == centreZ) {
							contains = true;
							break;
						}
					}
					if (!contains) { // If the prefab isn't already initalised, initalise it
						centres.Add(new xzCoordinates(centreX, centreZ));
						spawnBigSquare(centreX, centreZ);
					}
				}
			}
		}
		Invoke("checkNearestCentres", 0.25f); // Recur the method
	}

	// Method for calcualting the distance between two (x, z) coordinates
	public float twoDimensionalDistance(float firstX, float firstZ, float secondX, float secondZ) {
		return Mathf.Sqrt(Mathf.Pow((firstX-secondX), 2) + Mathf.Pow((firstZ-secondZ), 2)); 
	}
}

// Class for xz coordinates with constructor
public class xzCoordinates {
	public xzCoordinates(int thisX, int thisZ) {
		x = thisX;
		z = thisZ;
	}
	public int x;
	public int z;
}