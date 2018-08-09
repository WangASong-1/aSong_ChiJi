﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

//武器-攻击力, 防具-护甲值, 恢复剂-治疗量
public class PropBaseModel : MonoBehaviour {
    [SerializeField]
    public aSongUI_PropData.Prop prop;
    public PropName mName;
    public InteractionObject mInteractionObject;
    public InteractionTrigger[] mInteractionTriggers;
    private Rigidbody r;

    private void Awake()
    {
        prop = new aSongUI_PropData.Prop(mName);
        mInteractionObject = GetComponentInChildren<InteractionObject>();
        mInteractionTriggers = GetComponentsInChildren<InteractionTrigger>();
        r = GetComponent<Rigidbody>();
    }

    public void Pickup()
    {
        for (int i = 0; i < mInteractionTriggers.Length; i++)
        {
            mInteractionTriggers[i].gameObject.SetActive(false);
        }
    }

    public void Discard()
    {
        Debug.Log(prop.name.ToString());
        if(transform.parent != null)
        {
            var poser = transform.parent.GetComponent<Poser>();
            if (poser != null)
            {
                poser.poseRoot = null;
                poser.weight = 0f;
            }
        }

        transform.parent = null;

        if (r != null) r.isKinematic = false;

        for (int i = 0; i < mInteractionTriggers.Length; i++)
        {
            mInteractionTriggers[i].gameObject.SetActive(true);
        }
    }
}
