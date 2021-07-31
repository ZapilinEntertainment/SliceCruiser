using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackConstructor
{
    /// <summary>
    /// curvatury from 0 to 1, where 1 is 90 degrees angle; z means up vector change
    /// </summary>
    public static void BuildTrack(Transform startTransform, int sectionsCount, float maxSectionLength, float width, Vector3 curvature)
    {

        float GetSectionLength() { return maxSectionLength * (0.75f + 0.25f * Random.value); }        
       
        float l;
        RoadPoint startPoint = new RoadPoint(startTransform);
        Vector3 middlePos = Vector3.zero;
        Vector3 GetOffset() {
            var rightShift =  (Quaternion.AngleAxis(90f, startPoint.up) * startPoint.forward) * (0.5f - Random.value) * 2f * curvature.x * l;
            var upShift = startPoint.up * (0.5f - Random.value) * 2f * curvature.y * l;
            return rightShift + upShift;
        }
        Vector3 upvector; 
        RoadPoint GetEndPoint()
        {
            l = GetSectionLength();
            middlePos = startPoint.position + startPoint.forward * l;
            upvector = startPoint.up;
            if (curvature.z != 0f) upvector = Quaternion.AngleAxis(180f * (0.5f - Random.value), startPoint.forward) * startPoint.up;
            return new RoadPoint(
                    middlePos + GetOffset(),
                    upvector,
                    startPoint.forward
                    );
        }

        var endPoint = GetEndPoint();
        Vector3 fwd=BridgeConstructor.CreateBridge(startPoint, middlePos, endPoint , startTransform, width);
        for (int i = 0; i < sectionsCount - 1; i++)
        {
            startPoint = new RoadPoint(endPoint.position, endPoint.up, fwd);
            // middlepos fills in GetEndPoint
            endPoint = GetEndPoint();
            fwd = BridgeConstructor.CreateBridge(startPoint, middlePos, endPoint, startTransform, width);
        }
    }

    public static void BuildTrack(Transform startTransform, Transform endTransform, float maxSectionsLength, float width, float curvature)
    { 
        var dir = endTransform.position - startTransform.position;
        float x = 0f, totalLength = dir.magnitude, distanceLeft = totalLength, l;
        while (distanceLeft >= maxSectionsLength)
        {
            l = maxSectionsLength * (0.65f + Random.value * 0.35f);
            distanceLeft -= l;

            x = 1f - distanceLeft / totalLength;

        }
    }
}
