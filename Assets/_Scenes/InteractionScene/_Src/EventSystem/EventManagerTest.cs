using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class EventManagerTest : MonoBehaviour 
{
	UnityAction listener;

	void OnEnable()
	{
		listener = new UnityAction (MoveCube);
		EventManager.StartListening ("TouchEvent", listener);
	}

	void OnDisable()
	{
		EventManager.StopListening ("TouchEvent", listener);
	}

	void MoveCube()
	{
		transform.DOMove (Vector3.up * 2, 0.5f);
	}
}
