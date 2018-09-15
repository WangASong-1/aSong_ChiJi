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

    private bool b_Iint = false;

    private void Reset()
    {
        if(title)
            title.text = "";
        if(img)
            img.sprite = null;
        if(currentBullets)
            currentBullets.gameObject.SetActive(false);
        if(totalBbullets)
            totalBbullets.gameObject.SetActive(false);
    }

    void Init()
    {
        if (b_Iint)
            return;
        b_Iint = true;
        if (title == null && this.transform.Find("title") != null)
        {
            title = this.transform.Find("title").GetComponent<Text>();
        }

        if (img == null && this.transform.Find("Button") != null)
        {
            img = this.transform.Find("Button").GetComponent<Image>();
        }
        

        if (currentBullets == null && this.transform.Find("currentBullets") != null)
        {
            currentBullets = this.transform.Find("currentBullets").GetComponent<Text>();
        }
        

        if (totalBbullets == null && this.transform.Find("totalBullets") != null)
        {
            totalBbullets = this.transform.Find("totalBullets").GetComponent<Text>();
        }
    }

    void UpdateItem()
    {
        if (title != null)
            title.text = data.name.ToString();
       
        if (img != null)
            img.sprite = data.pic;

        if (currentBullets != null)
            currentBullets.text = 10.ToString();

        if (totalBbullets != null)
            totalBbullets.text = 20.ToString();
    }

    public void Refresh(aSong_PlayerData.Prop prop)
    {
        this.data = prop;
        Init();
        if (prop == null)
        {
            Reset();
        }
        else
        {
            UpdateItem();
        }
        
    }
}
