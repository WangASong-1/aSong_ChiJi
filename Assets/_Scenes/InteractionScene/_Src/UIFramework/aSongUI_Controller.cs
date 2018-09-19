using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using LitJson;
using System;

//这里我不想要将每个按钮都弄成 TTUIPage来单独刷新按钮内容.
//当有道具拾取了或者按钮被点击了.都来 Controller里面来执行各自定义的事件.代理
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
    private aSongUI_Backpack mUIBackpack;

    //这个应该用事件来做.
    public aSong_UserControlInteractions mUserCtrl;


    private aSongUI_Controller()
    {
        //NOTE : this is Test Init Here.
        jd = aSong_UnityJsonUtil.Read("AllProps");
        playerData = new aSong_PlayerData();
        playerData.dic_listProp = new Dic_PropModel();
        playerData.dic_bagProp = new Dic_PropModel();

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

    public int GetNum(PropName _name)
    {
        return int.Parse(jd[_name.ToString()]["num"].ToString());
    }

    public int GetMaxNum(PropName _name)
    {
        return int.Parse(jd[_name.ToString()]["maxNum"].ToString());
    }

    //ui中调用拾取:移除list中的model. 添加model 到对应的玩家背包list中
    public void PickupProp(int _propID)
    {
        //执行动画过程中,不宜拿道具
        if (!mUserCtrl.CanPickup())
            return;
        PropBaseModel model;
        if (playerData.PropInBag(_propID))
        {
            //从背包中拿出
            model = playerData.GetBagProp(_propID);
            Debug.Log("_propID = " + _propID + " |name = " + model.name);

            if (playerData.CurrentModel !=null)
                Debug.Log("playerData.CurrentModel ID = " + playerData.CurrentModel.prop.propID + "| current name = " + playerData.CurrentModel.name);

            if (model == playerData.CurrentModel)
            {
                //当前拿的就是这把武器,那么就收起放背后
                mUserCtrl.PutPropInBackpack(model);
                playerData.CurrentModel = null;
                Debug.Log("放背后呀");
            }
            else
            {
                if(playerData.CurrentModel)
                    mUserCtrl.PutPropInBackpack(playerData.CurrentModel);
                mUserCtrl.PickupProp(model);
                playerData.CurrentModel = model;
            }
            TTUIPage.ShowPage<aSongUI_Main>();
            return;
        }

        if (playerData.PropInList(_propID))
        {
            //list里面更新的话就只需要判断背包是否满了,满了就替换
            model = playerData.GetListProp(_propID);
            AddPropToBag(model);
            RemovePropFromList(model);
            //换手了
            if (model == playerData.CurrentModel)
            {
                mUserCtrl.PickupProp(model);
            }
            else
            {
                mUserCtrl.PutPropInBackpack(model);
            }
            TTUIPage.ShowPage<aSongUI_Main>();

          
            /*
            if (mUserCtrl.PickupProp(playerData.CurrentModel,model))
            {
                AddPropToBag(model);
                RemovePropFromList(model);
                TTUIPage.ShowPage<aSongUI_Main>();
                return;
            }
            */
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
        Debug.Log("Clicked name = " + UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        //Debug.Log("propID = " + item.data.propID);
        //Debug.Log("name = " + item.name);
        if(item.data != null)
            PickupProp(item.data.propID);
    }

    public void OnClickBackpack()
    {
        Debug.Log("点我点我");
        if (mUIBackpack == null)
        {
            TTUIPage.ShowPage<aSongUI_Backpack>();
            if (TTUIPage.allPages.ContainsKey("aSongUI_Backpack"))
            {
                mUIBackpack = (aSongUI_Backpack)TTUIPage.allPages["aSongUI_Backpack"];
                Debug.Log("mUIBackpack = " + mUIBackpack.gameObject.name);
            }

        }
        else
        {
            if (mUIBackpack.isActive())
            {
                mUIBackpack.Refresh();
            }
            else
            {
                TTUIPage.ShowPage<aSongUI_Backpack>();
            }
                
        }
    }

    public void RefreshPlayerData()
    {
        if (mUIBackpack!= null && mUIBackpack.isActive())
        {
            mUIBackpack.Refresh();
        }
    }
}
