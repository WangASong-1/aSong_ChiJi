using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class aSong_GraMP_40 : MonoBehaviour {
    [Tooltip("The object to interact to")]
    [SerializeField] InteractionObject interactionObject;
    [Tooltip("The effectors to interact with")]
    [SerializeField] FullBodyBipedEffector[] effectors;

    private InteractionSystem interactionSystem;
    private AimIK mAimIK;
    private Animator mAnim;

    public float magnitude = 1f;
    private Recoil recoil;

    void Awake()
    {
        interactionSystem = GetComponent<InteractionSystem>();
        mAnim = GetComponent<Animator>();

        recoil = GetComponent<Recoil>();
        mAimIK = GetComponent<AimIK>();
    }

    void GrabMP_40()
    {
        //mAnim.SetLayerWeight(1, 1);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E键按下");
            foreach (FullBodyBipedEffector e in effectors)
            {
                interactionSystem.StartInteraction(e, interactionObject, true);
                mAimIK.solver.transform = interactionObject.transform.Find("Gun/FBXExport_Props:MP40/AimTransform");
                mAimIK.solver.IKPositionWeight = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("R键按下");
            recoil.Fire(magnitude);
        }

        mAimIK.solver.IKPosition = transform.forward * 10f;
        mAimIK.solver.Update();
    }
}
