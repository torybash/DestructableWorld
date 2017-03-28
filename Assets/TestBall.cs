using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public class TestBall : MonoBehaviour {

	[SerializeField]
	private float speed = 1f;

	//private Rigidbody2D body;

	[SerializeField]
	Map map;

	void Awake(){
		//body = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate(){
		var bodyRect = new Rect((Vector2)transform.position, transform.localScale);
		var velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
		var move = velocity * Time.fixedDeltaTime;
		//body.MovePosition(newPos);
		
		map.CheckCollision(bodyRect, move);
	}
}
