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
    aSongUI_PropListItem currentItem = null;

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
        propList.transform.localScale = Vector3.zero;
        propList.transform.DOScale(new Vector3(1, 1, 1), 0.5f);

        //Get Skill Data.
        //NOTE:here,maybe you havent Show(...pageData),ofcause you can got your skill data from your data singleton
        aSongUI_PropData propData = this.data != null ? this.data as aSongUI_PropData : aSongUI_Controller.Instance.playerProp;

        //create skill items in list.
        for (int i = 0; i < propData.props.Count; i++)
        {
            CreateSkillItem(propData.props[i]);
        }

    }

    public override void Hide()
    {
        for (int i = 0; i < propItems.Count; i++)
        {
            GameObject.Destroy(propItems[i].gameObject);
        }
        propItems.Clear();

        this.gameObject.SetActive(false);
    }

    private void CreateSkillItem(aSongUI_PropData.Prop skill)
    {
        GameObject go = GameObject.Instantiate(propItem) as GameObject;
        go.transform.SetParent(propItem.transform.parent);
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        aSongUI_PropListItem item = go.AddComponent<aSongUI_PropListItem>();
        item.Refresh(skill);
        propItems.Add(item);

        //add click btn
        go.AddComponent<Button>().onClick.AddListener(OnClickSkillItem);
    }

    //点击按钮后，我们需要拿起武器或者收起武器
    private void OnClickSkillItem()
    {
        aSongUI_PropListItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<aSongUI_PropListItem>();
    }
    
}
