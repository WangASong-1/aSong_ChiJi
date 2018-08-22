using UnityEngine;
using System.Collections.Generic;

public class LTLocalizationData
{

    public string LanguageType;

    public Dictionary<string, string> LanguageData;

    public override string ToString()
    {
        string result = "LanguageType:" + LanguageType;
        List<string> tempKeys = new List<string>(LanguageData.Keys);
        for (int i = 0; i < tempKeys.Count; ++i)
        {
            result += "\nKey:[" + tempKeys[i] + "]|Value:[" + LanguageData[tempKeys[i]] + "]";
        }
        return result;
    }

}