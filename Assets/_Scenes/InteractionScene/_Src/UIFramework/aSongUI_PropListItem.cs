using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aSongUI_PropListItem : MonoBehaviour {

    public aSong_PlayerData.Prop data = null;

    public void Refresh(aSong_PlayerData.Prop prop)
    {
        this.data = prop;
        this.transform.Find("title").GetComponent<Text>().text = prop.name.ToString();
        this.transform.Find("Button").GetComponent<Image>().sprite = prop.pic;
    }
}
