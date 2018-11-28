using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestSkillSystem : MonoBehaviour {

	void Start () {
        //注册新技能
        SkillTriggerMgr.Instance.RegisterTriggerFactory("PlayAnimation", new SkillTriggerFactory<PlayAnimationTrigger>());

        //读取技能文件
        SkillFileReader.Instance.ReadSkillFile(Path.Combine( Application.streamingAssetsPath,"Skill"));

        //获取技能并执行
        Debug.Log(SkillSystem.Instance.GetTriggerCount(1, "PlayAnimation"));
    }


}
