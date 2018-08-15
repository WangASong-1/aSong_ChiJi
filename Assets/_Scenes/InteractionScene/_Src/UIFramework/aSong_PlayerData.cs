using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//道具名. 
public enum PropName
{
    pistol, M416, M67 ,other
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
        public static int idCount = 0;
        public Prop(PropName _name)
        {
            idCount++;
            name = _name;
            propID = idCount;
            pic = Resources.Load<Sprite>(_name.ToString());
            type = aSongUI_Controller.Instance.GetPropType(_name);
            weight = aSongUI_Controller.Instance.GetWeight(_name);

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

    public void AddBagProp(PropBaseModel model)
    {
        if (model == null || dic_bagProp.ContainsValue(model))
            return;
        switch (model.prop.type)
        {
            case PropType.bomb:
                bombs.Add(model);
                break;
            case PropType.rifle:
                if(guns.Count >= 2)
                {
                    //若2个rifle格子满了
                    if(currentModel.prop.type == PropType.rifle)
                    {
                        //手上拿的也是rifle,那么替换
                        dic_bagProp.Remove(currentModel.prop.propID);
                        for(int i = 0; i < guns.Count; i++)
                        {
                            if( guns[i] == currentModel)
                            {
                                guns[i] = model;
                            }
                        }
                    }
                    else
                    {
                        //手上拿的不是rifle,那么扔掉第一个格子中的rifle
                        dic_bagProp.Remove(guns[0].prop.propID);
                        guns[0] = model;
                    }
                }
                else
                {
                    //手上道具都没满,直接加入.
                    guns.Add(model);
                }
                break;
            case PropType.pistol:
                pistol = model;
                break;
            case PropType.other:

                break;
        }
        currentModel = model;
        dic_bagProp.Add(model.prop.propID, model);
        return;
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

    public int GetGunNum()
    {
        return Guns.Count;
    }

}
