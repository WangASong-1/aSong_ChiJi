using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class test : MonoBehaviour {

    public Transform A, B;

    private Dictionary<int, GameObject> mDic = new Dictionary<int, GameObject>();
    private Dictionary<int, int> mDic_int = new Dictionary<int, int>();

    private Hashtable mHash = new Hashtable();

    private Action<int> mAction;
    private Action<int, string> mActionStr;

    private Func<int,int> mFunc;

    private List<int> mList;

    void Start()
    {
        transform.position = transform.rotation * new Vector3(0.02f, -0.6f, 0);

        mDic.Add(1, gameObject);
        mDic.Add(2, gameObject);
        mDic.Add(3, gameObject);
        mDic.Add(4, gameObject);


        mDic_int.Add(1, 1);
        mDic_int.Add(2, 2);
        mDic_int.Add(3, 3);
        mDic_int.Add(4, 4);
        mDic_int.Add(5, 5);

        mHash.Add(1, 1);

        mAction = x => Debug.Log("111 + " + x);
        mAction(22222);
        mFunc = x => x*x;
        Debug.Log(mFunc(3)) ;

        string str = "Hello aSong111";
        mActionStr = (int x, string s) => {
            if (s.Length > str.Length)
                Debug.Log("字符串长 = " + s);
            else
                Debug.Log("字符串短 = " + str);
                                           };
        mActionStr(3, "hello World");
        Debug.Log(mFunc(3));


        //---------- 集合 ---------------------
        mList = new List<int> { 1, 3, 5, 7, 9 };
        mList.ForEach(item => Debug.Log(item));

        //Dictionary<int, string> mdic = new Dictionary<int, string> { { 1, "123"}, { 2, "456"}  };

        if (str.IsNumber())
        {
            Debug.Log("2222222222");
        }
        if (str.IsNumber(123))
        {
            Debug.Log("1111111111");
        }
    }

    void DelegateAAA()
    {
        Profiler.BeginSample("委托 action");

        Add(1, 2, delegate {
            //Debug.Log("匿名delegate");
        });
        Profiler.EndSample();
        Profiler.BeginSample("委托 func");
        int abc = 2;
        AddBBB(1, 2, delegate (string a) {
            //Debug.Log("匿名delegate" + a);
            return int.Parse( a) + abc;
        });
        Profiler.EndSample();
    }

    void Add(int a, int b, UnityAction action)
    {
        //Debug.Log("a+b = " +(a+b).ToString());
        action();
    }

    void AddBBB(int a, int b, Func<string, int> action)
    {
        //Debug.Log("a+b = " + (a + b).ToString());
        Debug.Log( action((a + b).ToString()));
    }

    private void Update()
    {
        return;
        mActionStr(3, "hello World");
        DelegateAAA();

        UnityEngine.Object o;

        Profiler.BeginSample("字典foreach");
        foreach(var a in mDic)
        {
            //Debug.Log("1222" + 1);
            o = a.Value;
        }
        Profiler.EndSample();
        Profiler.BeginSample("字典foreach int");
        foreach (var a in mDic_int)
        {
            //Debug.Log("1222" + 1);
        }
        Profiler.EndSample();
        Profiler.BeginSample("字典while 迭代器 var");
        var enumerator = mDic_int.GetEnumerator();
        while (enumerator.MoveNext())
        {

        }
        Profiler.EndSample();

        Profiler.BeginSample("字典while 迭代器 ");

        Dictionary<int, int>.Enumerator IDictionaryEnumerator = mDic_int.GetEnumerator();
        int aaa = 0;
        string bbb;//= "123" + 22.ToString();
        while (IDictionaryEnumerator.MoveNext())
        {
            //Debug.Log("123");
            aaa = IDictionaryEnumerator.Current.Value;
        }
        Profiler.EndSample();

        Profiler.BeginSample("哈希 迭代器 ");

        var abc = mHash.GetEnumerator();
        while (abc.MoveNext())
        {
            //Debug.Log("123");
            aaa = (int)abc.Value;
        }
        Profiler.EndSample();
    }
}
