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
        if (title == null && this.transform.Find("title") != null)
        {
            title = this.transform.Find("title").GetComponent<Text>();
        }
        if(title!= null)
            title.text = prop.name.ToString();

        if (img == null && this.transform.Find("Button") != null)
        {
            img = this.transform.Find("Button").GetComponent<Image>();
        }
        if(img!=null)
            img.sprite = prop.pic;


        if (currentBullets == null && this.transform.Find("currentBullets") != null)
        {
            currentBullets = this.transform.Find("currentBullets").GetComponent<Text>();
        }
        if(currentBullets!=null)
            currentBullets.text = 10.ToString();


        if (totalBbullets == null && this.transform.Find("totalBullets") != null)
        {
            totalBbullets = this.transform.Find("totalBullets").GetComponent<Text>();
        }
        if(totalBbullets!=null)
            totalBbullets.text = 20.ToString();


    }
}
