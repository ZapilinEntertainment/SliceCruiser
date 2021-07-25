using UnityEngine;

public sealed class RespawnFlower : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool active = false;
    private static RespawnFlower previousFlower;
    public static string ANIMATION_ACTIVATED_PROPERTY_NAME { get { return "activated"; } }
    public static string ANIMATION_OPENED_PROPERTY_NAME { get { return "opened"; } }
    private static string OPEN_STATE_NAME { get { return "Open"; } }
    private static string CLOSED_STATE_NAME { get { return "Close"; } }

    private void Start()
    {
        GameMaster.currentSession.SubscribeToSceneClosingEvent(this.SceneClosingEvent);
    }
    private void SceneClosingEvent()
    {
        GameMaster.currentSession?.UnsubscribeFromSceneClosingEvent(this.SceneClosingEvent);
        previousFlower = null;
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
