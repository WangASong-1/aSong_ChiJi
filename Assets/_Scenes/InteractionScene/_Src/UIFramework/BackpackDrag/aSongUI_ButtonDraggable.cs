using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class aSongUI_ButtonDraggable : aSongUI_ButtonBase
{
    public Image mImg;

    private void Awake()
    {
        mImg = GetComponent<Image>();
    }

    private void Update()
    {
        Vector3 position = Input.mousePosition;
        //在拖拽中实时更新图片的额位置信息
        //mImg.rectTransform.position = new Vector3(position.x, position.y, mImg.rectTransform.position.z);
        transform.position =  new Vector3(Input.mousePosition.x , Input.mousePosition.y , 0);
    }
}
