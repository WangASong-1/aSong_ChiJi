using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager {
    //这个dic用来管理需要监听的各类事件
    private Dictionary <string, UnityEvent> eventDictionary;

	private static EventManager eventManager = new EventManager();
	//单例
	public static EventManager instance
	{
		get
		{
			if (eventManager != null)
			{
                /*
				//在Unity的Hierarchy中必须有一个名为EventManager的空物体,并挂上EventManager脚本
				eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

				if (!eventManager)
				{
					Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
				}
				else
				{
					eventManager.Init (); 
				}*/
                eventManager.Init();
            }

			return eventManager;
		}
	}

	void Init ()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}
	//在需要监听某个事件的脚本中，调用这个方法来监听这个事件
	public static void StartListening (string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}
	//在不需要监听的时候停止监听
	public static void StopListening (string eventName, UnityAction listener)
	{
		if (eventManager == null) return;
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}
	//触发某个事件
	public static void TriggerEvent (string eventName)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke ();
		}
	}
}