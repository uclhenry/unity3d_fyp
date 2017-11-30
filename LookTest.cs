using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTest : MonoBehaviour {
    public Transform target;
    public float x;
    public float y;
    public float z;

    void Update()
    {
        // Point the object at the world origin
        transform.LookAt(new Vector3(x, y, z));
    }//Vector3.zero target 
}
