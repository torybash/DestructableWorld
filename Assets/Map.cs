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

	public bool IsTileFull(Pos2D pos)
	{
		//Debug.Log("IsTileFull - pos: " + pos + " = " + (_tex.GetPixel(pos.x, pos.y).a > 0));
		return _tex.GetPixel(pos.x, pos.y).a > 0;
	}

	public void CheckCollision(Rect bodyRect, Vector2 move, out Vector2 endPos)
	{
		//Debug.Log("CheckCollision  - bodyRect: " + bodyRect + ", move: " + move);

		var cornerPositions = new Vector2[]{
			//new Vector2(bodyRect.xMax, bodyRect.yMax),
			//new Vector2(bodyRect.xMax, bodyRect.yMin),
			//new Vector2(bodyRect.xMin, bodyRect.yMax),
			//new Vector2(bodyRect.xMin, bodyRect.yMin),

			new Vector2(bodyRect.x + bodyRect.width /2f, bodyRect.y + bodyRect.height /2f),
			new Vector2(bodyRect.x + bodyRect.width /2f, bodyRect.y - bodyRect.height /2f),
			new Vector2(bodyRect.x - bodyRect.width /2f, bodyRect.y + bodyRect.height /2f),
			new Vector2(bodyRect.x - bodyRect.width /2f, bodyRect.y - bodyRect.height /2f),
		};

		for (int i = 0; i < cornerPositions.Length; i++)
		{
			//Debug.Log("Corner " + i + ", " + cornerPositions[i]);
		}

		var cornerPos2Ds = new Pos2D[4];
		for (int i = 0; i < cornerPositions.Length; i++) {
			var pos = cornerPositions[i];
			cornerPos2Ds[i] = Pos2D.FloorVector2(pos * pixPerUnit);
		}

		//Pre-check
		if (IsAnyTilesFull(cornerPos2Ds)){
			Debug.LogError("Pre-check failed!")	;
		}

		var startPos = bodyRect.position;
		var goalEndPos = bodyRect.position + move;
		var startPos2D = Pos2D.FloorVector2(startPos);
		var goalEndPos2D = Pos2D.FloorVector2(goalEndPos);

		int movesCount = Mathf.Abs(startPos2D.x - goalEndPos2D.x) + Mathf.Abs(startPos2D.y - goalEndPos2D.y);
		//Debug.Log("start: " + startPos2D + ", end: "+ goalEndPos2D + ", movesCount: " + movesCount);

		
		var testRect = new Rect(startPos, bodyRect.size);
		var lastPos = testRect.position;
		while (testRect.position != goalEndPos)
		{
			//move 1 pix closer to goal pos
			testRect.position = Vector2.MoveTowards(testRect.position, goalEndPos, 1f / pixPerUnit);

			bool hasCollision = CheckCollision(testRect);
			if (hasCollision)
			{
				//Debug.Log("Collision found at rect: " + testRect);
				break;
			}
			lastPos = testRect.position;
		}

		endPos = lastPos;
	}

	private bool CheckCollision(Rect rect)
	{
		var cornerPositions = new Vector2[]{
			//new Vector2(rect.xMax, rect.yMax),
			//new Vector2(rect.xMax, rect.yMin),
			//new Vector2(rect.xMin, rect.yMax),
			//new Vector2(rect.xMin, rect.yMin),
			new Vector2(rect.x + rect.width /2f, rect.y + rect.height /2f),
			new Vector2(rect.x + rect.width /2f, rect.y - rect.height /2f),
			new Vector2(rect.x - rect.width /2f, rect.y + rect.height /2f),
			new Vector2(rect.x - rect.width /2f, rect.y - rect.height /2f),
		};

		var cornerPos2Ds = new Pos2D[4];
		for (int i = 0; i < cornerPositions.Length; i++) {
			var pos = cornerPositions[i];
			cornerPos2Ds[i] = Pos2D.FloorVector2(pos * pixPerUnit);
		}

		//up
		//From cornerPos2Ds[2].x to [0].x + 1, check +1y
		bool upperBlocked = false;
		for (int x = cornerPos2Ds[2].x; x < cornerPos2Ds[0].x + 1; x++)
		{
			var pos2D = new Pos2D(x, cornerPos2Ds[2].y + 1);
			if (IsTileFull(pos2D))
			{
				upperBlocked = true;
				break;
			}
		}

		//down
		//From cornerPos2Ds[3].x to [1].x + 1
		bool lowerBlocked = false;
		for (int x = cornerPos2Ds[3].x; x < cornerPos2Ds[1].x + 1; x++)
		{
			var pos2D = new Pos2D(x, cornerPos2Ds[3].y);
			if (IsTileFull(pos2D))
			{
				lowerBlocked = true;
				break;
			}
		}

		//left
		//From cornerPos2Ds[3].y to [2].y + 1
		bool leftBlocked = false;
		for (int y = cornerPos2Ds[3].y; y < cornerPos2Ds[2].y + 1; y++)
		{
			var pos2D = new Pos2D(cornerPos2Ds[3].x, y);
			if (IsTileFull(pos2D))
			{
				leftBlocked = true;
				break;
			}
		}

		//right
		//From cornerPos2Ds[1].y to [0].y + 1, check +1x
		bool rightBlocked = false;
		for (int y = cornerPos2Ds[1].y; y < cornerPos2Ds[0].y + 1; y++)
		{
			var pos2D = new Pos2D(cornerPos2Ds[1].x + 1, y);
			if (IsTileFull(pos2D))
			{
				rightBlocked = true;
				break;
			}
		}

		Debug.Log("leftBlocked: " + leftBlocked + ", rightBlocked: " + rightBlocked + ", upperBlocked: " + upperBlocked + ", lowerBlocked: " + lowerBlocked);

		return upperBlocked || lowerBlocked || leftBlocked || rightBlocked;
	}


}
