using UnityEngine;

public class TrackInitializer : MonoBehaviour
{
    void Start()
    {
        TrackConstructor.BuildTrack(transform, 10, 1000f, 150f, new Vector3(0.3f, 0.3f, 0.1f));
    }

}
