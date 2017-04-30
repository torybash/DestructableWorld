using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysBody : MonoBehaviour
{
	[SerializeField, ReadOnly]
	private Vector2 _velocity;
	public Vector2 Velocity {
		get { return _velocity; }
		set { _velocity = value; }
	}

	[SerializeField]
	private float maxSpeed = 10f;
	[SerializeField]
	private float drag = 0.95f;

	private Rect _bodyRect;

	private List<Vector2> _blockedDirections;
	public bool IsGrounded { get { return _blockedDirections != null && _blockedDirections.Contains(Vector2.down); } }

	public const float GRAVITY = -10f;

	[SerializeField]
	Map map;


	void FixedUpdate()
	{
		_bodyRect = new Rect((Vector2)transform.position, transform.localScale);

		_velocity = Vector2.ClampMagnitude(_velocity + GRAVITY * Vector2.up * Time.fixedDeltaTime, maxSpeed);

		

		var move = _velocity * Time.fixedDeltaTime;
		if (move != Vector2.zero)
		{
			var expectedPos = _bodyRect.position + move;
			Vector2 newPos; List<Vector2> blockedDirections;
			Phys.MoveRect(_bodyRect, move, out newPos, out blockedDirections);
			_blockedDirections = blockedDirections;
			transform.position = newPos;

			if (expectedPos.x != newPos.x)
				_velocity.x = 0;
			if (expectedPos.y != newPos.y)
				_velocity.y = 0;
		}

		_velocity *= 1f / (1f + drag * Time.fixedDeltaTime); 
	}

	public void AddVelocity(Vector2 vec)
	{
		_velocity = Vector2.ClampMagnitude(_velocity + vec, maxSpeed);

	}

	public void AddAcceleration(Vector2 acc)
	{
		_velocity = Vector2.ClampMagnitude(_velocity + acc * Time.fixedDeltaTime, maxSpeed);
		

		//velocity.x = Mathf.Clamp(velocity.x + acc.x * Time.fixedDeltaTime, -maxXSpeed, maxXSpeed);
		//velocity.y = Mathf.Clamp(velocity.y + acc.y * Time.fixedDeltaTime, -maxYSpeed, maxYSpeed);

		
		////velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
		//velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, -maxXSpeed, maxXSpeed);
		//velocity.y = Mathf.Clamp(velocity.y + Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime, -maxYSpeed, maxYSpeed);
		//velocity.y = Mathf.Clamp(velocity.y + gravity * Time.fixedDeltaTime, -maxYSpeed, maxYSpeed);
		//var move = velocity * Time.fixedDeltaTime;
		//body.MovePosition(newPos);

		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	velocity.y = Mathf.Clamp(velocity.y + jumpSpeed, -maxYSpeed, maxYSpeed);
		//}

		//if (move != Vector2.zero)
		//{
		//	Vector2 newPos;
		//	map.MoveRect(bodyRect, move, out newPos);
		//	transform.position = newPos;
		//}
	}

	
}
