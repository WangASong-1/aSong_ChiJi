using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;

public class aSongUI_Main : TTUIPage
{
    public GameObject playerHPBar;
    public GameObject playerWeapons;

  
    public aSongUI_Main() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/GameMainLayer";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        playerHPBar = transform.Find("PlayerHPBar").gameObject;
        playerWeapons = transform.Find("PlayerWeapon_1/Weapon").gameObject;

        playerWeapons.AddComponent<aSongUI_PropListItem>();
        playerWeapons.GetComponent<Button>().onClick.AddListener(aSongUI_Controller.Instance.OnClickSkillItem);

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
        Debug.Log("Refresh !!!!!!!!!!!!!!!!!!!!!");

        aSong_PlayerData propData = this.data != null ? this.data as aSong_PlayerData : aSongUI_Controller.Instance.playerData;
        if (propData.Guns.Count > 0)
        {
            if (propData.Guns[0] != null)
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!");
                playerWeapons.GetComponent<aSongUI_PropListItem>().Refresh(propData.Guns[0].prop);
            }
            else
            {
                playerWeapons.GetComponent<aSongUI_PropListItem>().Refresh(propData.Guns[0].prop);

            }
        }
        
        
    }

}
