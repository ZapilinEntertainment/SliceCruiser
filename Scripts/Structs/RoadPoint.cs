using UnityEngine;

public struct RoadPoint
{
    public Vector3 position, up, forward;
    public RoadPoint(Vector3 i_pos, Vector3 i_up, Vector3 i_fwd)
    {
        position = i_pos;
        up = i_up;
        forward = i_fwd;
    }
    public RoadPoint(Transform t)
    {
        position = t.position;
        up = t.up;
        forward = t.forward;
    }
}
