using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ChangeCubeColor : MonoBehaviour {
	[SerializeField]
	Material blueMaterial;
	UnityAction listener;

	void OnEnable()
	{
		listener = new UnityAction (ChangeColor);
		EventManager.StartListening ("TouchEvent", listener);
	}

	void OnDisable()
	{
		EventManager.StopListening ("TouchEvent", listener);
	}

	void ChangeColor()
	{
		transform.GetComponent<Renderer> ().material = blueMaterial;
	}
}
