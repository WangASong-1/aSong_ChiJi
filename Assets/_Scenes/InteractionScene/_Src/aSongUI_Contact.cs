using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

//可以写进 InteractionSystem 的
public class aSongUI_Contact : MonoBehaviour {

    public void OnTriggerEnter(Collider c)
    {

        var trigger = c.GetComponent<InteractionTrigger>();

        if (trigger == null) return;
        aSongUI_Controller.Instance.AddProp(trigger.GetComponentInParent<PropBaseModel>().prop);
    }

    public void OnTriggerExit(Collider c)
    {
        var trigger = c.GetComponent<InteractionTrigger>();
        if (trigger == null) return;
        aSongUI_Controller.Instance.RemoveProp(trigger.GetComponentInParent<PropBaseModel>().prop);
    }
}
