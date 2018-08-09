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

    public aSongUI_PropData playerProp;
    public JsonData jd;

    private aSong_UIPropList mUIPropList;

    //这个应该用事件来做.
    public aSong_UserControlInteractions mUserCtrl;


    private aSongUI_Controller()
    {
        //NOTE : this is Test Init Here.
        jd = aSong_UnityJsonUtil.Read(Application.streamingAssetsPath, "AllProps", false);
        playerProp = new aSongUI_PropData();
        playerProp.dic_prop = new Dic_PropModel();
        Debug.Log(PropName.M416.ToString());
        
    }

    //在武器范围内，添加需要拾取的武器到List中并刷新

    public void AddProp(PropBaseModel _model)
    {
        if (_model == null || playerProp.dic_prop.ContainsKey(_model.prop.propID))
            return;
        playerProp.dic_prop.Add(_model.prop.propID,_model);
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

    public void RemoveProp(PropBaseModel _model)
    {
        if (_model == null || !playerProp.dic_prop.ContainsKey(_model.prop.propID))
            return;
        playerProp.dic_prop.Remove(_model.prop.propID);
        if(playerProp.dic_prop.Count<=0)
            TTUIPage.ClosePage<aSong_UIPropList>();
        else
            RefreshPropList();

    }

    public PropType GetPropType(PropName _name)
    {
        return (PropType)Enum.Parse(typeof(PropType), jd[_name.ToString()]["propType"].ToString());
    }

    public void PickupProp(int _propID)
    {
        PropBaseModel model = playerProp.dic_prop[_propID];
        if(mUserCtrl.PickupProp(model))
            RemoveProp(model);
    }
}
