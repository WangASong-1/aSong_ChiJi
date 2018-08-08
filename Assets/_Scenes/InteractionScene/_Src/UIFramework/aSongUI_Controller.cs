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


    private aSongUI_Controller()
    {
        //NOTE : this is Test Init Here.
        jd = aSong_UnityJsonUtil.Read(Application.streamingAssetsPath, "AllProps", false);
        playerProp = new aSongUI_PropData();
        playerProp.props = new List<aSongUI_PropData.Prop>();
        playerProp.dic_prop = new Dictionary<int, aSongUI_PropData.Prop>();
        Debug.Log(PropName.M416.ToString());
        
    }

    //在武器范围内，添加需要拾取的武器到List中并刷新
    public void AddProp(PropName mName)
    {
        aSongUI_PropData.Prop prop = new aSongUI_PropData.Prop(mName);
        AddProp(prop);
    }

    public void AddProp(aSongUI_PropData.Prop _prop)
    {
        if (_prop == null || playerProp.props.Contains(_prop))
            return;
        playerProp.props.Add(_prop);
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


    public void RemoveProp(aSongUI_PropData.Prop _prop)
    {
        if (_prop == null || !playerProp.props.Contains(_prop))
            return;
        playerProp.props.Remove(_prop);
        if(playerProp.props.Count<=0)
            TTUIPage.ClosePage<aSong_UIPropList>();
        else
            RefreshPropList();

    }

    public PropType GetPropType(PropName _name)
    {
        return (PropType)Enum.Parse(typeof(PropType), jd[_name.ToString()]["propType"].ToString());
    }
}
