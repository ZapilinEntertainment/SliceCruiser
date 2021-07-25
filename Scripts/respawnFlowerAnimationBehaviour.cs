using UnityEngine;
// https://docs.unity3d.com/ScriptReference/StateMachineBehaviour.html

public class respawnFlowerAnimationBehaviour : StateMachineBehaviour
{
    private enum AnimatorStatus : byte { NotUsed, Opening, Opened, Closing, Closed}

    private float prevprogress = 0f; // можно было сделать и через самих анимационных клипов
    private static bool materialsLoaded = false;
    private AnimatorStatus status;
    private static Material closedMaterial, changingMaterial, openedMaterial;
    private static Animator currentAnimatingObject;
    private static string SATURATION_PROPERTY_NAME{get { return "Vector1_CD33CCF9"; }}  

    private static void Prepare()
    {
        GameMaster.currentSession.SubscribeToSceneClosingEvent(SceneClosing);
        closedMaterial = Resources.Load<Material>("Materials/ancientPillar");
        closedMaterial.SetFloat(SATURATION_PROPERTY_NAME, 0f);
        openedMaterial = Resources.Load<Material>("Materials/FlowerMaterial");
        openedMaterial.SetFloat(SATURATION_PROPERTY_NAME, 1f);
        changingMaterial = new Material(openedMaterial);
        changingMaterial.name = "ChangingMaterial";
        materialsLoaded = true;
    }
    private static void SceneClosing()
    {
        GameMaster.currentSession?.UnsubscribeFromSceneClosingEvent(SceneClosing);
        currentAnimatingObject = null;
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool(RespawnFlower.ANIMATION_ACTIVATED_PROPERTY_NAME) == false) return;
        //
        if (!materialsLoaded)
        {
            Prepare();
        }
        //
        if (currentAnimatingObject != null) INLINE_StopOngoingAnimation();
        //
        currentAnimatingObject = animator;
        var open = currentAnimatingObject.GetBool(RespawnFlower.ANIMATION_OPENED_PROPERTY_NAME);
        if (open)
        {
            status = AnimatorStatus.Opening;
            changingMaterial.SetFloat(SATURATION_PROPERTY_NAME, 0f);
            prevprogress = 0f;
        }
        else
        {
            status = AnimatorStatus.Closing;
            changingMaterial.SetFloat(SATURATION_PROPERTY_NAME, 1f);
            prevprogress = 1f;
        }
        var rrs = currentAnimatingObject.GetComponentsInChildren<Renderer>();
        foreach (var r in rrs)
        {
            r.sharedMaterial = changingMaterial;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool opening = status == AnimatorStatus.Opening;
        if (opening || status == AnimatorStatus.Closing)
        {            
            float endval = opening ? 1f : 0f;
            float progress = stateInfo.normalizedTime; // https://docs.unity3d.com/ScriptReference/AnimatorStateInfo-normalizedTime.html
            progress -= (int)progress;
            changingMaterial.SetFloat(SATURATION_PROPERTY_NAME, progress);
            if (opening)
            {
                if (progress < prevprogress)
                {
                    Stop(animator);
                }
            }
            else
            {
                if (progress > prevprogress) Stop(animator);
            }
            prevprogress = progress;
        }
    }

    private void Stop(Animator animator)
    {
            bool opening = status == AnimatorStatus.Opening;
            if (opening || status == AnimatorStatus.Closing)
            {
                var rrs = animator.GetComponentsInChildren<Renderer>();
                Material m;
                if (opening)
                {
                    m = openedMaterial;
                    status = AnimatorStatus.Opened;
                }
                else
                {
                    m = closedMaterial;
                    status = AnimatorStatus.Closed;
                }
                foreach (var r in rrs)
                {
                    r.sharedMaterial = m;   
                }
                if (currentAnimatingObject == animator) currentAnimatingObject = null;
            }
    }

    private void INLINE_StopOngoingAnimation()
    {
        var b = currentAnimatingObject.GetBool(RespawnFlower.ANIMATION_OPENED_PROPERTY_NAME);
        var rrs = currentAnimatingObject.GetComponentsInChildren<Renderer>();
        foreach (var r in rrs)
        {
            r.sharedMaterial = b ? openedMaterial : closedMaterial;
        }
        currentAnimatingObject = null;
    }

    private void INLINE_CheckStatus(Animator animator)
    {
        bool a = animator.GetBool(RespawnFlower.ANIMATION_ACTIVATED_PROPERTY_NAME),
             b = animator.GetBool(RespawnFlower.ANIMATION_OPENED_PROPERTY_NAME);
        if (a == false) status = AnimatorStatus.NotUsed;
        else
        {
            status = b ? AnimatorStatus.Opening : AnimatorStatus.Closing;
        }
    }

}
