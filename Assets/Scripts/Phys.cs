using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Phys {

	[Serializable]
	public enum DebugLevel
	{
		NONE	= 0,
		SIMPLE	= 1,
		ALL	= 2,
	}

	public static DebugLevel debugLevel = DebugLevel.SIMPLE;
	private static bool DebugSimple { get { return debugLevel >= DebugLevel.SIMPLE; } }
	private static bool DebugAll { get { return debugLevel >= DebugLevel.ALL; } }

	public const float PIX_PER_UNIT = 10f;

	private static Map currMap;


	public static void SetMap(Map map)
	{
		currMap = map;
	}

	public static void MoveRect(Rect bodyRect, Vector2 move, out Vector2 endPos, out List<Vector2> blockedDirections)
	{
		if (DebugSimple) Debug.Log("|------------CheckCollision  - bodyRect: " + bodyRect + ", move: " + move.ToString("F5"));
		blockedDirections = new List<Vector2>();

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
			cornerPos2Ds[i] = Pos2D.FloorVector2(pos * PIX_PER_UNIT);
		}

		//Pre-check
		if (currMap.IsAnyTilesFull(cornerPos2Ds)){
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
			newPos.x = Mathf.MoveTowards(newPos.x, goalEndPos.x, Mathf.Abs( normMove.x / PIX_PER_UNIT));
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
					blockedDirections.Add(Vector2.right);
				}
			}else if (move.x < 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.LEFT);
				if (hasCollision)
				{
					normMove.x = 0;
					normMove.Normalize();
					blockedDirections.Add(Vector2.left);
				}
			}

			newPos = testRect.position;
			newPos.y = Mathf.MoveTowards(newPos.y, goalEndPos.y, Mathf.Abs( normMove.y / PIX_PER_UNIT));
			innerTestRect.position = newPos;

			if (move.y > 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.UP);
				if (hasCollision)
				{
					normMove.y = 0;
					if (normMove != Vector2.zero) normMove.Normalize();
					blockedDirections.Add(Vector2.up);
				}
			}else if (move.y < 0)
			{
				hasCollision = CheckRectCollision(innerTestRect, Direction.DOWN);
				if (hasCollision)
				{
					normMove.y = 0;
					if (normMove != Vector2.zero) normMove.Normalize();
					blockedDirections.Add(Vector2.down);
				}
			}

			if (DebugAll) Debug.Log("normMove: " + normMove.ToString("F5") + ", newPos: " + newPos.ToString("F5"));
			if (normMove == Vector2.zero) // hasCollision )
			{
				if (DebugSimple) Debug.Log("Collision found at rect: " + testRect);
				break;
			}
			//testRect.position = Vector2.MoveTowards(testRect.position, testRect.position + normMove, 1f / pixPerUnit);
			newPos.x = Mathf.MoveTowards(testRect.position.x, goalEndPos.x, Mathf.Abs( normMove.x / PIX_PER_UNIT));
			newPos.y = Mathf.MoveTowards(testRect.position.y, goalEndPos.y, Mathf.Abs( normMove.y / PIX_PER_UNIT));
			testRect.position = newPos;

			lastPos = testRect.position;
		}
		if (DebugSimple) Debug.Log("tries: "+ tries);

		endPos = lastPos;
	}

	private static bool CheckRectCollision(Rect rect, Direction dir)
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
			cornerPos2Ds[i] = Pos2D.FloorVector2(pos * PIX_PER_UNIT);
		}

		switch (dir)
		{
			case Direction.UP:
				for (int x = cornerPos2Ds[2].x; x < cornerPos2Ds[0].x + 1; x++)
				{
					var pos2D = new Pos2D(x, cornerPos2Ds[2].y);
					if (currMap.IsTileFull(pos2D))
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
					if (currMap.IsTileFull(pos2D))
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
					if (currMap.IsTileFull(pos2D))
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
					if (currMap.IsTileFull(pos2D))
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
