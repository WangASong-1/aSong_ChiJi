using UnityEngine;
using System.Collections;

public class EventListener : MonoBehaviour, IEventListener
{
    // Awake is called when the script instance is being loaded.
    void Start()
    {
        if (EventNodeCore.Instance)
        {
            Debug.Log("EventListener222222222222222222222222");
            EventNodeCore.Instance.AttachEventListener(EventDef.EventTest1, this);
        }
    }

    void OnDestroy()
    {
        if (EventNodeCore.Instance)
        {
            EventNodeCore.Instance.DetachEventListener(EventDef.EventTest1, this);
        }
    }
    public bool HandleEvent(int id, object param1, object param2)
    {
        Debug.Log("EventListener.HandleEvent =>" + " id=" + id + "param1=" + param1);
        switch (id)
        {
            case EventDef.EventTest1:
                Debug.Log(this.name + "=>" + "HandleEvent EventTest1");
                return false;
        }
        return false;
    }

    public int EventPriority()
    {
        return 0;
    }
}