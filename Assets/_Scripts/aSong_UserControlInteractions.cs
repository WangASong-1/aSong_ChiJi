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

        
        //开枪
        MakeFire();
    }

    void MakeFire()
    {
        if (Input.GetMouseButtonDown(0) && currentProp)
        {
            currentProp.Using();
        }
    }


    //控制器只负责手上的道具,手上有道具时该丢的丢该放背包的放背包
    void PutCurrentInBag()
    {
        animator.SetLayerWeight(1, 0);
        interactionSystem.StopInteraction(FullBodyBipedEffector.RightHand);
        interactionSystem.StopInteraction(FullBodyBipedEffector.LeftHand);

        
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

    void PutPropInBag(PropBaseModel prop)
    {
        prop.transform.parent = weaponPoint[0];
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localEulerAngles = Vector3.zero;
    }

    public bool PickupProp(PropBaseModel current, PropBaseModel model, bool b_propFromBag = false)
    {
        if (interactionSystem.IsInInteraction(FullBodyBipedEffector.RightHand))
        {
            //Debug.LogError("我在动画中不方便拿东西");
            return false;
        }

        bool b_IsBagFull = false;
        bool b_needGrab = true;
        currentProp = model;
        if (b_propFromBag)
        {
            //从背包里拿出来的道具,那么直接将手上的道具放背包去
            if (current != null)
            {
                PutPropInBag(current);
                PutCurrentInBag();
                b_needGrab = true;
            }
            //这个是要拿的东西为空,说明只是打算把手上的道具收回背包，直接返回
            if (model == null)
            {
                return true;
            }
        }

        b_IsBagFull = aSongUI_Controller.Instance.playerData.IsBagFull(model);

        model.Pickup(gameObject);
        if (!b_IsBagFull)
        {
            //背包没满
            if (current != null)
            {
                //手上有道具，但是背包没满,直接放入背包
                PutPropInBag(model);
                b_needGrab = false;
                model = null;
            }
        }
        else
        {
            //背包满了
            if (!b_propFromBag)
            {
                //从地上捡起来道具，丢掉手上的
                current.Discard();
            }
           
        }

        if (b_needGrab)
        {
            InteractionObject _obj = model.mInteractionObject;
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
        }
        return true;
    }
}
