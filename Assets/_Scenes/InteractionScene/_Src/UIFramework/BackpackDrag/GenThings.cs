using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GenThings : MonoBehaviour
{

    private aSongUI_DragManager gsm;

    public void On_Press_GT()
    {
        gsm.GenAll();
    }

    void Awake()
    {
        gsm = aSongUI_DragManager.GetInstance();
    }
}