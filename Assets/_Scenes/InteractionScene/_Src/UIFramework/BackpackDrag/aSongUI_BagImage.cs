using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class aSongUI_BagImage : MonoBehaviour {
    private aSongUI_DragManager gsm;
    private Image bag_image;
    public int mouse_type = 0; // 0->没有装备，1...有不同装备
    public Sprite hair;
    public Sprite weapon;
    public Sprite foot;
    public Sprite UISprite;
    public Color weapon_color;
    public Color UISprite_color;

    void Awake()
    {
        gsm = aSongUI_DragManager.GetInstance();
        bag_image = GetComponent<Image>();
    }

    public void On_equip_Button()
    {
        Debug.Log("my bag click!");
        int MouseType = gsm.GetMouse().GetMouseType(); // 得到鼠标目前的mousetype
        if (bag_image.sprite != UISprite && MouseType == 0) // 若鼠标没有图片在上面，并且bag的image不为空有装备，则取走bag_image的装备
        {
            Debug.Log(mouse_type);
            bag_image.sprite = UISprite;
            bag_image.color = UISprite_color;
            gsm.GetMouse().SetMouseType(mouse_type); // 将当前装备的type给鼠标
            mouse_type = 0; // 此背包的mousetype变为0，则当前背包啥都没有
        }
        else
        {   // 若鼠标上有装备，则改背包的image sprite改变，根据type变为不同装备图片
            Debug.Log("my bag equipped!");
            if (MouseType == 1) bag_image.sprite = hair;
            else if (MouseType == 2) bag_image.sprite = weapon;
            else if (MouseType == 3) bag_image.sprite = foot;
            mouse_type = MouseType; // mousetype变为鼠标的mousetype
            bag_image.color = weapon_color; // 有装备了
            gsm.GetMouse().SetMouseType(0); // 鼠标装备消失
        }
    }
}
