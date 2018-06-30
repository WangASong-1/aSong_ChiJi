using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class TestInteractions : MonoBehaviour {
    public FullBodyBipedIK fullBodyIK;
    public Transform rightHand, leftHand;

    private void LateUpdate()
    {
        fullBodyIK.solver.rightHandEffector.position = rightHand.position;
        fullBodyIK.solver.rightHandEffector.rotation = rightHand.rotation;
        fullBodyIK.solver.rightHandEffector.positionWeight = 1;
        fullBodyIK.solver.rightHandEffector.rotationWeight = 1;

        fullBodyIK.solver.leftHandEffector.position = leftHand.position;
        fullBodyIK.solver.leftHandEffector.rotation = leftHand.rotation;
        fullBodyIK.solver.leftHandEffector.positionWeight = 1;
        fullBodyIK.solver.leftHandEffector.rotationWeight = 1;
    }
}
