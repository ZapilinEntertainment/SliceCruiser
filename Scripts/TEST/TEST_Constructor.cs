using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Constructor : MonoBehaviour
{
    [SerializeField] private Vector3 startPoint, endPoint;
    [SerializeField] private Material mat;
    [SerializeField] private GameObject player;
    [SerializeField] [Range(0.1f, 1f)]  private float integrity = 1f;
    [SerializeField] [Range(1f, 120f)] private float wallsAngle = 30f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3Int scale = new Vector3Int(200, 10, 200);
        var dir = (endPoint - startPoint);
        int length = Mathf.CeilToInt(dir.magnitude);
        dir.Normalize();
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.tag = GameMaster.waterTag;
        g.layer = LayerMask.NameToLayer("Water");
        g.transform.rotation = Quaternion.LookRotation(dir);
        g.transform.localScale = scale;
        var mr = g.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //
        Transform host = new GameObject("road host").transform;
        host.position = Vector3.zero;
        host.rotation = Quaternion.identity;
        //
        bool integrityCheck = integrity != 1f ;
        Transform t;
        Vector3 pos, pos2;
        Quaternion rot, rot2;
        float angle = (90f - wallsAngle / 2f);
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * scale.x * 0.95f, 
            y = Mathf.Cos(angle * Mathf.Deg2Rad) * scale.x * 0.95f;
        for (int i = 0; i < length; i += scale.z)
        {
            pos = startPoint + dir * i;
            rot = g.transform.rotation;
            if (!integrityCheck || Random.value <= integrity) Instantiate(g, pos, rot, host);
            //
            if (!integrityCheck || Random.value <= integrity) {
                t = Instantiate(g, host).transform;
                t.rotation = rot;
                pos2 = pos + t.TransformDirection(x, y, 0f);
                t.position = pos2;
                t.Rotate(Vector3.forward * wallsAngle, Space.Self);
                rot2 = t.rotation;
                if (!integrityCheck || Random.value <= integrity)
                {
                    t = Instantiate(g, host).transform;
                    t.rotation = rot2;
                    t.position = pos2 + t.TransformDirection(x, y, 0f);
                    t.Rotate(Vector3.forward * wallsAngle, Space.Self);
                }
            }           
            //
            if (!integrityCheck || Random.value <= integrity)
            {
                t = Instantiate(g, host).transform;
                t.rotation = rot;                
                pos2 = pos + t.TransformDirection(-x, y,0f);
                t.position = pos2;
                t.Rotate(Vector3.back * wallsAngle, Space.Self);
                rot2 = t.rotation;
                if (!integrityCheck || Random.value <= integrity)
                {
                    t = Instantiate(g, host).transform;
                    t.rotation = rot2;
                    t.position = pos2 + t.TransformDirection(-x, y, 0f);
                    t.Rotate(Vector3.back * wallsAngle, Space.Self);
                }
            }
        }

        if (player != null)
        {
            player.transform.position = startPoint + Vector3.up * 5;
            player.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }


    private void CreateSquare(GameObject pref)
    {
        GameObject g;
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                g = Instantiate(pref, new Vector3(-9900 + i * 200, 0f, -9900 + j * 200),
                    Quaternion.Euler(Random.value * 8f - 4f, 0f, Random.value * 8f - 4f));
                g.AddComponent<TEST_SquareRotator>();
            }
        }
    }
    
}
