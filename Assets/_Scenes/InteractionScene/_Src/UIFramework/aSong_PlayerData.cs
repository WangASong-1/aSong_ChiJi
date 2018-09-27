using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//道具名. 
public enum PropName
{
    M9, M416, M67 ,AK47,M14, bullet_9mm, P1911, bullet_12, bullet_45ACP, bullet_300, bullet_556, bullet_762, bullet_arrow,
    health_painkiller, health_cola, health_adrenaline, health_bandage, health_firstAidKit, health_medkit, jerrican,
    Telescope_x2, Telescope_x3, Telescope_x4, Telescope_x6, Telescope_x8, Muzzle_1, GunHandle_1, CartridgeClip_1, Gunstock_1,
    other
}

//道具种类:步枪,手枪,雷
//
public enum PropType
{
    rifle, pistol, bomb, bullet,health, telescope, muzzle, gunHandle, cartridgeClip, gunstock, other
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
            type = aSongUI_Controller.Instance.GetPropType(_name);
            pic = Resources.Load<Sprite>(type.ToString()+"/" + _name.ToString());
            weight = aSongUI_Controller.Instance.GetWeight(_name);
            num = aSongUI_Controller.Instance.GetNum(_name);
            maxNum = aSongUI_Controller.Instance.GetMaxNum(_name);
        }

        private Prop() { }
    }

    #region 拾取list 定义以及基础操作
    //玩家拾取了的 Dic
    //list中的prop
    public Dic_PropModel dic_listProp;

    /// <summary>
    /// 判断指定id是否在list中
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool PropInList(int id)
    {
        return dic_listProp.ContainsKey(id);
    }

    /// <summary>
    /// 根据id获取model
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PropBaseModel GetListProp(int id)
    {
        return dic_listProp[id];
    }
    
    /// <summary>
    /// 将指定的model加入到list中
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool AddListProp(PropBaseModel model)
    {
        if (model == null || dic_listProp.ContainsValue(model))
            return false;
        dic_listProp.Add(model.prop.propID,model);
        return true;
    }

    /// <summary>
    /// 移除指定的model
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool RemoveListProp(PropBaseModel model)
    {
        if (model == null || !dic_listProp.ContainsValue(model))
            return false;
        dic_listProp.Remove(model.prop.propID);
        return true;
    }
    #endregion


    #region 背包其他道具定义
    private PropBaseModel currentModel;
    /// <summary>
    /// 武器list
    /// </summary>
    private List<PropBaseModel> guns = new List<PropBaseModel>() { null, null, null };
    private PropBaseModel pistol = new PropBaseModel();
    /// <summary>
    /// 道具_恢复剂list
    /// </summary>
    private List<PropBaseModel> healths = new List<PropBaseModel>();
    /// <summary>
    /// 道具_炸弹类list
    /// </summary>
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
        foreach (var item in guns)
        {
            if (item != null)
                count++;
        }
        return count;
    }


    #endregion

    #region 背包道具的基本操作
    /// <summary>
    /// 背包中显示的道具的 dic
    /// </summary>
    public Dic_PropModel dic_bagProp;

    /// <summary>
    /// 道具是否在背包中:检查两个1.dic_bagProp,2.guns
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool PropInBag(int id)
    {
        return dic_bagProp.ContainsKey(id) || GetWeaponFromBackpack(id);
    }

    /// <summary>
    /// 获取指定id的model
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PropBaseModel GetBagProp(int id)
    {
        if (!PropInBag(id))
            return null;
        PropBaseModel model = null;
        if (dic_bagProp.ContainsKey(id))
            model = dic_bagProp[id];
        else
        {
            model = GetWeaponFromBackpack(id);
        }
        return model;
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
        }
        //只要有加入背包的举动,就必须更新背包
        aSongUI_Controller.Instance.RefreshPlayerData();

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

    PropBaseModel GetWeaponFromBackpack(int id)
    {
        foreach(var item in guns)
        {
            if (item != null)
            {
                if (item.prop.propID == id)
                    return item;
            }
        }
        return null;
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

    bool IsWeaponInBackpack(int id)
    {
        foreach(var item in guns)
        {
            if (item == null)
                continue;
            if (item.prop.propID == id)
                return true;
        }
        return false;
    }
    #endregion

}
