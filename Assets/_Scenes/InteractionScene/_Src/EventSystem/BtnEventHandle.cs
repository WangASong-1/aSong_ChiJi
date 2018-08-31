using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnEventHandle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void BtnClicked()
	{
		EventManager.TriggerEvent ("TouchEvent");
	}
}
