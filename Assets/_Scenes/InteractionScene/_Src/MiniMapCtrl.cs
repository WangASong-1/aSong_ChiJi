using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public class MiniMapCtrl : MonoBehaviour {
    public FollowTarget mMapFollow;
    public Slider mScaleSlider;
    public Transform mMap;

    public bool b_mapClicked = false;

    private Vector3 preMousePos = Vector3.zero;
    private bool b_mapBeDragged = false;

    private void Awake()
    {
        mScaleSlider.value = mMapFollow.offset.y;
    }

    public void ScaleMap()
    {
        //map.localScale = Vector3.one * mScaleSlider.value;
        mMapFollow.offset.y = mScaleSlider.value;
    }

    public void ClickMap()
    {
        if (b_mapBeDragged)
        {
            b_mapBeDragged = false;
            preMousePos = Vector3.zero;
            return;
        }
        b_mapClicked = !b_mapClicked;
        if (b_mapClicked)
        {
            transform.localScale = Vector3.one * 2f;
            mMapFollow.offset.y = mScaleSlider.minValue;
        }
        else
        {
            transform.localScale = Vector3.one ;
            mMapFollow.offset.y = mScaleSlider.maxValue;
        }
        mScaleSlider.value = mMapFollow.offset.y;
    }

    public void DragMap()
    {
        if (!b_mapBeDragged)
        {
            b_mapBeDragged = true;
            preMousePos = Input.mousePosition;
        }
        mMap.position += Input.mousePosition - preMousePos;
        preMousePos = Input.mousePosition;

        Vector3 vec = mMap.localPosition;
        vec.x = Mathf.Abs(vec.x) > 50f ? 50f * Mathf.Sign(vec.x) : vec.x;
        vec.y = Mathf.Abs(vec.y) > 50f ? 50f * Mathf.Sign(vec.y) : vec.y;
        mMap.localPosition = vec;
        //vec.x = Mathf.Abs(vec.x) > 50f ? 50f * Mathf.Sign(vec.x) : vec.x;
    }
}
