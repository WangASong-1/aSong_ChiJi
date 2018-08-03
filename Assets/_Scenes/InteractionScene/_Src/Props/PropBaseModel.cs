using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//武器-攻击力, 防具-护甲值, 恢复剂-治疗量
public class PropBaseModel : MonoBehaviour {
    [SerializeField]
    private int propID;

    public int PropID
    {
        get
        {
            return propID;
        }
    }

    [SerializeField]
    private Sprite mPropSprite;//{get; private set; }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        //这里根据ID来获取对应的图片
        mPropSprite = Resources.Load<Sprite>(PropID.ToString());
    }
}
