using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class testDelegate : MonoBehaviour {
    public delegate void Read(int a);
    Read reading;


    // Use this for initialization
    void Start () {
        int a = 0;
        reading += Read11111;
        reading(a);
    }

    void Read11111(int a)
    {
        Debug.Log(123321);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
