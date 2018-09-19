using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//道具名. 
public enum PropName
{
    M9, M416, M67 ,AK47,M14, bullet_9mm, P1911, bullet_12, bullet_45ACP, bullet_300, bullet_556, bullet_762, bullet_arrow,
    health_painkiller, health_cola, health_adrenaline, health_bandage, health_firstAidKit, health_medkit, jerrican,  other
}

//道具种类:步枪,手枪,雷
public enum PropType
{
    rifle, pistol, bomb, bullet, other
}


public class aSong_PlayerData {
    [System.Serializable]
    public class Prop
    {
        public int propID;
        public Sprite pic;
        public PropType type;
        public PropName name;
        public float weight;
        public int num;
        public int maxNum;
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
            maxNum = aSongUI_Controller.Instance.GetMaxNum(_name);
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


    /// <summary>
    /// 从背包中移除指定的model
    /// </summary>
    /// <param name="model"></param>
    public void RemoveBagProp(PropBaseModel model)
    {
        if (model == null || !dic_bagProp.ContainsValue(model))
            return;
        dic_bagProp.Remove(model.prop.propID);
        aSongUI_Controller.Instance.RefreshPlayerData();

    }

    public void RemoveBagProp(int propID)
    {
        if (!dic_bagProp.ContainsKey(propID))
            return;
        dic_bagProp.Remove(propID);
        aSongUI_Controller.Instance.RefreshPlayerData();
    }

   
    //道具放入背包.有丢掉判断
    public void AddBagProp(PropBaseModel model)
    {
        if (model == null || dic_bagProp.ContainsValue(model))
            return;
        bool b_addToBackpack = true;
        switch (model.prop.type)
        {
            case PropType.bomb:
                b_addToBackpack = AddPropToBackpack(model, bombs);
                break;
            case PropType.pistol:
            case PropType.rifle:
                PutWeaponInBackpack(model);
                b_addToBackpack = false;
                if (currentModel == null)
                    currentModel = model;
                break;
            case PropType.bullet:
                b_addToBackpack = AddPropToBackpack(model, bulletList);
                break;
            case PropType.other:

                break;
        }

        if (b_addToBackpack)
        {
            dic_bagProp.Add(model.prop.propID, model);
            aSongUI_Controller.Instance.RefreshPlayerData();
        }
        return;
    }

    bool AddPropToBackpack(PropBaseModel model, List<PropBaseModel> list)
    {
        if (list.Count >= 1)
        {
            list[list.Count - 1].prop.num += model.prop.num;
            if (list[list.Count - 1].prop.num > model.prop.maxNum)
            {
                list.Add(model);
                list[bombs.Count - 1].prop.num = model.prop.maxNum;
                model.prop.num = list[list.Count - 1].prop.num - model.prop.maxNum;
                return true;
            }
            else
            {
                //没有被加入背包的model,应该被缓存池回收
            }
        }
        else
        {
            list.Add(model);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 将武器放入背包的检查逻辑
    /// </summary>
    /// <param name="model"></param>
    void PutWeaponInBackpack(PropBaseModel model)
    {
        if (currentModel)
            Debug.Log("current = " + currentModel.name);
        if (model)
            Debug.Log("model = " + model.name);

        int count_rifle = 0;
        int index_pistol = -1;
        for (int i = 0; i < guns.Count; i++)
        {
            if (guns[i] == null)
                continue;
            if (guns[i].prop.type == PropType.rifle)
                count_rifle++;
            if (guns[i].prop.type == PropType.pistol)
            {
                index_pistol = i;
            }
        }

        if (model.prop.type == PropType.pistol)
        {
            //已经有手枪了
            if (index_pistol >= 0)
            {
                //RemoveBagProp(guns[index_pistol].prop.propID);
                guns[index_pistol].Discard();
                guns[index_pistol] = model;
                if (currentModel != null && currentModel.prop.type == model.prop.type)
                {
                    //手上拿的也是枪
                    currentModel = model;
                }
            }
            else //没有手枪,直接按顺序放入背包
            {
                PutWeaponIn(model);
            }
        }
        if (model.prop.type == PropType.rifle)
        {
            //有手枪,然后背包里还有一把步枪.那么手枪要挪到第三格去
            if (index_pistol >= 0 && count_rifle >= 1 && index_pistol != 2)
            {
                guns[2] = guns[index_pistol];
                guns[index_pistol] = model;
            }
            if (count_rifle == 2)
            {
                //枪满了,手上拿的也刚好是步枪,丢掉手上的,拾取地上的
                if (currentModel != null && currentModel.prop.type == PropType.rifle)
                {
                    //手上拿的是步枪,移除手上的
                    //RemoveBagProp(currentModel.prop.propID);
                    currentModel.Discard();
                    for (int i = 0; i < guns.Count; i++)
                    {
                        if (guns[i] == currentModel)
                        {
                            guns[i] = model;
                        }
                    }
                    currentModel = model;
                }
                else
                {
                    //手上拿的不是步枪
                    //RemoveBagProp(guns[0].prop.propID);
                    guns[0].Discard();
                    guns[0] = model;
                }
            }
            else
            {
                //枪格子没满的时候,不用管手上拿的是啥,都直接放入背包
                PutWeaponIn(model);
            }
        }
    }

    /// <summary>
    /// 判断背包是否满了
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool IsBagFull(PropType type)
    {
        bool isFull = false;

        switch (type)
        {
            case PropType.pistol:
                foreach (var item in guns)
                {
                    if (item == null)
                        continue;
                    if (item.prop.type == PropType.pistol)
                        isFull = true;
                }
                break;
            case PropType.rifle:
                int count = 0;
                foreach (var item in guns)
                {
                    if (item == null)
                        continue;
                    if (item.prop.type == PropType.rifle)
                        count++;
                }
                isFull = count == 2;
                break;
            case PropType.bomb:
                isFull = false;
                break;
            case PropType.other:
                break;
        }
        return isFull;
    }

    /// <summary>
    /// 按顺序放入
    /// </summary>
    /// <param name="model"></param>
    void PutWeaponIn(PropBaseModel model)
    {
        for (int i = 0; i < guns.Count; i++)
        {
            if (guns[i] == null)
            {
                guns[i] = model;
                break;
            }
        }
    }

    /// <summary>
    /// 获取当前拿在手上武器在guns中的index
    /// </summary>
    /// <returns></returns>
    int GetCurrentGunIndex()
    {
        for (int i = 0; i < guns.Count; i++)
        {
            //说明手上拿的是枪
            if (currentModel == guns[i])
                return i;
        }
        return -1;
    }

    private PropBaseModel currentModel;
    private List<PropBaseModel> guns = new List<PropBaseModel>() { null, null,null};
    private PropBaseModel pistol= new PropBaseModel();
    private List<PropBaseModel> healths = new List<PropBaseModel>();
    private List<PropBaseModel> bombs = new List<PropBaseModel>();
    //这只是一种子弹的list
    private List<PropBaseModel> bulletList = new List<PropBaseModel>();
    
    //使用这个方便寻找对应model的list
    //private Dictionary<PropName, List<PropBaseModel>> dic_addableModelList = new Dictionary<PropName, List<PropBaseModel>>();

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
        int count = 0;
        foreach(var item in guns)
        {
            if (item != null)
                count++;
        }
        return count;
    }



}
