using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotateVector;
    [SerializeField] private Space rotatingSpace;

    private void Update()
    {
        transform.Rotate(rotateVector * Time.deltaTime, rotatingSpace);
    }
}
