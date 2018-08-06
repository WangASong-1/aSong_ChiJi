using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;

public class aSongUI_Main : MonoBehaviour {
    private void Awake()
    {
        TTUIPage.ShowPage<aSong_UIPropList>();
    }
}
