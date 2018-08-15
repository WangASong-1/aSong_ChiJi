using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using RootMotion.FinalIK;
using TinyTeam.UI;

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

    private aSongUI_Contact mContact;

    [SerializeField]
    private PropBaseModel currentProp;

    private void Start()
    {
        aSongUI_Controller.Instance.mUserCtrl = this;
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;
        mContact = GetComponent<aSongUI_Contact>();
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
            //aSongUI_Controller.Instance.AddProp(objs[i].GetComponentInParent<PropBaseModel>().prop) ;
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

    void PutInBag()
    {
        if (aSongUI_Controller.Instance.playerData.GetGunNum() >= 2)
        {
            if (currentProp != null)
            {
                currentProp.Discard();
            }
        }

    }

    public bool PickupProp(PropBaseModel model)
    {
        if (interactionSystem.IsInInteraction(FullBodyBipedEffector.RightHand))
            return false;

        bool b_Discard = false;
        switch (model.prop.type)
        {
            case PropType.pistol:
                if (aSongUI_Controller.Instance.playerData.Pistol != null)
                    b_Discard = true;
                break;
            case PropType.rifle:
                if (aSongUI_Controller.Instance.playerData.GetGunNum() >= 2)
                    b_Discard = true;
                break;
            case PropType.bomb:
                    b_Discard = false;
                break;
            case PropType.other:
                break;
        }
        if (b_Discard)
        {
            currentProp.Discard();
        }
       
        InteractionObject _obj = model.mInteractionObject;
        model.Pickup();
        interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, _obj, false);
        interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, _obj, false);
        if (model.prop.name == PropName.M416)
        {
            
            animator.SetLayerWeight(1, 1);
        }
        else
        {
            animator.SetLayerWeight(1, 0);

        }

        currentProp = model;
        return true;
    }

    //丢掉指定道具,为空就是丢掉手上道具
    public void Discard(PropBaseModel model = null)
    {
        if(currentProp == model || (currentProp!=null && model==null))
        {
            currentProp = null;
            animator.SetLayerWeight(1, 0);
            interactionSystem.StopInteraction(FullBodyBipedEffector.RightHand);
            interactionSystem.StopInteraction(FullBodyBipedEffector.LeftHand);

        }
        model.Discard();

    }
}
