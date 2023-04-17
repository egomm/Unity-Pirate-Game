using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitalisePrefab : MonoBehaviour
{

	public GameObject ocean;
	public List<xzCoordinates> centres = new List<xzCoordinates>();
	public Dictionary<xzCoordinates, GameObject[]> prefabs = new Dictionary<xzCoordinates, GameObject[]>();
	// Start is called before the first frame update
	void Start() {
		ocean.GetComponent<WaveManager>().amplitude = 1; // Initalise the prefab information
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
	
	void ManagePrefabs() { // In this case, the ampltidue of the waves are decreasing from 1 to 0
		GameObject component = GameObject.Find("Ocean(Clone)");
		if (component.GetComponent<WaveManager>().amplitude > 0) { 
			component.GetComponent<WaveManager>().amplitude -= 0.05f;
			ocean.GetComponent<WaveManager>().amplitude -= 0.05f;
			if (component.GetComponent<WaveManager>().amplitude > 0) { 
				Invoke("ManagePrefabs", 1);
			} else {
				component.GetComponent<WaveManager>().amplitude = 0;
				ocean.GetComponent<WaveManager>().amplitude = 0;
			}
		} else {
			component.GetComponent<WaveManager>().amplitude = 0;
			ocean.GetComponent<WaveManager>().amplitude = 0;
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
		int i = 0;
		for (int x = -1; x <= 1; x+=2) {
			for (int z = -1; z <= 1; z+=2) {
				GameObject component = Instantiate(ocean, new Vector3(centreX+5*x, 0, centreZ+5*z), Quaternion.identity);
				components[i] = component; // Add to an array of all of the prefabs in this square
				i++;
			}
		}
		prefabs.Add(new xzCoordinates(centreX, centreZ), components); // Add information about the big square to the dictionary
	}

	// Method for destroying squares more than x units away from the boat
	void destoryFarCentres() {
		GameObject boat = GameObject.Find("Boat"); 
		float boatX = boat.transform.position.x;
		float boatZ = boat.transform.position.z;
		List<xzCoordinates> toRemove = new List<xzCoordinates>();
		foreach (var item in prefabs) { // Iterate over the prefabs
			if (twoDimensionalDistance(boatX, boatZ, item.Key.x, item.Key.z) > 50) { 
				toRemove.Add(item.Key); // remove this after (otherwise will cause issues if iterating while removing)
				for (int i = 0; i < item.Value.Length; i++) {
					Destroy(item.Value[i]); // Destory the prefabs (of the big square) if the big square is more than x units away from the boat
				}
			}
		}
		foreach (xzCoordinates key in toRemove) { // Remove the key from the dictionary and the array
			prefabs.Remove(key);
			int index = 0;
			foreach (xzCoordinates centre in centres) {
				if (centre.x == key.x && centre.z == key.z) { // .Contains() won't work here as I am comparing custom constructors
					centres.RemoveAt(index);
					break;
				}
				index++;
			}
		}
		Invoke("destoryFarCentres", 0.25f); // Recur the method
	}

	void checkNearestCentres() {
		GameObject boat = GameObject.Find("Boat");
		float boatX = boat.transform.position.x;
		float boatZ = boat.transform.position.z;
		int normalisedX = (int) Mathf.Round(boatX/20); // Divide by 20 as the dimensions of the big square are 20*20
		int normalisedZ = (int) Mathf.Round(boatZ/20);
		for (int x = (normalisedX-1); x <= (normalisedX+1); x++) {
			for (int z = (normalisedZ-1); z <= (normalisedZ+1); z++) {
				int centreX = 20*x;
				int centreZ = 20*z;
				if (twoDimensionalDistance(boatX, boatZ, centreX, centreZ) < 30) { // If the boat is less than 30 units from the centre initalise the square 
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