using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class aSong_GraMP_40 : MonoBehaviour {
    [Tooltip("The object to interact to")]
    [SerializeField] InteractionObject interactionObject;
    [Tooltip("The effectors to interact with")]
    [SerializeField] FullBodyBipedEffector[] effectors;

    private InteractionSystem interactionSystem;
    private Animator mAnim;

    public float magnitude = 1f;
    private Recoil recoil;

    void Awake()
    {
        interactionSystem = GetComponent<InteractionSystem>();
        mAnim = GetComponent<Animator>();

        recoil = GetComponent<Recoil>();
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

            }
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("R键按下");
            recoil.Fire(magnitude);
        }
    }
}
