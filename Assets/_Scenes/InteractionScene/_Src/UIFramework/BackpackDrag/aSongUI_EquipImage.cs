using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class aSongUI_EquipImage : MonoBehaviour {
    private aSongUI_DragManager gsm;
    private Image equip_image;
    public int mouse_type;
    public Sprite weapon; // 为此背包中相应的装备
    public Sprite UISprite;
    public Color weapon_color;
    public Color UISprite_color;

    void Awake()
    {
        gsm = aSongUI_DragManager.GetInstance();
        equip_image = GetComponent<Image>();
    }

    public void On_equip_Button()
    {
        Debug.Log("equip click");
        int MouseType = gsm.GetMouse().GetMouseType(); // 得到鼠标上的mousetype
        if (equip_image.sprite == weapon && MouseType == 0) // 取走装备区装备，当装备区含有装备并且mousetype=0鼠标上没有装备
        {
            equip_image.sprite = UISprite;
            equip_image.color = UISprite_color;
            gsm.GetMouse().SetMouseType(mouse_type);
        }
        else
        {
            // 只有相同各类型的装备能够放到相应的装备区，并且不能够重复装备
            if (mouse_type == MouseType && equip_image.sprite != weapon)
            {
                // 将装备佩戴到装备区中
                equip_image.sprite = weapon;
                equip_image.color = weapon_color;
                mouse_type = MouseType;
                gsm.GetMouse().SetMouseType(0);
            }
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 防止重复装备
        if (mouse_type == 1 && gsm.GetHair() == 1)
        {
            Debug.Log("mouse_type");
            gsm.SetHair(0); // 已装备头部不能够再装备头部
            equip_image.sprite = weapon;
            equip_image.color = weapon_color;
        }
        else if (mouse_type == 2 && gsm.GetWeapon() == 1)
        {
            Debug.Log("mouse_type");
            gsm.SetWeapon(0); // 已装备武器不能够再装备武器
            equip_image.sprite = weapon;
            equip_image.color = weapon_color;
        }
        else if (mouse_type == 3 && gsm.GetFoot() == 1)
        {
            Debug.Log("mouse_type");
            gsm.SetFoot(0); // 已装备脚部
            equip_image.sprite = weapon;
            equip_image.color = weapon_color;
        }
    }
}
