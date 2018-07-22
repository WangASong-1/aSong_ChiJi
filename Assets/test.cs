using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    public Transform A, B;

    void Start()
    {
        //设置A的欧拉角
        //试着更改各个分量查看B的不同旋转状态
        A.eulerAngles = new Vector3(1.0f, 1.5f, 2.0f);
        Debug.Log("111111111111111111111111111111111111111" + B.rotation * B.position);

    }

    void Update()
    {
        B.rotation *= A.rotation;
        //输出B的欧拉角，注意观察B的欧拉角变化
        Debug.Log(B.eulerAngles);
    }

}
