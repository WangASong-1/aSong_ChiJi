using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInstance {
    public bool m_IsUsed = false;
    public List<ISkillTrigger> m_SkillTriggers = new List<ISkillTrigger>();

    public SkillInstance()
    {
    }

    public SkillInstance(SkillInstance other)
    {
        foreach(ISkillTrigger trigger in other.m_SkillTriggers)
        {
            m_SkillTriggers.Add(trigger);
        }
    }

    public void Reset()
    {
        foreach (ISkillTrigger trigger in m_SkillTriggers)
        {
            trigger.Reset();
        }
    }

    public int GetTriggerCount(string typeName)
    {
        Debug.Log("typeName = " + typeName);
        int count = 0;
        foreach(ISkillTrigger trigger in m_SkillTriggers)
        {
            if (trigger.GetTypeName() == typeName)
                ++count;
        }
        return count;
    }
}
