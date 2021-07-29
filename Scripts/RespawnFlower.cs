using UnityEngine;

public sealed class RespawnFlower : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool active = false;
    private static RespawnFlower previousFlower;
    private static bool subscribedToSceneClosingEvent = false;
    public static string ANIMATION_ACTIVATED_PROPERTY_NAME { get { return "activated"; } }
    public static string ANIMATION_OPENED_PROPERTY_NAME { get { return "opened"; } }
    private static string OPEN_STATE_NAME { get { return "Open"; } }
    private static string CLOSED_STATE_NAME { get { return "Close"; } }



    private void Start()
    {
        if (!subscribedToSceneClosingEvent)
        {
            GameMaster.currentSession.SubscribeToSceneClosingEvent(SceneClosingEvent);
            subscribedToSceneClosingEvent = true;
        }
    }
    private static void SceneClosingEvent()
    {
        GameMaster.currentSession?.UnsubscribeFromSceneClosingEvent(SceneClosingEvent);
        previousFlower = null;
        subscribedToSceneClosingEvent = false;
    }
    public static bool TrySetRespawnPosition(Transform t)
    {
        if (previousFlower != null)
        {
            var r = Random.insideUnitCircle;
            var pft = previousFlower.transform;
            t.position = pft.position + new Vector3(r.x, 0f, r.y) * 50f;
            t.rotation = Quaternion.LookRotation(t.position - pft.position, pft.up);
            return true;
        }
        else return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && !active)
        {
            active = true;
            animator.SetBool(ANIMATION_ACTIVATED_PROPERTY_NAME, true);
            animator.SetBool(ANIMATION_OPENED_PROPERTY_NAME, true);                     
            previousFlower?.Close();
            previousFlower = this;
        }
    }

    private void Close()
    {
        active = false;
        animator.SetBool(ANIMATION_OPENED_PROPERTY_NAME, false);
    }
}
