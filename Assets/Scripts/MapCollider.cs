using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Map))]
public class MapCollider : MonoBehaviour {

	private Map _map;

	private Transform _colliderContainer;
	private List<BoxCollider2D> _colliders = new List<BoxCollider2D>();

	void Awake()
	{
		_map = GetComponent<Map>();
		_colliderContainer = new GameObject("ColliderContainer").transform;
		_colliderContainer.SetParent(transform);
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			UpdateMap();
		}
	}

	private void UpdateMap()
	{
		int w = _map.XSize; int h = _map.YSize;
		var colors = _map.Tex.GetPixels();

		//foreach (var coll in _colliders)
		//{
		//	if (coll!=null) Destroy(coll);
		//}
		//_colliders.Clear();



		//for (int x = 0; x < w; x++)
		//{
		//	for (int y = 0; y < h; y++)
		//	{
		//		if (colors[x + y * h].a > 0)
		//		{
		//			var coll = _colliderContainer.gameObject.AddComponent<BoxCollider2D>();
		//			coll.size = Vector2.one / _map.PixPerUnit;
		//			coll.offset = new Vector2(x, y) / _map.PixPerUnit;
		//			_colliders.Add(coll);
		//		}

		//	}
		//}
	}
}
