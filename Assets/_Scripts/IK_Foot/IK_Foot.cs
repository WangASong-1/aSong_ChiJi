using UnityEngine;
using System.Collections;

public class IK_Foot : MonoBehaviour {
    public float FootOffsetY;
    public Transform target;

    Vector3 rightFootPos;
    Vector3 leftFootPos;

    float leftFootWeight;
    float rightFootWeight;


    Quaternion rightFootRot;
    Quaternion leftFootRot;

    private Animator mAnim;

    private Transform leftFoot;
    private Transform rightFoot;

    private bool isLeftFootIK;
    private bool isRightFootIK;


    private void Awake()
    {
        mAnim = GetComponent<Animator>();
        leftFoot = mAnim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = mAnim.GetBoneTransform(HumanBodyBones.RightFoot);

    }


    void Update () {
        Vector3 lPos = leftFoot.position;
        RaycastHit leftHit;
        isLeftFootIK = false;
        if (Physics.Raycast(lPos + Vector3.up*0.5f, -transform.up, out leftHit, 1))
        {
            leftFootPos = Vector3.Lerp(lPos, leftHit.point + FootOffsetY * Vector3.up, Time.deltaTime*10f); 
            leftFootRot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
            Debug.DrawLine(lPos + Vector3.up * 0.5f, leftHit.point, Color.red);
            isLeftFootIK = true;
        }

        Vector3 rPos = rightFoot.position;
        RaycastHit rightHit;
        isRightFootIK = false;
        if (Physics.Raycast(rPos + Vector3.up * 0.5f, -transform.up, out rightHit, 1))
        {
            rightFootPos = Vector3.Lerp(rPos, rightHit.point + FootOffsetY * Vector3.up, Time.deltaTime * 10f);
            //  * transform.rotation 是为了保持脚的方向跟身体的方向一致
            rightFootRot = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
            Debug.DrawLine(rPos + Vector3.up * 0.5f, rightHit.point, Color.red);
            isRightFootIK = true;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        mAnim.SetLookAtWeight(1);
        mAnim.SetLookAtPosition(target.position);

        
        leftFootWeight = mAnim.GetFloat("leftFootWeight");
        mAnim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        mAnim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos);
        mAnim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        mAnim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRot);
        

        
        rightFootWeight = mAnim.GetFloat("rightFootWeight");

        mAnim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        mAnim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos);
        mAnim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        mAnim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
       

    }
}
