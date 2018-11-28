using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkilltriggerExecuteType
{
    other
}

public interface ISkillTrigger {
    void Init(string args);
    void Reset();
    ISkillTrigger Clone();
    bool Execute(ISkillCore instance, float curTime);
    float GetStartTime();
    bool IsExecuted();
    SkilltriggerExecuteType GetExecuteType();
    string GetTypeName();
}
