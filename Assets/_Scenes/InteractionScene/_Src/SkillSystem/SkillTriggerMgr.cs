using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillTriggerMgr : Singleton<SkillTriggerMgr>
{
    private Dictionary<string, ISkillTriggerFactory> m_DicTriggerFactory = new Dictionary<string, ISkillTriggerFactory>();

    public void RegisterTriggerFactory(string _name, ISkillTriggerFactory fac)
    {
        if (m_DicTriggerFactory.ContainsKey(_name))
            return;
        m_DicTriggerFactory.Add(_name, fac);
    }

    public ISkillTrigger CreateTrigger(string type, string args)
    {
        return m_DicTriggerFactory[type].CreateTrigger(args);
    }
   
}
