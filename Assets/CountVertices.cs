using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountVertices : MonoBehaviour
{    
    public bool global = false;
    private void OnValidate()
    {
        int vertexCount = 0;
        var renderers = global ? FindObjectsOfType<SkinnedMeshRenderer>() : GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers) vertexCount += r.sharedMesh.vertexCount;
        var meshFilters = global ? FindObjectsOfType<MeshFilter>() : GetComponentsInChildren<MeshFilter>();
        foreach (var filter in meshFilters) vertexCount += filter.sharedMesh.vertexCount;

        print("Vertices: " + vertexCount);
    }
}
