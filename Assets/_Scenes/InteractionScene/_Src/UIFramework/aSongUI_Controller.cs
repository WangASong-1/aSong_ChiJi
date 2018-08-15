using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using LitJson;
using System;

public class aSongUI_Controller {
    private static aSongUI_Controller m_instance;
    public static aSongUI_Controller Instance
    {
        get
        {
            if (m_instance == null)
            {
                Debug.Log("1111111");
                m_instance = new aSongUI_Controller();
            }
            return m_instance;
        }
    }

    public aSong_PlayerData playerData;
    public JsonData jd;

    private aSong_UIPropList mUIPropList;

    //这个应该用事件来做.
    public aSong_UserControlInteractions mUserCtrl;


    private aSongUI_Controller()
    {
        //NOTE : this is Test Init Here.
        jd = aSong_UnityJsonUtil.Read(Application.streamingAssetsPath, "AllProps", false);
        playerData = new aSong_PlayerData();
        playerData.dic_listProp = new Dic_PropModel();
        playerData.dic_bagProp = new Dic_PropModel();
        
        Debug.Log(PropName.M416.ToString());
        
    }

    //在武器范围内，添加需要拾取的武器到List中并刷新
    public void AddPropToList(PropBaseModel _model)
    {
        if (!playerData.AddListProp(_model))
            return;
        
        RefreshPropList();
    }

  
    void RefreshPropList()
    {
        if (mUIPropList == null)
        {
            TTUIPage.ShowPage<aSong_UIPropList>();
            if (TTUIPage.allPages.ContainsKey("aSong_UIPropList"))
            {
                mUIPropList = (aSong_UIPropList)TTUIPage.allPages["aSong_UIPropList"];
            }

        }
        else
        {
            mUIPropList.Refresh();
        }
    }

    public void RemovePropFromList(PropBaseModel _model)
    {
        if (!playerData.RemoveListProp(_model))
            return;
        if(playerData.dic_listProp.Count<=0)
            TTUIPage.ClosePage<aSong_UIPropList>();
        else
            RefreshPropList();
    }

    void AddPropToBag(PropBaseModel _model)
    {
        playerData.AddBagProp(_model);
    }

    void RemovePropFromBag(PropBaseModel _model)
    {
        playerData.RemoveBagProp(_model);
    }

    public PropType GetPropType(PropName _name)
    {
        return (PropType)Enum.Parse(typeof(PropType), jd[_name.ToString()]["propType"].ToString());
    }

    public float GetWeight(PropName _name)
    {
        return float.Parse(jd[_name.ToString()]["weight"].ToString());

    }

    //ui中调用拾取:移除list中的model. 添加model 到对应的玩家背包list中
    public void PickupProp(int _propID)
    {
        PropBaseModel model = playerData.dic_listProp[_propID];
        if (mUserCtrl.PickupProp(model))
        {
            AddPropToBag(model);
            RemovePropFromList(model);
            TTUIPage.ShowPage<aSongUI_Main>();
        }
    }

    public void Discard(int _propID)
    {
        PropBaseModel model = playerData.dic_listProp[_propID];
        RemovePropFromBag(model);
    }



    //点击按钮后，我们需要拿起武器或者收起武器
    public void OnClickSkillItem()
    {
        aSongUI_PropListItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<aSongUI_PropListItem>();
        Debug.Log("propID = " + item.data.propID);
        PickupProp(item.data.propID);
    }
}
