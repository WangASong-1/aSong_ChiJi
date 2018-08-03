using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using RootMotion.FinalIK;

//武器挂在骨骼下面在拿取的时候因为会随着骨骼动，导致武器位移，导致最后抓取拿到手上的位置不是我们预定好的位置。要么不挂在骨骼下面，要么抓取的时候parent = null
//需要动画做基础,FinalIK只是在动画上修整.比如双手持枪,就无法做到待机之类的.顶多固定到那一个点上.
public class aSong_UserControlInteractions : UserControlThirdPerson
{
    [SerializeField] CharacterThirdPerson character;
    [SerializeField] InteractionSystem interactionSystem; // Reference to the InteractionSystem of the character
    [SerializeField] bool disableInputInInteraction = true; // If true, will keep the character stopped while an interaction is in progress
    public float enableInputAtProgress = 0.8f; // The normalized interaction progress after which the character is able to move again

    private Animator animator;
    [SerializeField]
    private Transform[] weaponPoint;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;
    }

    protected override void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            animator.SetLayerWeight(1, 0);
            //InteractionObject obj = interactionSystem.GetInteractionObject(FullBodyBipedEffector.RightHand);
            interactionSystem.StopInteraction(FullBodyBipedEffector.RightHand);
            interactionSystem.StopInteraction(FullBodyBipedEffector.LeftHand);
            if (obj != null)
            {
                obj.targetsRoot.parent = weaponPoint[0];
                obj.targetsRoot.localPosition = Vector3.zero;
                obj.targetsRoot.localEulerAngles = Vector3.zero;

            }
            var poser = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<Poser>();
            if (poser != null)
            {
                poser.poseRoot = null;
                poser.weight = 0f;
            }
            poser = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<Poser>();
            if (poser != null)
            {
                poser.poseRoot = null;
                poser.weight = 0f;
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            //InteractionObject obj = interactionSystem.GetInteractionObject(FullBodyBipedEffector.RightHand);
            
          
            if (obj != null)
            {
                interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, obj, false);
                interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, obj, false);
                animator.SetLayerWeight(1, 1);

            }


        }
        // Disable input when in interaction
        if (disableInputInInteraction && interactionSystem != null && (interactionSystem.inInteraction || interactionSystem.IsPaused()))
        {

            // Get the least interaction progress
            float progress = interactionSystem.GetMinActiveProgress();

            // Keep the character in place
            if (progress > 0f && progress < enableInputAtProgress)
            {
                state.move = Vector3.zero;
                state.jump = false;
                return;
            }
        }

        // Pass on the FixedUpdate call
        base.Update();

        
    }


    public InteractionObject obj;
    // Triggering the interactions
    void OnGUI()
    {
        // If jumping or falling, do nothing
        if (!character.onGround) return;

        // If an interaction is paused, resume on user input
        if (interactionSystem.IsPaused() && interactionSystem.IsInSync())
        {
            GUILayout.Label("Press E to resume interaction");

            if (Input.GetKey(KeyCode.E))
            {
                interactionSystem.ResumeAll();
            }

            return;
        }

        // If not paused, find the closest InteractionTrigger that the character is in contact with
        int closestTriggerIndex = interactionSystem.GetClosestTriggerIndex();

        InteractionObject[] objs = interactionSystem.GetClosestInteractionObjectsInRange();
        //objs 有一个 model,保存他的 图片,然后将图片获取 放到 control 上 显示到 view(滑条)中,并添加点击事件，点击后拾取,出范围后 remove view中按钮
        for(int i = 0; i < objs.Length; i++)
        {
            
        }

        // ...if none found, do nothing
        if (closestTriggerIndex == -1) return;

        // ...if the effectors associated with the trigger are in interaction, do nothing
        if (!interactionSystem.TriggerEffectorsReady(closestTriggerIndex)) return;

        // Its OK now to start the trigger
        GUILayout.Label("Press E to start interaction");

        if (Input.GetKey(KeyCode.E))
        {
            interactionSystem.TriggerInteraction(closestTriggerIndex, false);
            animator.SetLayerWeight(1, 1);
            obj = interactionSystem.GetInteractionObject(FullBodyBipedEffector.RightHand);
            obj.targetsRoot.GetComponentInChildren<InteractionTrigger>().enabled = false;
        }
       
    }

    
}
