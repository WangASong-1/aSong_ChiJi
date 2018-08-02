using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using RootMotion.FinalIK;

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
