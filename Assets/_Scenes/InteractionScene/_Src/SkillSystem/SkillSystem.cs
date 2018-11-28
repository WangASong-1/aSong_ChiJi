using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : Singleton<SkillSystem> {
    private Dictionary<int, SkillInstance> mDic_BaseSkillInstance = new Dictionary<int, SkillInstance>();
    private List<SkillInstance> mList_SkillInstances = new List<SkillInstance>();

    public SkillSystem()
    {

    }

    public int CalcSkillBaseId(int skillId)
    {
        return 0;

    }

    public int CalcSkillId(int skillBaseId, int skillLevel)
    {
        return 0;

    }

    public int CalcSkillLevel(int skillId)
    {
        return 0;

    }

    /// <summary>
    /// 获取指定baseId技能的指定trigger名字的个数
    /// </summary>
    /// <param name="skillBaseId"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public int GetTriggerCount(int skillBaseId, string typeName)
    {
        return mDic_BaseSkillInstance[skillBaseId].GetTriggerCount(typeName);
    }

    public bool Init()
    {
        return false;
    }

    public SkillInstance NewSkillINstance(int skillBaseId)
    {
        SkillInstance skillInstance = new SkillInstance(mDic_BaseSkillInstance[skillBaseId]);
        return skillInstance;
    }

    public void RecycleSkillInstance(SkillInstance skill)
    {
        if (mList_SkillInstances.Contains(skill))
            return;
        mList_SkillInstances.Add(skill);
    }

    public void AddSkillInstanceToPool(int skillId, SkillInstance skill, bool v)
    {
        if (mDic_BaseSkillInstance.ContainsKey(skillId))
            return;
        mDic_BaseSkillInstance.Add(skillId, skill);
    }
    

    public void Reset()
    {

    }
}
