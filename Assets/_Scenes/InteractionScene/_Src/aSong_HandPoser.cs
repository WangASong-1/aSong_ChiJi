using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class aSong_HandPoser : MonoBehaviour {
    public Transform poseRoot;
    private Animator mAnim;
    private void Awake()
    {
        mAnim = GetComponentInParent<Animator>();
    }

    //必须有Animator的Gameobject上
    private void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("OnAnimatorIK");
        mAnim.SetLayerWeight(0, 1);
        Transform LeftHand = mAnim.GetBoneTransform(HumanBodyBones.LeftHand);
        mAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        mAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        mAnim.SetIKPosition(AvatarIKGoal.LeftHand, poseRoot.position);
        mAnim.SetIKRotation(AvatarIKGoal.LeftHand, poseRoot.rotation);
        
    }
}
