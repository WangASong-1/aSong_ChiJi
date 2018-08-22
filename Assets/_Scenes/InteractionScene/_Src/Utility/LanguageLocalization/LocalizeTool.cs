using UnityEngine;
using System.Collections;

public class LocalizeTool : MonoBehaviour {
    public enum LocalType
    {
        Materia, Texture
    } 
    public LocalType _type = LocalType.Texture;
    public string key;
    public Renderer mRender;
    private bool b_Init = false;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (b_Init)
            return;
        b_Init = true;
        switch (_type)
        {
            case LocalType.Materia:
                //Debug.Log("(LTLocalization.GetText(key) = " + LTLocalization.GetText(key));
                mRender.material = Resources.Load<Material>(LTLocalization.GetText(key));
                break;
            case LocalType.Texture:
                mRender.material.SetTexture("_MainTex", Resources.Load<Texture>(LTLocalization.GetText(key)));

                break;
        }
    }
}
