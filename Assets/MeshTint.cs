using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshTint : MonoBehaviour
{
    public MeshFilter filter;

    public List<Color> MeshColors = new List<Color>();
    public List<int> Meshes = new();

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
		Meshes = filter?.mesh.triangles.ToList();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
