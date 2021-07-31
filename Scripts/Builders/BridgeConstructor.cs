using UnityEngine;

public abstract class BridgeConstructor 
{
    private static readonly string surfaceTag = "EnergySource";
    private static readonly int surfaceLayer = LayerMask.NameToLayer("Water");
    private static readonly Material surfaceMaterial = Resources.Load<Material>("Materials/Water");

    /// <summary>
    /// returns endPoint forward vector
    /// </summary>
    public static Vector3 CreateBridge(RoadPoint start, Vector3 middlePoint, RoadPoint end, Transform host, float averageWidth)
    {
        float d1 = (end.position - middlePoint).magnitude, d2 = (middlePoint - start.position).magnitude;

        int sections = (int)(d1 + d2);
        float p = 1f / sections, x = p;
        Vector3 nextPoint, pos;

        GameObject g;
        Transform t;
        Vector3 previousPoint = start.position, p1, p2, fwd = end.forward;
        Renderer rr;
        for (int i = 1; i < sections; i++)
        {
            p1 = Vector3.Lerp(start.position, middlePoint, x);
            p2 = Vector3.Lerp(middlePoint, end.position, x);
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
            t.rotation = Quaternion.LookRotation(nextPoint - pos, Vector3.Lerp(start.up, end.up, x));
            t.localScale = new Vector3(averageWidth * (0.9f + Random.value * 0.2f), 1f, 1f);
            fwd = t.forward;
            x += p;
        }
        return fwd;
    }
}
