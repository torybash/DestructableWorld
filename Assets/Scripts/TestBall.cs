using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysBody))]
public class TestBall : MonoBehaviour {

	private PhysBody body;

	[SerializeField]
	private float movementAcceleration = 10f;

	[SerializeField]
	private float maxRunSpeed = 3f;
	[SerializeField]
	private float jumpSpeed = 5f;

	private Vector2 velocity;

	void Awake(){
		body = GetComponent<PhysBody>();
	}

	void FixedUpdate(){

		var input = new Vector2(Input.GetAxis("Horizontal"), 0);

		var newVel = body.Velocity + input * Time.fixedDeltaTime * movementAcceleration;
		if (Mathf.Abs(body.Velocity.x) < maxRunSpeed)  //Clamp x velocity only if was not moving past max-run speed
		{
			newVel.x = Mathf.Clamp(newVel.x, -maxRunSpeed, maxRunSpeed);
		}
		if (Mathf.Abs(newVel.x) < maxRunSpeed || Mathf.Abs(newVel.x) < Mathf.Abs(body.Velocity.x))
		{
			body.Velocity = newVel;
		}


		if (Input.GetKeyDown(KeyCode.Space) && body.IsGrounded)
		{
			body.AddVelocity(Vector2.up * jumpSpeed);
		}
	}
}
