using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstrctSkillTrigger : ISkillTrigger {
    protected float m_StartTime = 0f;
    protected bool m_IsExecuted = false;
    protected string m_TypeName;
    protected SkilltriggerExecuteType m_ExecuteType = SkilltriggerExecuteType.other;

    public abstract void Init(string args);

    public virtual void Reset()
    {
        m_IsExecuted = false;
    }

    public abstract ISkillTrigger Clone();

    public abstract bool Execute(ISkillCore instance, float curTime);

    public float GetStartTime()
    {
        return m_StartTime;
    }

    public bool IsExecuted()
    {
        return m_IsExecuted;
    }

    public SkilltriggerExecuteType GetExecuteType()
    {
        return m_ExecuteType;
    }

    public string GetTypeName()
    {
        return m_TypeName;
    }
}
