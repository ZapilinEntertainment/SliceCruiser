using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BridgeConstructor 
{
    private static readonly string surfaceTag = "EnergySource";
    private static readonly int surfaceLayer = LayerMask.NameToLayer("Water");
    private static readonly Material surfaceMaterial = Resources.Load<Material>("Materials/Water");


    public static void CreateBridge(RoadPoint A, RoadPoint B, RoadPoint C, Transform host, float averageWidth)
    {
        float d1 = (C.position - B.position).magnitude, d2 = (B.position - A.position).magnitude;

        int sections = (int)(d1 + d2);
        float p = 1f / sections, x = p;
        Vector3 nextPoint, pos, u1, u2, up;

        GameObject g;
        Transform t;
        Vector3 previousPoint = A.position, p1, p2;
        Renderer rr;
        for (int i = 1; i < sections; i++)
        {
            p1 = Vector3.Lerp(A.position, B.position, x);
            p2 = Vector3.Lerp(B.position, C.position, x);
            nextPoint = Vector3.Lerp(p1, p2, x);
            //nextPoint = (1 - x * x) * A.position + 2f * x * (1f - x) * B.position + x * x * C.position;
            g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.layer = surfaceLayer;
            g.tag = surfaceTag;
            rr = g.GetComponent<Renderer>();
            rr.sharedMaterial = surfaceMaterial;
            rr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            rr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            rr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            rr.receiveShadows = false;
            t = g.transform;
            t.parent = host;
            
            pos = Vector3.Lerp(previousPoint, nextPoint, 0.5f);
            previousPoint = pos;
            t.position = pos;
            u1 = Vector3.Lerp(A.up, B.up, x);
            u2 = Vector3.Lerp(B.up, C.up, x);
            t.rotation = Quaternion.LookRotation(nextPoint - pos, Vector3.Lerp(u1, u2, x));
            t.localScale = new Vector3(averageWidth * (0.9f + Random.value * 0.2f), 1f, 1f);

            x += p;
        }
    }
}
