﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	[SerializeField] private MeshRenderer rend;
	[SerializeField] private MeshFilter filter;

	[SerializeField] private int xSize = 1920;
	[SerializeField] private int ySize = 1080;
	[SerializeField] private float pixPerUnit = 10f;

	[SerializeField] private float brushSize = 8f;

	[SerializeField] private Texture2D testTex;

	private Texture2D _tex;
	private Material _rendMat;

	private Color _alphaColor = new Color(0,0,0,0);


	void Start () {
		transform.localScale = new Vector3(xSize, ySize, 1f) / pixPerUnit;
		transform.position = new Vector3(xSize, ySize, 1f) / pixPerUnit / 2f;

		_tex = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);

		Debug.Log("_Tex filtermode: "+ _tex.filterMode);
//		_Tex.filterMode = FilterMode.Point;

		_rendMat = rend.material;

		_tex.SetPixels32(testTex.GetPixels32());
		_tex.Apply();

		_rendMat.mainTexture = _tex;
	}
	
	void Update () {
		if (Input.GetMouseButton(0))
			Painting(0);
		if (Input.GetMouseButton(1))
			Painting(1);
		
	}

	private void Painting(int type){
		var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) * pixPerUnit;

		Debug.Log("MousePos: "+ mousePos);

		for (int x = (int)(mousePos.x - brushSize); x < (int)(mousePos.x + brushSize + 1f); x++) {
			for (int y = (int)(mousePos.y - brushSize); y < (int)(mousePos.y + brushSize + 1f); y++) {
				_tex.SetPixel(x, y, type == 0 ? Color.red : _alphaColor);
			}
		}

		_tex.Apply();

		_rendMat.mainTexture = _tex;

	}
}