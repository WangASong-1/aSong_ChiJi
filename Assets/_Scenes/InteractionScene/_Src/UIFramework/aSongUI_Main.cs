using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;

public class aSongUI_Main : TTUIPage
{
    public GameObject playerHPBar;
    public GameObject playerWeapons;


    public aSongUI_Main(UIType type, UIMode mod, UICollider col) : base(type, mod, col)
    {
        
        uiPath = "UIPrefab/GameMainLayer";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        playerHPBar = transform.Find("PlayerHPBar").gameObject;
        playerWeapons = transform.Find("PlayerWeapon_1").gameObject;
        playerWeapons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(aSongUI_Controller.Instance.OnClickSkillItem);
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
    }
}
