using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class WaterManager : MonoBehaviour {
    private MeshFilter meshFilter;

    // Awake is called before Start
    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame - Late update is called after update
    void LateUpdate() {
        Vector3[] vertices = meshFilter.mesh.vertices; // Get the vertices of the mesh
        for (int i = 0; i < vertices.Length; i++) {
            if (WaveManager.instance != null) {
                // Iterate over the vertices and set the y position to the current wave height at this position
                vertices[i].y = WaveManager.instance.GetWaveHeight(transform.position.z + vertices[i].z);  
            }
        }
        meshFilter.mesh.vertices = vertices;
        // updates the normals based on the current geometry of the mesh, ensuring that lighting effects and shading appear correctly on the rendered surface
        meshFilter.mesh.RecalculateNormals();
    }
}
