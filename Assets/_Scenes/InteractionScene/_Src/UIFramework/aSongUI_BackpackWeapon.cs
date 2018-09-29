using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aSongUI_BackpackWeapon : MonoBehaviour {
    private Text nameText;
    private Text bulletNeedText;
    private Text bulletsText;
    [SerializeField]
    private Image[] mAllImages = new Image[6];

    //private aSong_PlayerData.Prop[] mProps = new aSong_PlayerData.Prop[6];
    [SerializeField]

    private PropBaseModel[] mModels = new PropBaseModel[6];

    public void Init()
    {
        nameText = transform.Find("Instruction/Name").GetComponent<Text>();
        bulletNeedText = transform.Find("Instruction/bulletsNeed").GetComponent<Text>();
        bulletsText = transform.Find("Instruction/bullets").GetComponent<Text>();
        mAllImages[0] = transform.Find("Weapon/weaponImg").GetComponent<Image>();
        mAllImages[1] = transform.Find("Weapon/Telescope").GetComponent<Image>();
        mAllImages[2] = transform.Find("Weapon/Muzzle").GetComponent<Image>();
        mAllImages[3] = transform.Find("Weapon/GunHandle").GetComponent<Image>();
        mAllImages[4] = transform.Find("Weapon/CartridgeClip").GetComponent<Image>();
        mAllImages[5] = transform.Find("Weapon/Gunstock").GetComponent<Image>();
    }

    /// <summary>
    /// 装上道具,返回卸下来的
    /// </summary>
    /// <param name="_model"></param>
    /// <returns></returns>
    public PropBaseModel PutOnParts(PropBaseModel _model)
    {
        Debug.Log("配件name = " + _model.prop.name);
        PropBaseModel tmp = null;
        switch (_model.prop.type)
        {
            case PropType.telescope:
                tmp = mModels[1];
                mModels[1] = _model;
                break;
            case PropType.muzzle:
                tmp = mModels[2];
                mModels[2] = _model;
                break;
            case PropType.gunHandle:
                tmp = mModels[3];
                mModels[3] = _model;
                break;
            case PropType.cartridgeClip:
                tmp = mModels[4];
                mModels[4] = _model;
                break;

            case PropType.gunstock:
                tmp = mModels[5];
                mModels[5] = _model;
                break;
        }
        Refresh();

        return tmp;
    }

    public void PutOnWeapon(PropBaseModel _model)
    {
        if (!_model )
        {
            //卸下武器,清空配件
            Reset();
            return;
        }
        if((mModels[0] && _model != mModels[0]))
        {
            //Debug.Log("aSongUI_BackpackWeapon::PutOnWeapon  换武器了,清空配件");
            Reset();
        }
        mModels[0] = _model;
        //清空凹槽,刷新重新显示
        nameText.text = mModels[0].prop.name.ToString();
        Refresh();
        //根据prop类型,显示对应需求的配件(另外写一个不同武器对应的配件需求的文档,便于管理,不要杂糅在一个文档里)
    }

    public void Refresh()
    {
        bulletNeedText.text = "9毫米子弹";
        bulletsText.text = "10/100";
        
        for (int i = 0; i < mModels.Length; i++)
        {
            if (mModels[i] == null)
                continue;
            mAllImages[i].sprite = mModels[i].prop.pic;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < mModels.Length; i++)
        {
            if (i > 0)
                aSongUI_Controller.Instance.AddPropToBag(mModels[i]);
            mModels[i] = null;
            mAllImages[i].sprite = null;
            bulletNeedText.text = "";
            bulletsText.text = "";
            nameText.text = "";
            
        }
    }

    public bool CanPartsOn()
    {
        return mModels[0] != null;
    }

    public PropBaseModel GetWeaponModel()
    {
        return mModels[0];
    }
}
