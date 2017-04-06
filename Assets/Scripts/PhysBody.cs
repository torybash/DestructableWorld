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

	private Rect _bodyRect;


	private float gravity = -10f;

	[SerializeField]
	Map map;


	void FixedUpdate()
	{
		_bodyRect = new Rect((Vector2)transform.position, transform.localScale);

		_velocity = Vector2.ClampMagnitude(_velocity + gravity * Vector2.up * Time.fixedDeltaTime, maxSpeed);

		var move = _velocity * Time.fixedDeltaTime;
		if (move != Vector2.zero)
		{
			Vector2 newPos;
			map.MoveRect(_bodyRect, move, out newPos);
			transform.position = newPos;
		}
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
