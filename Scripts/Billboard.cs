using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
   
    void Update()
    {
        if (Camera.main != null)
        {
            var dir = Camera.main.transform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}
