using UnityEngine;
//在资源面板右键Create，创建该类对应的Asset文件
[CreateAssetMenu(fileName = "GameDataAsset", menuName = "Creat GameData Asset")]
public class GameData : ScriptableObject { 
    [Header("测试_字符串")]
    public string testStr = "Test String";
    [Space(10)]
    [Header("测试_数组")]
    public int[] testArr;
}