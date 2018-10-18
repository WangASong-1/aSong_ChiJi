using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aSongUI_DragManager : MonoBehaviour {
    private static aSongUI_DragManager _instance;
    private static aSongUI_MouseImage _Mouse;
    [SerializeField]
    private int IsHair = 0;
    [SerializeField]

    private int IsWeapon = 0;
    [SerializeField]
    private int IsFoot = 0;

    public static aSongUI_DragManager GetInstance()
    {
        if (_instance == null)
        {
            Debug.Log("aSongUI_DragManager~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            GameObject obj = GameObject.Find("DragCanvas");

            _instance = obj.AddComponent<aSongUI_DragManager>();
            //_instance = new aSongUI_DragManager();
        }
        return _instance;
    }

    public void SetMouse(aSongUI_MouseImage _mouse)
    {
        if (_Mouse == null)
        {
            _Mouse = _mouse;
        }
    }

    public aSongUI_MouseImage GetMouse()
    {
        return _Mouse;
    }

    public void GenAll()
    {
        Debug.Log("GenAll");
        IsFoot = 1;
        IsHair = 1;
        IsWeapon = 1;
    }

    public int GetHair() { return IsHair; }
    public int GetWeapon() { return IsWeapon; }
    public int GetFoot() { return IsFoot; }

    public void SetHair(int a) { IsHair = a; }
    public void SetWeapon(int a) { IsWeapon = a; }
    public void SetFoot(int a) { IsFoot = a; }
}