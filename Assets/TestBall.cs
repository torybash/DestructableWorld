using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public class TestBall : MonoBehaviour {

	[SerializeField]
	private float speed = 8f;

	//private Rigidbody2D body;

	[SerializeField]
	Map map;

	void Awake(){
		//body = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate(){
		var newPos = (Vector2)transform.position + new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
		//body.MovePosition(newPos);
		
		var newBodyRect = new Rect(newPos, transform.localScale);
		map.CheckCollision(newBodyRect);
	}
}
