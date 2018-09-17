using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aSongUI_BackpackWeapon : MonoBehaviour {
    private Text nameText;
    private Text bulletNeedText;
    private Text bulletsText;
    private Sprite weaponSprite;
    private aSong_PlayerData.Prop[] mProps = new aSong_PlayerData.Prop[6];

    public void Init()
    {
        nameText = transform.Find("Instruction/Name").GetComponent<Text>();
        bulletNeedText = transform.Find("Instruction/bulletsNeed").GetComponent<Text>();
        bulletsText = transform.Find("Instruction/bullets").GetComponent<Text>();

    }

    public void Refresh(aSong_PlayerData.Prop prop)
    {
        if(mProps[0] != prop)
        {
            //清空凹槽,刷新重新显示
            mProps[0] = prop;
            nameText.text = mProps[0].name.ToString();
            //根据prop类型,显示对应需求的配件(另外写一个配件需求的文档,便于管理,不要杂糅在一个文档里)

        }
        Refresh();
    }

    public void Refresh()
    {
        bulletNeedText.text = "9毫米子弹";
        bulletsText.text = "10/100";
    }

}
