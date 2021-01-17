using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaController : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    private Mesh m;

    private void Awake()
    {
        m = meshFilter.mesh;
        meshFilter.sharedMesh = m;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
