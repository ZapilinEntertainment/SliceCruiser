using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_SquareRotator : MonoBehaviour
{
    private Quaternion a, b;

    private void Start()
    {
        a = transform.rotation;
        b = Quaternion.LookRotation(Vector3.forward, (Vector3.up + new Vector3(Random.value * 0.2f, 0f, Random.value * 0.2f)).normalized);
    }

    void Update()
    {
        if (transform.rotation != b)
            transform.rotation = Quaternion.Lerp(transform.rotation, b, Mathf.Sin(Time.time) * 0.02f);
        else
        {
            b = a;
            a = transform.rotation;
        }
    }
}
