using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

//可以写进 InteractionSystem 的
public class aSongUI_Contact : MonoBehaviour {
    [SerializeField]
    public Dic_PropModel dic_propModel = new Dic_PropModel();

    public void OnTriggerEnter(Collider c)
    {

        PropBaseModel model = c.GetComponentInParent<PropBaseModel>();

        if (model == null) return;
        if(!dic_propModel.ContainsKey(model.prop.propID))
            dic_propModel.Add(model.prop.propID, model);
        aSongUI_Controller.Instance.AddProp(model);
    }

    public void OnTriggerExit(Collider c)
    {
        PropBaseModel model = c.GetComponentInParent<PropBaseModel>();

        if (model == null) return;
        if (dic_propModel.ContainsKey(model.prop.propID))
            dic_propModel.Remove(model.prop.propID);
        aSongUI_Controller.Instance.RemoveProp(model);
    }
}
