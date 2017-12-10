using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LocalMovement : MonoBehaviour {
	[SerializeField]
	private float moveSpeed = 4;
	[SerializeField]
	private float rotationSpeed = 12;
	[SerializeField]
	private float jumpForce = 4;

	private Vector3 movementInputs;

	private float groundedRay = 1.1f; //Needed to validate jump only when near the ground
	private float groundedRayOffset = 1; //Needed to validate jump only when near the ground

	private Animator anim;
	private Rigidbody rb;

	void Start () {
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
	}
	void Update(){
		movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
	}
	void FixedUpdate () {
		//Apply Movements
		rb.MovePosition (transform.position + transform.forward * (movementInputs.z * moveSpeed) * Time.deltaTime);
		transform.RotateAround (transform.position,Vector3.up,  movementInputs.x * rotationSpeed * Time.deltaTime);

		anim.SetFloat ("Speed", (Input.GetAxis("Vertical") * moveSpeed));

		//Jump and Check for Ground
		if (Physics.Raycast (transform.position + (Vector3.up * groundedRayOffset), -Vector3.up, groundedRay)) {
			if (Input.GetButtonDown ("Jump")) {
				rb.AddForce (Vector3.up * jumpForce, ForceMode.Impulse);
				anim.SetTrigger ("Jump");
			} 
			anim.SetBool ("Grounded", true);
		} else {
			anim.SetBool ("Grounded", false);
		}
	}

}
