using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;

public class aSongUI_Backpack : TTUIPage
{
    GameObject propItem = null;
    Text weightText;
    aSongUI_BackpackWeapon leftWeapon;
    aSongUI_BackpackWeapon rightWeapon;


    List<aSongUI_PropListItem> propItems = new List<aSongUI_PropListItem>();
    List<aSongUI_PropListItem> propItemsPool = new List<aSongUI_PropListItem>();

    RectTransform content;
    private float itemHeight = 0f;

    public aSongUI_Backpack() : base(UIType.PopUp, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/Backpack";
    }

    public override void Awake(GameObject go)
    {
        propItem = this.transform.Find("Storehouse/Scroll View/Viewport/Content/Item").gameObject;
        propItem.SetActive(false);
        itemHeight = propItem.GetComponent<RectTransform>().rect.height;

        content = this.transform.Find("Storehouse/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        weightText = transform.Find("Storehouse/Instruction/Weight").GetComponent<Text>();
        transform.Find("Storehouse/Instruction/Back").GetComponent<Button>().onClick.AddListener(()=>{
            TTUIPage.ClosePage<aSongUI_Backpack>();
        });

        leftWeapon = transform.Find("Weapons/LeftWeapon").gameObject.AddComponent<aSongUI_BackpackWeapon>();
        rightWeapon = transform.Find("Weapons/RightWeapon").gameObject.AddComponent<aSongUI_BackpackWeapon>();
        leftWeapon.Init();
        rightWeapon.Init();

        
    }

    public override void Hide()
    {
        base.Hide();
        ResetData();
    }

    public override void Active()
    {
        base.Active();
    }

    public override void Refresh()
    {
       
        base.Refresh();
        if (isActive())
            ResetData();
        ShowPage();
    }

    void ResetData()
    {
        for (int i = 0; i < propItems.Count; i++)
        {
            propItems[i].gameObject.SetActive(false);
            propItems[i].transform.position = Vector3.zero;
            propItemsPool.Add(propItems[i]);
        }
        propItems.Clear();
    }

    void BackpackListButtonCliked()
    {
        aSongUI_PropListItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<aSongUI_PropListItem>();
        Debug.Log("Clicked name = " + UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        //Debug.Log("propID = " + item.data.propID);
        Debug.Log("name = " + item.data.name.ToString());
        if (item.data != null)
        {
            //PickupProp(item.data.propID);
            switch (item.data.type)
            {
                case PropType.bomb:
                    break;
                case PropType.bullet:
                    break;
                case PropType.cartridgeClip:
                case PropType.gunHandle:
                case PropType.gunstock:
                case PropType.muzzle:
                case PropType.telescope:
                    PutOnParts(item.data);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 背包list中的丢弃按钮
    /// </summary>
    /// <param name="_num"></param>
    void Discard(int _num = 0)
    {

    }

    /// <summary>
    /// 背包list中的装备配件按钮
    /// </summary>
    /// <param name="_prop"></param>
    void PutOnParts(aSong_PlayerData.Prop _prop)
    {
        aSongUI_BackpackWeapon curentBackpackWeapon = GetCurrent_aSongUI_BackpackWeapon();
        //当前没拿武器
        if (!curentBackpackWeapon)
            return;
        if (!curentBackpackWeapon.CanPartsOn())
        {
            Debug.Log("11111111111111111");
            curentBackpackWeapon.Reset();
            return;
        }
        //先从数据中背包List删除
        //再将道具引用放到aSongUI_BackpackWeapon对应的槽中
        //aSongUI_Contact.m_instance
        
        PropBaseModel _model = aSongUI_Controller.Instance.playerData.GetBagProp(_prop.propID);
        Debug.Log("装配件_model = " + _model.prop.name);
        aSongUI_Controller.Instance.RemovePropFromBag(_model);
        _model = curentBackpackWeapon.PutOnParts(_model);
        if (_model)
        {
            Debug.Log("卸下来的道具 = " + _model.prop.name);
            aSongUI_Controller.Instance.AddPropToBag(_model);
        }
            
        
    }

    /// <summary>
    /// 背包list中的使用按钮
    /// </summary>
    /// <param name="_prop"></param>
    void UsingProp(aSong_PlayerData.Prop _prop)
    {

    }

    private void ShowPage()
    {
        aSong_PlayerData propData = this.data != null ? this.data as aSong_PlayerData : aSongUI_Controller.Instance.playerData;
        //Debug.Log("propData.props.Count = " + propData.dic_bagProp.Count);
        var enumerator = propData.dic_bagProp.GetEnumerator();
        float countWeight = 0;
        while (enumerator.MoveNext())
        {
            Debug.Log("id = " + enumerator.Current.Value.prop.propID);
            if (enumerator.Current.Value.prop.type == PropType.pistol || enumerator.Current.Value.prop.type == PropType.rifle)
            {
                continue;
            }
            countWeight += enumerator.Current.Value.prop.weight * enumerator.Current.Value.prop.num;
            AddPropToItem(enumerator.Current.Value.prop);
        }
        content.sizeDelta = new Vector2(content.rect.width, propData.dic_listProp.Count * itemHeight);
        weightText.text = countWeight + "/200";

        if (propData.Guns[0])
            leftWeapon.PutOnWeapon(propData.Guns[0]);
        if (propData.Guns[1])
            rightWeapon.PutOnWeapon(propData.Guns[1]);
    }

    private void AddPropToItem(aSong_PlayerData.Prop prop)
    {
        if (propItemsPool.Count <= 0)
            CreatePropItem(prop);
        else
            GetPropItemFromPool(prop);
    }

    private void GetPropItemFromPool(aSong_PlayerData.Prop prop)
    {
        if (propItemsPool.Count <= 0)
            return;
        aSongUI_PropListItem item = propItemsPool[0];
        propItemsPool.Remove(item);
        propItems.Add(item);
        item.gameObject.SetActive(true);
        item.transform.localPosition = (propItems.Count - 1) * -50 * Vector3.up;
        item.Refresh(prop);
        return;
    }

    private void CreatePropItem(aSong_PlayerData.Prop prop)
    {
        GameObject go = GameObject.Instantiate(propItem) as GameObject;
        go.transform.SetParent(propItem.transform.parent);
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        aSongUI_PropListItem item = go.AddComponent<aSongUI_PropListItem>();
        item.Refresh(prop);
        propItems.Add(item);
        item.transform.localPosition = (propItems.Count - 1) * -50 * Vector3.up;
        Debug.Log("CreatePropItem");
        //add click btn
        //go.AddComponent<Button>().onClick.AddListener(aSongUI_Controller.Instance.OnClickSkillItem);
        go.AddComponent<Button>().onClick.AddListener(BackpackListButtonCliked);
        
    }

    aSongUI_BackpackWeapon GetCurrent_aSongUI_BackpackWeapon()
    {
        aSongUI_BackpackWeapon backpackWeapon = null;
        if(rightWeapon.CanPartsOn() && rightWeapon.GetWeaponModel() == aSongUI_Controller.Instance.CurrentModel)
        {
            backpackWeapon = rightWeapon;
        }

        if (leftWeapon.CanPartsOn() && leftWeapon.GetWeaponModel() == aSongUI_Controller.Instance.CurrentModel)
        {
            backpackWeapon = leftWeapon;
        }
        return backpackWeapon;
    }
}
