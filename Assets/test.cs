using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    public Transform A, B;

    void Start()
    {
        //设置A的欧拉角
        //试着更改各个分量查看B的不同旋转状态
        //A.eulerAngles = new Vector3(1.0f, 1.5f, 2.0f);
        //Debug.Log("111111111111111111111111111111111111111" + B.rotation * B.position);

        aSong_UnityJsonUtil.Init("","bbb");
        //
        //aSong_UnityJsonUtil.SetString("123", "456");
        //aSong_UnityJsonUtil.Save(true);
        aSong_UnityJsonUtil.Read();

        //相当于将该物体放到偏移该物体指定该位移值的位置(坐标系是以该物体)
        transform.position = transform.rotation * new Vector3(0.02f, -0.6f, 0);
    }
}
