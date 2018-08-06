using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//武器-攻击力, 防具-护甲值, 恢复剂-治疗量
public class PropBaseModel : MonoBehaviour {
    [SerializeField]
    public aSongUI_PropData.Prop prop;
    public PropName mName;


    private void Awake()
    {
        prop = new aSongUI_PropData.Prop(mName);
        
    }
}
