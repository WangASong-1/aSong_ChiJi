using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SaveHelper
{

    private const string M_KEY = "12365498774136985245695175336975";

    public static bool IsFileExist(string filePath)
    {
        return File.Exists(filePath);
    }

    public static bool IsDirectoryExists(string filePath)
    {
        return Directory.Exists(filePath);
    }

    public static void CreateFile(string fileName, string content)
    {
        StreamWriter streamWriter = File.CreateText(fileName);
        streamWriter.Write(content);
        streamWriter.Close();
    }

    public static void CreateDirectory(string filePath)
    {
        if (IsDirectoryExists(filePath))
        {
            return;
        }
        Directory.CreateDirectory(filePath);
    }

    private static string SerializeObject(object pObject)
    {
        string serializedString = string.Empty;
        serializedString = JsonConvert.SerializeObject(pObject);
        return serializedString;
    }

    private static object DeserializeObject(string pString, Type pType)
    {
        object deserializedObject = null;
        deserializedObject = JsonConvert.DeserializeObject(pString, pType);
        return deserializedObject;
    }

    private static string RijndaelEncrypt(string pString, string pKey)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(pString);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private static String RijndaelDecrypt(string pString, string pKey)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
        byte[] toEncryptArray = Convert.FromBase64String(pString);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    public static void SaveData(string fileName, object pObject)
    {
        // 如果文件已存在，则删除
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        string toSave = SerializeObject(pObject);
        toSave = RijndaelEncrypt(toSave, M_KEY);
        StreamWriter streamWriter = File.CreateText(fileName);
        streamWriter.Write(toSave);
        streamWriter.Close();
    }

    public static object ReadData(string str, Type pType, bool isFile = true)
    {
        string data;
        if (isFile)
        {
            // 如果文件不存在，则返回空
            if (!File.Exists(str))
            {
                return null;
            }
            StreamReader streamReader = File.OpenText(str);
            data = streamReader.ReadToEnd();
            streamReader.Close();
        }
        else
        {
            data = str;
        }

        data = RijndaelDecrypt(data, M_KEY);
        return DeserializeObject(data, pType);
    }
}