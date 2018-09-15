using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;

public class aSong_Backpack : TTUIPage
{

    public aSong_Backpack() : base(UIType.PopUp, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/Backpack";
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
