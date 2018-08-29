using UnityEngine;
using System.Collections;

public class EventNode2 : EventNode
{
    private static EventNode2 mInstance;
    public static EventNode2 Instance
    {
        get
        {
            return mInstance;
        }
    }

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        mInstance = this;
        base.EventNodePriority = 20;
        if (EventNodeCore.Instance)
        {
            EventNodeCore.Instance.AttachEventNode(this);
        }
    }
    void OnDestroy()
    {
        if (EventNodeCore.Instance)
        {
            EventNodeCore.Instance.DetachEventNode(this);
        }
    }


}