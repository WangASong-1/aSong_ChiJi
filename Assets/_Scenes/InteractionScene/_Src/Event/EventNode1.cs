using UnityEngine;
using System.Collections;

public class EventNode1 : EventNode
{
    private static EventNode1 mInstance;
    public static EventNode1 Instance
    {
        get
        {
            return mInstance;
        }
    }

    void Awake()
    {
        mInstance = this;

        //base.EventNodePriority = 30;

        if (EventNodeCore.Instance)
        {
            EventNodeCore.Instance.AttachEventNode(this);
        }
        Debug.Log("-------------1");
    }

    void OnDestroy()
    {
        if (EventNodeCore.Instance)
        {
            EventNodeCore.Instance.DetachEventNode(this);
        }
    }
}