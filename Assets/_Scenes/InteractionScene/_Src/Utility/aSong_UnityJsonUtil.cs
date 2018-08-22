using UnityEngine;
using System.IO;  
using System.Collections.Generic;  
using LitJson; 
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using System;


//该方法仅仅作为一次性读写方法,不保证长期的数据正确性(除非仅仅针对单独的数据或者即读即改即存的)
//千万别在协程中使用，有风险.尸忆岛里面用在了2个地方：存档和玩家全局数据。
public class aSong_UnityJsonUtil {
	
	//相关变量声明：  
	private static string mFolderName;  
	private static string mFileName;
    //只能保存键值对，有点局限
	private static Dictionary<string, string> Dic_Value = new Dictionary<string, string>();

	private static StringBuilder mStringBuilder = new StringBuilder ();
	private static JsonWriter mJsonWriter = new JsonWriter(mStringBuilder);
    
	private static string FileName {  
		get {  
			return Path.Combine(FolderName, mFileName);  
		}  
	}  

	private static string FolderName {  
		get {
            //return Path.Combine(Application.persistentDataPath, mFolderName); 
            return Path.Combine(Application.streamingAssetsPath, mFolderName);
        }  
	}  

	public static void Init(string pFolderName, string pFileName) {  
		mFolderName = pFolderName;  
		mFileName = pFileName;  
		Dic_Value.Clear();  
	}

	//读取文件及json数据加载到Dictionary中  
	public static void Read() {
		if(!Directory.Exists(FolderName)) {  
			Directory.CreateDirectory(FolderName);
		}
		if(File.Exists(FileName)) {
			FileStream fs = new FileStream(FileName, FileMode.Open);  
			StreamReader sr = new StreamReader(fs);
            string str = sr.ReadToEnd();
            Debug.Log("str =  " + str);
            if(str.Length > 0)
            {
                JsonData values;
                //判断,第一个字符是{就认为是未加密的
                if (!str[0].Equals("{"))
                    values = JsonMapper.ToObject(decipheringContent(str));
                else
                    values = JsonMapper.ToObject(str);
                var p = values.Keys.GetEnumerator();
                p.MoveNext();
                for (int i = 0; i < values.Count; i++)
                {
                    Dic_Value.Add(p.Current, values[p.Current].ToString());
                    p.MoveNext();
                }
            }
			if(fs != null) {  
				fs.Close();  
			}
			if(sr != null) {  
				sr.Close();  
			}  
		}
	}

	//将Dictionary数据转成json保存到本地文件  
	public static void Save(bool b_encryption = false) {  
		string values = JsonMapper.ToJson(Dic_Value);

		Debug.Log("保存数据: 文件名 = " + FileName + "  内容 = " + values);
        if(b_encryption)
		    values = encryptionContent (values);

		if(!Directory.Exists(FolderName)) {  
			Directory.CreateDirectory(FolderName);  
		}  
		FileStream file = new FileStream(FileName, FileMode.Create);  
		byte[] bts = System.Text.Encoding.UTF8.GetBytes(values);  
		file.Write(bts,0,bts.Length);  
		if(file != null) {  
			file.Close();  
		}  
	}


    //读取文件及json数据加载到Dictionary中  
    public static bool FileExists()
    {
        return File.Exists(FileName);
    }

    //bool => 是否需要解密
    /// <summary>
    /// 读取指定文件,并返回JSonData数据.不影响全局Dic
    /// </summary>
    /// <param name="pFileName"></param>
    /// <param name="pFolderName"></param>
    /// <param name="b_encryption"></param>
    /// <returns></returns>
    public static JsonData Read(string pFileName, string pFolderName = "")
    {
        Init(pFolderName, pFileName);
        if (!Directory.Exists(FolderName))
        {
            Directory.CreateDirectory(FolderName);
        }
        if (File.Exists(FileName))
        {
            FileStream fs = new FileStream(FileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            JsonData values = null;
            string str = sr.ReadToEnd();
            Debug.Log("str =  " + str);
            if (str.Length > 0)
            {
                if (!str[0].Equals('{'))
                {
                    Debug.Log("111");
                    values = JsonMapper.ToObject(decipheringContent(str));
                }
                else
                {
                    Debug.Log("222");
                    values = JsonMapper.ToObject(str);
                }
            }

                
            if (fs != null)
            {
                fs.Close();
            }
            if (sr != null)
            {
                sr.Close();
            }
            return values;
        }
        Debug.Log("没有加载到文件 name = " + pFileName);
        return null;
    }

    //判断当前是否存在该key值  
    public static bool HasKey(string pKey) {  
		return Dic_Value.ContainsKey(pKey);  
	}  

	//读取string值  
	public static string GetString(string pKey) {  
		if(HasKey(pKey)) {  
			return Dic_Value[pKey];  
		} else {  
			return string.Empty;  
		}  
	}

	public static int GetDicCount(){
		return Dic_Value.Count;
	}

	//保存string值  
	public static void SetString(string pKey, string pValue) {  
		if(HasKey(pKey)) {  
			Dic_Value[pKey] = pValue;  
		} else {  
			Dic_Value.Add(pKey, pValue);  
		}  
	}

//--------------------------------------------------------------------------------------------------------------------\
	//以下是加密解密
//--------------------------------------------------------------------------------------------------------------------\
	//获取输入框的值
	public string inputText;

	//密钥
	private static string strkeyValue = "12365498732145678952836974115987";

	/// <summary>
	/// 内容加密
	/// </summary>
	/// <param name="ContentInfo">要加密内容</param>
	/// <param name="strkey">key值</param>
	/// <returns></returns>
	public static string encryptionContent(string ContentInfo)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes(strkeyValue);
		RijndaelManaged encryption = new RijndaelManaged();
		encryption.Key = keyArray;
		encryption.Mode = CipherMode.ECB;
		encryption.Padding = PaddingMode.PKCS7;
		ICryptoTransform cTransform = encryption.CreateEncryptor();
		byte[] _EncryptArray = UTF8Encoding.UTF8.GetBytes(ContentInfo);
		byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);
		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

	/// <summary>
	/// 内容解密
	/// </summary>
	/// <param name="encryptionContent">被加密内容</param>
	/// <param name="strkey">key值</param>
	/// <returns></returns>
	public static string decipheringContent(string encryptionContent)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes(strkeyValue);
		RijndaelManaged decipher = new RijndaelManaged();
		decipher.Key = keyArray;
		decipher.Mode = CipherMode.ECB;
		decipher.Padding = PaddingMode.PKCS7;
		ICryptoTransform cTransform = decipher.CreateDecryptor();
		byte[] _EncryptArray = Convert.FromBase64String(encryptionContent);
		byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);
		return UTF8Encoding.UTF8.GetString(resultArray);
	}
}
	