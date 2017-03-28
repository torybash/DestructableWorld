using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Map : MonoBehaviour {

	[SerializeField] private MeshRenderer rend;

	[SerializeField] private int xSize = 1920;
	[SerializeField] private int ySize = 1080;
	[SerializeField] private float pixPerUnit = 10f;

	[SerializeField] private float brushSize = 8f;

	[SerializeField] private Texture2D testTex;

	private Texture2D _tex;
	private Material _rendMat;

	public int XSize { get { return xSize; } }
	public int YSize { get { return ySize; } }
	public float PixPerUnit { get { return pixPerUnit; } }



	public Texture2D Tex { get { return _tex; } }


	private Color _alphaColor = new Color(0,0,0,0);


	void Start () {
		rend.transform.localScale = new Vector3(xSize, ySize, 1f) / pixPerUnit;
		rend.transform.position = new Vector3(xSize, ySize, 1f) / pixPerUnit / 2f;

		_tex = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);
		_tex.filterMode = FilterMode.Point;
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

	public bool IsAnyTilesFull(Pos2D[] pos2Ds){
		for (int i = 0; i < pos2Ds.Length; i++) {
			var pos2D = pos2Ds[i];
			if (IsTileFull(pos2D.x, pos2D.y)) return true;
		}
		return false;
	}

	public bool IsTileFull(int x, int y)
	{
		return _tex.GetPixel(x, y).a > 0;
	}

	public void CheckCollision(Rect bodyRect, Vector2 move)
	{
		var cornerPositions = new Vector2[]{
			new Vector2(bodyRect.xMax, bodyRect.yMax),
			new Vector2(bodyRect.xMax, bodyRect.yMin),
			new Vector2(bodyRect.xMin, bodyRect.yMax),
			new Vector2(bodyRect.xMin, bodyRect.yMin),
		};

		var cornerPos2Ds = new Pos2D[4];
		for (int i = 0; i < cornerPositions.Length; i++) {
			var pos = cornerPositions[i];

			cornerPos2Ds[i] = new Pos2D(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
		}

		//Pre-check
		if (IsAnyTilesFull(cornerPos2Ds)){
			Debug.LogError("Pre-check failed!")	;
		}

		var startPos = Pos2D.RoundVector2(bodyRect.position);
		var goalEndPos = Pos2D.RoundVector2(bodyRect.position + move);

		int movesCount = Mathf.Abs(startPos.x - goalEndPos.x) + Mathf.Abs(startPos.y - goalEndPos.y);
		Debug.Log("start: " + startPos + ", end: "+ goalEndPos + ", movesCount: " + movesCount);
	}


}
