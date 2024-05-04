using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeRotating : MonoBehaviour
{
    public Transform transform;

    public Vector3 rotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation);
    }
}
