using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

	private Camera _cam;
	private Camera Cam
	{
		get
		{
			if (_cam == null) _cam = GetComponent<Camera>();
			return _cam;
		}
	}

	void Awake()
	{
		Events.I.MapLoaded.AddListener(OnMapLoaded);
	}

	private void OnMapLoaded(Map map)
	{
		Cam.orthographicSize = map.YSize / Phys.PIX_PER_UNIT / 2f;

		float aspect = map.XSize / (float)map.YSize;
		transform.position = new Vector3(map.transform.position.x + Cam.orthographicSize * aspect, map.transform.position.y + Cam.orthographicSize, transform.position.z);
	}


}
