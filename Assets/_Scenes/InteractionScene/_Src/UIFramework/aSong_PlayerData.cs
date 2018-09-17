using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//道具名. 
public enum PropName
{
    M9, M416, M67 ,AK47,M14, bullets_9mm,  other
}

//道具种类:射手步枪,冲锋枪,雷
public enum PropType
{
    rifle, pistol, bomb, other
}


public class aSong_PlayerData {
    public class Prop
    {
        public int propID;
        public Sprite pic;
        public PropType type;
        public PropName name;
        public float weight;
        public int num;
        public static int idCount = 0;
        public Prop(PropName _name)
        {
            idCount++;
            name = _name;
            propID = idCount;
            pic = Resources.Load<Sprite>(_name.ToString());
            type = aSongUI_Controller.Instance.GetPropType(_name);
            weight = aSongUI_Controller.Instance.GetWeight(_name);
            num = aSongUI_Controller.Instance.GetNum(_name);
        }

        private Prop() { }
    }

    //玩家拾取了的 Dic
    //list中的prop
    public Dic_PropModel dic_listProp;
    //背包dic
    public Dic_PropModel dic_bagProp;


    public bool PropInList(int id)
    {
        return dic_listProp.ContainsKey(id);
    }

    public PropBaseModel GetListProp(int id)
    {
        return dic_listProp[id];
    }
    public bool PropInBag(int id)
    {
        return dic_bagProp.ContainsKey(id);
    }
    public PropBaseModel GetBagProp(int id)
    {
        return dic_bagProp[id];
    }
    public bool AddListProp(PropBaseModel model)
    {
        if (model == null || dic_listProp.ContainsValue(model))
            return false;
        dic_listProp.Add(model.prop.propID,model);
        return true;
    }

    public bool RemoveListProp(PropBaseModel model)
    {
        if (model == null || !dic_listProp.ContainsValue(model))
            return false;
        dic_listProp.Remove(model.prop.propID);
        return true;
    }

    //道具放入背包.有丢掉判断
    public void AddBagProp(PropBaseModel model)
    {
        if (model == null || dic_bagProp.ContainsValue(model))
            return;
        switch (model.prop.type)
        {
            case PropType.bomb:
                bombs.Add(model);
                break;
            case PropType.pistol:
            case PropType.rifle:
                PutWeaponInBackpack(model);
                break;
            case PropType.other:

                break;
        }
        if(currentModel == null)
            currentModel = model;

        dic_bagProp.Add(model.prop.propID, model);
        return;
    }

    void PutWeaponInBackpack(PropBaseModel model)
    {
        if(currentModel)
            Debug.Log("current = " + currentModel.name);
        if(model)
            Debug.Log("model = " + model.name);

        if (guns.Count == 3)
        {
            //背包满一定是:2把rifle 一把pistol
            //背包满的判断:丢掉手上的,加入新拿的
            if (currentModel.prop.type == PropType.rifle || currentModel.prop.type == PropType.pistol)
            {
                //手上拿的也是rifle,那么替换
                dic_bagProp.Remove(currentModel.prop.propID);
                for (int i = 0; i < guns.Count; i++)
                {
                    if (guns[i] == currentModel)
                    {
                        guns[i] = model;
                    }
                }
                currentModel = model;
            }
        }
        else
        {
            Debug.Log("123321");
            //背包不满的判断
            int count = 0;
            bool b_pisotl = false;
            foreach (var item in guns)
            {
                if (item.prop.type == PropType.rifle)
                    count++;
                if (item.prop.type == PropType.pistol)
                    b_pisotl = true;
            }
            if((model.prop.type == PropType.rifle && count==2) || (b_pisotl && model.prop.type == PropType.pistol))
            {
                Debug.Log("1233211111111");

                //如果有手枪并且当前拿取的是手枪；或者当前拿取的是rifle并且背包2把rifle了,那么丢掉道具
                if (currentModel.prop.type == model.prop.type)
                {
                    Debug.Log("1233222222222");

                    //手上拿跟拾取的是同类型的
                    dic_bagProp.Remove(currentModel.prop.propID);
                    for (int i = 0; i < guns.Count; i++)
                    {
                        //寻找currentModel在guns中的位置
                        if (guns[i] == currentModel)
                        {
                            guns[i] = model;
                        }
                    }
                    currentModel = model;
                }
                else
                {
                    //手上拿的不是拾取类型的,那么直接移除然后加入新的
                    dic_bagProp.Remove(guns[0].prop.propID);
                    guns[0] = model;
                }
            }
            else
            {
                guns.Add(model);
            }
        }
    }

    public void RemoveBagProp(PropBaseModel model)
    {
        if (model == null || !dic_bagProp.ContainsValue(model))
            return;
        dic_bagProp.Remove(model.prop.propID);
        return;
    }

    public PropBaseModel PickupProp(int propID)
    {
        PropBaseModel model = null;
        if (dic_listProp.ContainsKey(propID))
        {
            model = dic_listProp[propID];
            dic_listProp.Remove(propID);
        }
        if (dic_bagProp.ContainsKey(propID))
        {
            model = dic_bagProp[propID];
            dic_bagProp.Remove(propID);
        }
        return model;
    }

    public bool IsBagFull(PropBaseModel model)
    {
        bool b_Discard = false;

        switch (model.prop.type)
        {
            case PropType.pistol:
                if (pistol != null)
                    b_Discard = true;
                break;
            case PropType.rifle:
                if (GetGunNum() >= 3)
                    b_Discard = true;
                break;
            case PropType.bomb:
                b_Discard = false;
                break;
            case PropType.other:
                break;
        }
        return b_Discard;
    }

    private PropBaseModel currentModel;
    private List<PropBaseModel> guns = new List<PropBaseModel>();
    private PropBaseModel pistol= new PropBaseModel();
    private List<PropBaseModel> healths = new List<PropBaseModel>();
    private List<PropBaseModel> bombs = new List<PropBaseModel>();

    public List<PropBaseModel> Guns
    {
        get
        {
            return guns;
        }
    }

    public PropBaseModel Pistol
    {
        get
        {
            return pistol;
        }
    }

    public List<PropBaseModel> Healths
    {
        get
        {
            return healths;
        }
    }

    public List<PropBaseModel> Bombs
    {
        get
        {
            return bombs;
        }
    }

    public PropBaseModel CurrentModel
    {
        get
        {
            return currentModel;
        }

        set
        {
            currentModel = value;
        }
    }

    public int GetGunNum()
    {
        return Guns.Count;
    }

}
