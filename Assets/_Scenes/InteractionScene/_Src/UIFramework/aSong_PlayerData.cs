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
    public Dic_PropModel dic_bagProp;

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
        dic_bagProp.Add(model.prop.propID, model);
        switch (model.prop.type)
        {
            case PropType.bomb:
                break;
            case PropType.rifle:
                break;
            case PropType.pistol:
                break;
            case PropType.other:
                break;
        }
        return;
    }

    public void RemoveBagProp(PropBaseModel model)
    {
        if (model == null || !dic_bagProp.ContainsValue(model))
            return;
        dic_bagProp.Remove(model.prop.propID);
        return;
    }
    

    private List<PropBaseModel> guns;
    private PropBaseModel pistol;
    private List<PropBaseModel> healths;
    private List<PropBaseModel> bombs;

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
