using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aSongUI_PropListItem : MonoBehaviour {

    public aSong_PlayerData.Prop data = null;
    private Text title;
    private Image img;
    private Text currentBullets;
    private Text totalBbullets;



    public void Refresh(aSong_PlayerData.Prop prop)
    {
        this.data = prop;
        if (this.transform.Find("title") != null)
        {
            title = this.transform.Find("title").GetComponent<Text>();
            title.text = prop.name.ToString();
        }

        if (this.transform.Find("Button") != null)
        {
            img = this.transform.Find("Button").GetComponent<Image>();
            img.sprite = prop.pic;
        }

        if (this.transform.Find("currentBullets") != null)
        {
            currentBullets = this.transform.Find("currentBullets").GetComponent<Text>();
            currentBullets.text = 10.ToString();
        }

        if (this.transform.Find("currentBullets") != null)
        {
            totalBbullets = this.transform.Find("totalBullets").GetComponent<Text>();
            totalBbullets.text = 20.ToString();
        }
        
    }
}
