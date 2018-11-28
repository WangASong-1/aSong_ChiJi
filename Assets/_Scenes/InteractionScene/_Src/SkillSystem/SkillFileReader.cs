using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillFileReader : Singleton<SkillFileReader>
{
    public void ReadSkillFile(string _FileName)
    {
        ParseScript(_FileName);
    }

    private void AddSkillInstanceToPool(int skillId, SkillInstance skill, bool v)
    {
        Debug.Log("SkillFileReader::AddSkillInstanceToPool =>添加技能 id = " + skillId);
        SkillSystem.Instance.AddSkillInstanceToPool(skillId, skill, v);
    }


    private bool ParseScript(string filename)
    {
        bool ret = false;
        try
        {
            StreamReader sr = new StreamReader(filename);
            if (sr != null)
            {
                ret = LoadScriptFromStream(sr);
                sr.Close();
            }
        }
        catch (Exception e)
        {
            string err = "Exception:" + e.Message + "\n" + e.StackTrace + "\n";
            Debug.LogError(err);
        }

        return ret;
    }

    private bool LoadScriptFromStream(StreamReader sr)
    {
        bool bracket = false;
        SkillInstance skill = null;
        do
        {
            string line = sr.ReadLine();
            if (line == null)
                break;

            line = line.Trim();

            if (line.StartsWith("//") || line == "")
                continue;

            if (line.StartsWith("skill"))
            {
                int start = line.IndexOf("(");
                int end = line.IndexOf(")");
                if (start == -1 || end == -1)
                    Debug.LogError("ParseScript Error, start == -1 || end == -1  {0}" + line);

                int length = end - start - 1;
                if (length <= 0)
                {
                    Debug.LogError("ParseScript Error, length <= 1, {0}" + line);
                    return false;
                }

                string args = line.Substring(start + 1, length);
                int skillId = (int)Convert.ChangeType(args, typeof(int));
                skill = new SkillInstance();
                AddSkillInstanceToPool(skillId, skill, true);
            }
            else if (line.StartsWith("{"))
            {
                bracket = true;
            }
            else if (line.StartsWith("}"))
            {
                bracket = false;

                // 按时间排序
                skill.m_SkillTriggers.Sort((left, right) =>
                {
                    if (left.GetStartTime() > right.GetStartTime())
                    {
                        return -1;
                    }
                    else if (left.GetStartTime() == right.GetStartTime())
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            }
            else
            {
                // 解析trigger
                if (skill != null && bracket == true)
                {
                    int start = line.IndexOf("(");
                    int end = line.IndexOf(")");
                    if (start == -1 || end == -1)
                        Debug.LogError("ParseScript Error, {0}" + line);

                    int length = end - start - 1;
                    if (length <= 0)
                    {
                        Debug.LogError("ParseScript Error, length <= 1, {0}" + line);
                        return false;
                    }

                    string type = line.Substring(0, start);
                    string args = line.Substring(start + 1, length);
                    args = args.Replace(" ", "");
                    ISkillTrigger trigger = SkillTriggerMgr.Instance.CreateTrigger(type, args);
                    Debug.Log("111111");
                    if (trigger != null)
                    {
                        skill.m_SkillTriggers.Add(trigger);
                        Debug.Log("2222222  count = " + skill.GetTriggerCount(trigger.GetTypeName()));

                    }
                }
            }
        } while (true);


        return true;
    }


}
