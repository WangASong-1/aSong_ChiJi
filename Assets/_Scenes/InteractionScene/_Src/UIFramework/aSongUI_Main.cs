using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;

public class aSongUI_Main : TTUIPage
{
    public GameObject playerHPBar;
    public GameObject playerWeapons;

    public Text currentWeaponBullet_1;
    public Image currentWeaponImg_1;


  
    public aSongUI_Main() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/GameMainLayer";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        playerHPBar = transform.Find("PlayerHPBar").gameObject;
        playerWeapons = transform.Find("PlayerWeapon_1").gameObject;
        playerWeapons.AddComponent<aSongUI_PropListItem>();
        playerWeapons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(aSongUI_Controller.Instance.OnClickSkillItem);

        currentWeaponBullet_1 = playerWeapons.transform.Find("Weapon/current").GetComponent<Text>();
        currentWeaponImg_1 = playerWeapons.transform.Find("Weapon").GetComponent<Image>();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Active()
    {
        base.Active();
    }

    public override void Refresh()
    {
        base.Refresh();

        aSong_PlayerData propData = this.data != null ? this.data as aSong_PlayerData : aSongUI_Controller.Instance.playerData;
        if (propData.Guns.Count > 0)
        {
            if (propData.Guns[0] != null)
            {
                playerWeapons.GetComponent<aSongUI_PropListItem>().Refresh(propData.Guns[0].prop);
                currentWeaponImg_1.sprite = propData.Guns[0].prop.pic;
                currentWeaponBullet_1.text = 10.ToString();
            }
            else
            {
                currentWeaponImg_1.sprite = null;
                currentWeaponBullet_1.text = null;
            }
        }
        
        
    }

}
