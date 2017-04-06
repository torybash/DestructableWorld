using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Map : MonoBehaviour {


	[Serializable]
	public enum DebugLevel
	{
		NONE	= 0,
		SIMPLE	= 1,
		ALL	= 2,
	}

	private bool DebugSimple { get { return debugLevel >= DebugLevel.SIMPLE; } }
	private bool DebugAll { get { return debugLevel >= DebugLevel.ALL; } }

	[SerializeField] private MeshRenderer rend;

	[SerializeField] private int xSize = 1920;
	[SerializeField] private int ySize = 1080;
	[SerializeField] private float pixPerUnit = 10f;

	[SerializeField] private float brushSize = 8f;

	[SerializeField] private Texture2D testTex;

	[SerializeField] private DebugLevel debugLevel = DebugLevel.SIMPLE;

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

	public void MoveRect(Rect bodyRect, Vector2 move, out Vector2 endPos)
	{
		if (DebugSimple) Debug.Log("|------------CheckCollision  - bodyRect: " + bodyRect + ", move: " + move.ToString("F5"));

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

		var normMove = move.normalized;
		var testRect = new Rect(startPos, bodyRect.size);
		var lastPos = testRect.position;

		if (DebugSimple) Debug.Log("start: " + startPos.ToString("F5") + ", end: " + goalEndPos.ToString("F5") + ", movesCount: " + movesCount + ", normMove: " + normMove.ToString("F5"));

		int tries = 0;
		while (((Mathf.Abs(move.x) > 0 && testRect.position.x != goalEndPos.x) || (Mathf.Abs(move.y) > 0 && testRect.position.y != goalEndPos.y)) && tries++ < 10)
		{
			//move 1 pix closer to goal pos
			//testRect.position = Vector2.MoveTowards(testRect.position, goalEndPos, 1f / pixPerUnit);
			
			var innerTestRect = new Rect(testRect);
			//innerTestRect.position = Vector2.MoveTowards(testRect.position, testRect.position + normMove, 1f / pixPerUnit);

			//innerTestRect.position = testRect.position + normMove / pixPerUnit;
			var newPos = testRect.position;
			newPos.x = Mathf.MoveTowards(newPos.x, goalEndPos.x, Mathf.Abs( normMove.x / pixPerUnit));
			//newPos.y = Mathf.MoveTowards(newPos.y, goalEndPos.y, Mathf.Abs( normMove.y / pixPerUnit));
			innerTestRect.position = newPos;

			bool hasCollision = false;
			if (move.x > 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.RIGHT);
				if (hasCollision)
				{
					normMove.x = 0;
					normMove.Normalize();
				}
			}else if (move.x < 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.LEFT);
				if (hasCollision)
				{
					normMove.x = 0;
					normMove.Normalize();
				}
			}

			newPos = testRect.position;
			newPos.y = Mathf.MoveTowards(newPos.y, goalEndPos.y, Mathf.Abs( normMove.y / pixPerUnit));
			innerTestRect.position = newPos;

			if (move.y > 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.UP);
				if (hasCollision)
				{
					normMove.y = 0;
					if (normMove != Vector2.zero) normMove.Normalize();
				}
			}else if (move.y < 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.DOWN);
				if (hasCollision)
				{
					normMove.y = 0;
					if (normMove != Vector2.zero) normMove.Normalize();
				}
			}

			if (DebugAll) Debug.Log("normMove: " + normMove.ToString("F5") + ", newPos: " + newPos.ToString("F5"));
			if (normMove == Vector2.zero) // hasCollision )
			{
				if (DebugSimple) Debug.Log("Collision found at rect: " + testRect);
				break;
			}
			//testRect.position = Vector2.MoveTowards(testRect.position, testRect.position + normMove, 1f / pixPerUnit);
			newPos.x = Mathf.MoveTowards(testRect.position.x, goalEndPos.x, Mathf.Abs( normMove.x / pixPerUnit));
			newPos.y = Mathf.MoveTowards(testRect.position.y, goalEndPos.y, Mathf.Abs( normMove.y / pixPerUnit));
			testRect.position = newPos;

			lastPos = testRect.position;
		}
		if (DebugSimple) Debug.Log("tries: "+ tries);

		endPos = lastPos;
	}

	private bool CheckRectCollision(Rect rect, Direction dir)
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

		switch (dir)
		{
			case Direction.UP:
				for (int x = cornerPos2Ds[2].x; x < cornerPos2Ds[0].x + 1; x++)
				{
					var pos2D = new Pos2D(x, cornerPos2Ds[2].y);
					if (IsTileFull(pos2D))
					{
						if (DebugAll)  Debug.Log(dir.ToString() + " move with rect: " + rect.ToString("F5") + " not allowed here!");
						return true;
					}
				}
				break;
			case Direction.RIGHT:
				for (int y = cornerPos2Ds[1].y; y < cornerPos2Ds[0].y + 1; y++)
				{
					var pos2D = new Pos2D(cornerPos2Ds[1].x, y);
					if (IsTileFull(pos2D))
					{
						if (DebugAll)  Debug.Log(dir.ToString() + " move with rect: " + rect.ToString("F5") + " not allowed here!");
						return true;
					}
				}
				break;
			case Direction.DOWN:
				for (int x = cornerPos2Ds[3].x; x < cornerPos2Ds[1].x + 1; x++)
				{
					var pos2D = new Pos2D(x, cornerPos2Ds[3].y);
					if (IsTileFull(pos2D))
					{
						if (DebugAll) Debug.Log(dir.ToString() + " move with rect: " + rect.ToString("F5") + " not allowed here!");
						return true;
					}
				}
				break;
			case Direction.LEFT:
				for (int y = cornerPos2Ds[3].y; y < cornerPos2Ds[2].y + 1; y++)
				{
					var pos2D = new Pos2D(cornerPos2Ds[3].x, y);
					if (IsTileFull(pos2D))
					{
						if (DebugAll) Debug.Log(dir.ToString() + " move with rect: " + rect.ToString("F5") + " not allowed here!");
						return true;
					}
				}
				break;
		}

		if (DebugAll) Debug.Log(dir.ToString() + " IS ALLOWED with rect: " + rect.ToString("F5"));

		return false;

		////up
		////From cornerPos2Ds[2].x to [0].x + 1, check +1y
		//bool upperBlocked = false;
		//for (int x = cornerPos2Ds[2].x; x < cornerPos2Ds[0].x + 1; x++)
		//{
		//	var pos2D = new Pos2D(x, cornerPos2Ds[2].y + 1);
		//	if (IsTileFull(pos2D))
		//	{
		//		upperBlocked = true;
		//		break;
		//	}
		//}

		////down
		////From cornerPos2Ds[3].x to [1].x + 1
		//bool lowerBlocked = false;
		//for (int x = cornerPos2Ds[3].x; x < cornerPos2Ds[1].x + 1; x++)
		//{
		//	var pos2D = new Pos2D(x, cornerPos2Ds[3].y);
		//	if (IsTileFull(pos2D))
		//	{
		//		lowerBlocked = true;
		//		break;
		//	}
		//}

		////left
		////From cornerPos2Ds[3].y to [2].y + 1
		//bool leftBlocked = false;
		//for (int y = cornerPos2Ds[3].y; y < cornerPos2Ds[2].y + 1; y++)
		//{
		//	var pos2D = new Pos2D(cornerPos2Ds[3].x, y);
		//	if (IsTileFull(pos2D))
		//	{
		//		leftBlocked = true;
		//		break;
		//	}
		//}

		////right
		////From cornerPos2Ds[1].y to [0].y + 1, check +1x
		//bool rightBlocked = false;
		//for (int y = cornerPos2Ds[1].y; y < cornerPos2Ds[0].y + 1; y++)
		//{
		//	var pos2D = new Pos2D(cornerPos2Ds[1].x + 1, y);
		//	if (IsTileFull(pos2D))
		//	{
		//		rightBlocked = true;
		//		break;
		//	}
		//}

		//Debug.Log("leftBlocked: " + leftBlocked + ", rightBlocked: " + rightBlocked + ", upperBlocked: " + upperBlocked + ", lowerBlocked: " + lowerBlocked);

		//return upperBlocked || lowerBlocked || leftBlocked || rightBlocked;
	}


	private enum Direction
	{
		UP,RIGHT,DOWN,LEFT
	}
}
