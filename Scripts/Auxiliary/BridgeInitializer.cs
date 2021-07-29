using System.Collections;
using UnityEngine;

public class BridgeInitializer : MonoBehaviour
{
    [SerializeField]private float maxRough = 1f, width = 30f;
    // Start is called before the first frame update
    void Start()
    {
        int l = transform.childCount;
        if (l > 1)
        {
            float x, distance;
            Vector3 bendPoint,a,b, bendFwd, bendUp;
            Transform t1, t2;
            for (int i = 0; i < l - 1; i++)
            {
                t1 = transform.GetChild(i);
                t2 = transform.GetChild(i+1);
                a = t1.position; b = t2.position;
                distance = Vector3.Distance(a, b);
                x = 0.2f + Random.value * 0.6f;
                bendPoint = Vector3.Lerp(a, b, x);
                bendFwd = Vector3.Lerp(t1.forward, t2.forward, x);
                bendUp = Vector3.Lerp(t1.up, t2.up, x);
                bendPoint += (Quaternion.AngleAxis(Random.value * 360f, bendFwd) * bendUp) * (distance * (Random.value * 0.8f + 0.2f) * maxRough);

                BridgeConstructor.CreateBridge(new RoadPoint(t1), new RoadPoint(bendPoint,bendUp, bendFwd), new RoadPoint(t2), transform, width);
            }
        }
        
    }
}
