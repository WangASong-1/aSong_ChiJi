using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;
using DG.Tweening;

public class aSong_UIPropList : TTUIPage
{
    GameObject propList = null;
    GameObject propItem = null;
    List<aSongUI_PropListItem> propItems = new List<aSongUI_PropListItem>();
    List<aSongUI_PropListItem> propItemsPool = new List<aSongUI_PropListItem>();


    public bool b_showed = false;

    public aSong_UIPropList() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/PropListGroup";
    }

    public override void Awake(GameObject go)
    {
        propList = this.transform.Find("PropList").gameObject;

        propItem = this.transform.Find("PropList/Viewport/Content/Item").gameObject;
        propItem.SetActive(false);
    }

    public override void Refresh()
    {
        if (b_showed)
        {
            Hide();
            ShowPage();
        }
        else
        {
            propList.transform.localScale = Vector3.zero;
            propList.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            ShowPage();
        }
    }

    /// <summary>
    /// 隐藏,需要隐藏就代表着list中没有需要拾取的道具了
    /// </summary>
    public override void Hide()
    {
        //Debug.Log("Hide");
        b_showed = false;
        for (int i = 0; i < propItems.Count; i++)
        {
            propItems[i].transform.localPosition = Vector3.zero;
            propItems[i].gameObject.SetActive(false);
            propItemsPool.Add(propItems[i]);
        }
        propItems.Clear();
        this.gameObject.SetActive(false);
    }
    
    //道具还需要根据sort 进行排序. 1.各种类的枪,2.手枪,3.护甲装备,4.血包,6.炸弹......
    //分两种：一种是已经拥有了的，一种是未拥有但是需要的.后一种全排在前一种的前面
    private void ShowPage()
    {
        //Get Skill Data.
        //NOTE:here,maybe you havent Show(...pageData),ofcause you can got your skill data from your data singleton
        b_showed = true;
        this.gameObject.SetActive(true);
        aSong_PlayerData propData = this.data != null ? this.data as aSong_PlayerData : aSongUI_Controller.Instance.playerData;
        Debug.Log("propData.props.Count = " + propData.dic_listProp.Count);


        var enumerator = propData.dic_listProp.GetEnumerator();
        while (enumerator.MoveNext())
        {
            AddPropToItem(enumerator.Current.Value.prop);
        }
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
        item.Refresh(prop);
        item.transform.localPosition = Vector3.up * -50 * (propItems.Count - 1);
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
        item.transform.localPosition = Vector3.up * -50 * (propItems.Count - 1);
        Debug.Log("CreatePropItem");
        //add click btn
        go.AddComponent<Button>().onClick.AddListener(aSongUI_Controller.Instance.OnClickSkillItem);
    }

    
    
}
