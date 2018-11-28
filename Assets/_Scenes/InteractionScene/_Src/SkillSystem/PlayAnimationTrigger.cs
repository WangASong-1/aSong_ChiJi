using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationTrigger : AbstrctSkillTrigger
{
    int m_AnimationId = 0;

    public override ISkillTrigger Clone()
    {
        return null;
    }

    public override bool Execute(ISkillCore instance, float curTime)
    {
        Debug.Log("m_TypeName = " + m_TypeName);
        return false;
    }

    public override void Init(string args)
    {
        m_TypeName = "PlayAnimation";
    }
}
