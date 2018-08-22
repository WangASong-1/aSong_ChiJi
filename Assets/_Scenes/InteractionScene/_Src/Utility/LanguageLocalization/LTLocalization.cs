using UnityEngine;
using System.Collections.Generic;

public class LTLocalization
{

    public const string LANGUAGE_ENGLISH = "EN";
    public const string LANGUAGE_CHINESE = "CN";
    public const string LANGUAGE_JAPANESE = "JP";
    public const string LANGUAGE_FRENCH = "FR";
    public const string LANGUAGE_GERMAN = "GE";
    public const string LANGUAGE_ITALY = "IT";
    public const string LANGUAGE_KOREA = "KR";
    public const string LANGUAGE_RUSSIA = "RU";
    public const string LANGUAGE_SPANISH = "SP";

    private const string KEY_CODE = "KEY";
    private const string FILE_PATH = "LTLocalization/localization";

    private SystemLanguage language = SystemLanguage.Chinese;
    private Dictionary<string, string> textData = new Dictionary<string, string>();

    private static LTLocalization mInstance;

    private LTLocalization()
    {
    }

    private static string GetLanguageAB(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Afrikaans:
            case SystemLanguage.Arabic:
            case SystemLanguage.Basque:
            case SystemLanguage.Belarusian:
            case SystemLanguage.Bulgarian:
            case SystemLanguage.Catalan:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return LANGUAGE_CHINESE;
            case SystemLanguage.Czech:
            case SystemLanguage.Danish:
            case SystemLanguage.Dutch:
            case SystemLanguage.English:
            case SystemLanguage.Estonian:
            case SystemLanguage.Faroese:
            case SystemLanguage.Finnish:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.French:
                return LANGUAGE_FRENCH;
            case SystemLanguage.German:
                return LANGUAGE_GERMAN;
            case SystemLanguage.Greek:
            case SystemLanguage.Hebrew:
            case SystemLanguage.Icelandic:
            case SystemLanguage.Indonesian:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Italian:
                return LANGUAGE_ITALY;
            case SystemLanguage.Japanese:
                return LANGUAGE_JAPANESE;
            case SystemLanguage.Korean:
                return LANGUAGE_KOREA;
            case SystemLanguage.Latvian:
            case SystemLanguage.Lithuanian:
            case SystemLanguage.Norwegian:
            case SystemLanguage.Polish:
            case SystemLanguage.Portuguese:
            case SystemLanguage.Romanian:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Russian:
                return LANGUAGE_RUSSIA;
            case SystemLanguage.SerboCroatian:
            case SystemLanguage.Slovak:
            case SystemLanguage.Slovenian:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Spanish:
                return LANGUAGE_SPANISH;
            case SystemLanguage.Swedish:
            case SystemLanguage.Thai:
            case SystemLanguage.Turkish:
            case SystemLanguage.Ukrainian:
            case SystemLanguage.Vietnamese:
            case SystemLanguage.Unknown:
                return LANGUAGE_ENGLISH;
        }
        return LANGUAGE_CHINESE;
    }

    private static string GetWinReadPath(string fileName)
    {
        return Application.dataPath + "/_DataDir/" + fileName + ".csv";
    }

    private static string GetWinSavePath(string fileName)
    {
        return Application.dataPath + "/Resources/LTLocalization/" + fileName + ".txt";
    }

    private void ReadData()
    {
#if UNITY_EDITOR
        // 在Windows平台下读取语言配置文件
        string CSVFilePath = GetWinReadPath(FILE_PATH);
        LTCSVLoader loader = new LTCSVLoader();
        loader.ReadFile(CSVFilePath);
        // 将配置文件序列化为多个语言类
        int csvRow = loader.GetRow();
        int csvCol = loader.GetCol();
        Debug.Log("row:" + csvRow + "col:" + csvCol);
        for (int tempCol = 1; tempCol < csvCol; ++tempCol)
        {
            LTLocalizationData languageData = new LTLocalizationData();
            // 获取第一行数据(语言类型)
            languageData.LanguageType = loader.GetValueAt(tempCol, 0);
            // 遍历生成变量
            languageData.LanguageData = new Dictionary<string, string>();
            for (int tempRow = 1; tempRow < csvRow; ++tempRow)
            {
                //Debug.Log("loader.GetValueAt(0, tempRow) = " + loader.GetValueAt(0, tempRow));
                languageData.LanguageData.Add(loader.GetValueAt(0, tempRow), loader.GetValueAt(tempCol, tempRow));
            }
            // 将语言对象序列化存档
            SaveHelper.SaveData(GetWinSavePath(languageData.LanguageType), languageData);

            if (GetLanguageAB(language).Equals(languageData.LanguageType))
            {
                textData = languageData.LanguageData;
            }
        }
#else
        // 读取对应的语言对象
        TextAsset tempAsset = (TextAsset)Resources.Load("LTLocalization/" + GetLanguageAB(language), typeof(TextAsset));
        if (null == tempAsset)
        {
            tempAsset = (TextAsset)Resources.Load("LTLocalization/" + "EN", typeof(TextAsset));
        }
        if (null == tempAsset)
        {
            Debug.LogError("未检测到语言配置文件");
        }
        else
        {
            string saveData = tempAsset.text;
            LTLocalizationData currentLanguageData = (LTLocalizationData)SaveHelper.ReadData(saveData, typeof(LTLocalizationData), false);
            textData = currentLanguageData.LanguageData;
        }
#endif
    }

    private void SetLanguage(SystemLanguage language)
    {
        this.language = language;
    }

    public static void Init()
    {
        Debug.Log("Init 初始化");
        mInstance = new LTLocalization();
        mInstance.SetLanguage(Application.systemLanguage);
        mInstance.SetLanguage(SystemLanguage.English);
        mInstance.ReadData();
    }

    public static void ManualSetLanguage(SystemLanguage setLanguage)
    {
        if (null == mInstance)
        {
            mInstance = new LTLocalization();
        }
        mInstance.SetLanguage(setLanguage);
        mInstance.ReadData();
    }

    private static bool IsChinese()
    {
        if (null == mInstance)
        {
            Init();
        }
        return mInstance.language == SystemLanguage.Chinese;
    }

    public static string GetText(string key)
    {
        if (null == mInstance)
        {
            Init();
        }
        if (mInstance.textData.ContainsKey(key))
        {
            return mInstance.textData[key];
        }
        Debug.Log("没有这个key呀");
        return string.Empty;
        //return "[NoDefine]" + key;
    }

}