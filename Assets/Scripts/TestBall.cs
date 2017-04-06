using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysBody))]
public class TestBall : MonoBehaviour {

	private PhysBody body;

	[SerializeField]
	private float acceleration = 10f;

	[SerializeField]
	private float maxRunSpeed = 3f;
	[SerializeField]
	private float jumpSpeed = 5f;

	private Vector2 velocity;

	[SerializeField]
	Map map;

	void Awake(){
		body = GetComponent<PhysBody>();
	}

	void FixedUpdate(){

		var input = new Vector2(Input.GetAxis("Horizontal"), 0);

		var newVel = body.Velocity + input * Time.fixedDeltaTime * acceleration;
		if (Mathf.Abs(body.Velocity.x) < maxRunSpeed)  //Clamp x velocity only if was not moving past max-run speed
		{
			newVel.x = Mathf.Clamp(newVel.x, -maxRunSpeed, maxRunSpeed);
		}
		if (Mathf.Abs(newVel.x) < maxRunSpeed || Mathf.Abs(newVel.x) < Mathf.Abs(body.Velocity.x))
		{
			body.Velocity = newVel;
		}

		//  if (Mathf.Abs(newVelocity.x) <= Mathf.Abs(m_MaxVelocityForInput.x) && Mathf.Abs(newVelocity.y) <= Mathf.Abs(m_MaxVelocityForInput.y)
		//    || newVelocity.magnitude < m_Velocity.magnitude)
		//{
		//    m_Velocity = newVelocity;
		//}

		//newVel.x = Mathf.Clamp(body.Velocity.x + input.x * Time.fixedDeltaTime, -maxRunSpeed, maxRunSpeed);

		//body.AddAcceleration(input * speed);

		//var bodyRect = new Rect((Vector2)transform.position, transform.localScale);
		////velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
		//velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, -maxXSpeed, maxXSpeed);
		////velocity.y = Mathf.Clamp(velocity.y + Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime, -maxYSpeed, maxYSpeed);
		//velocity.y = Mathf.Clamp(velocity.y + gravity * Time.fixedDeltaTime, -maxYSpeed, maxYSpeed);
		//var move = velocity * Time.fixedDeltaTime;
		////body.MovePosition(newPos);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			//velocity.y = Mathf.Clamp(velocity.y + jumpSpeed, -maxYSpeed, maxYSpeed);
			body.AddAcceleration(Vector2.up * jumpSpeed);
		}

		//if (move != Vector2.zero)
		//{
		//	Vector2 newPos;
		//	map.MoveRect(bodyRect, move, out newPos);
		//	transform.position = newPos;
		//}

	}
}
