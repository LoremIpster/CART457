using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stillness : MonoBehaviour {
	
	public Rigidbody rb;
	Vector3 vel;

	void Start () {
		rb = GetComponent<Rigidbody>();
		vel = rb.velocity;
	}
	
	void FixedUpdate () {
		Debug.Log (rb.velocity);
		if (rb.velocity.x == 0 && rb.velocity.z == 0) {
			Debug.Log ("vel is " + vel);
		} else if (vel.x > 0 && vel.y > 0){
			Debug.Log ("MOVING");
		}
		
	}
}
