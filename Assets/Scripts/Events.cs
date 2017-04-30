using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events : MonoBehaviour {

	public class MapLoadedEvent : UnityEvent<Map> { }

	public MapLoadedEvent MapLoaded = new MapLoadedEvent();


	public static Events sInstance;

	public static Events I
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = FindObjectOfType<Events>();
				if (sInstance == null)
				{
					sInstance = new GameObject("Events").AddComponent<Events>();
				}
			}
			return sInstance;
		}
	}
	
}
