using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour
{
    private static float t;

    public static float GetWaveHeight(float x, float z)
    {
        return Mathf.Sin(x * 80f + z * 80f + t);
    }
    public static float GetWaveHeight(Transform tt)
    {
        var p = tt.position;
        return Mathf.Sin(p.x * 80f + p.z * 80f + t);
    }

    private void Update()
    {
        t = Time.time;
    }
}
