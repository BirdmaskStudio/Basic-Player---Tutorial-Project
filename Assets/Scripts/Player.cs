using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField]
	private float acceleration = 4;
	[SerializeField]
	private float moveSpeed = 4;
	[SerializeField]
	private float rotationSpeed = 12;
	[SerializeField]
	private float jumpForce = 4;
	[SerializeField]
	[Tooltip("Well auto set if left at 0")]
	private float gravityMut;


	private Vector3 movement; 
	private Quaternion storedRot;


	[SerializeField]
	private float verticalSpeed;

	private Rigidbody rb;
	private Animator anim;
	[SerializeField]
	private Transform forRef;
	// Use this for initialization
	void Start () {
		if(gravityMut == 0){
			gravityMut = Physics.gravity.y;
		}
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		storedRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
		AlignToCamera (ref movementInputs);
		movement = movementInputs;
		if (Input.GetButtonDown ("Reset")) {

			anim.SetTrigger ("Headbutt");
		}

	}

	void AlignToCamera(ref Vector3 InputsDir ){
		Vector3 camForward = forRef.forward;
		camForward.y = 0;
		InputsDir = Quaternion.LookRotation (camForward) * InputsDir; 
	}

	void FixedUpdate(){
		Vector3 finalMovement = movement;

		if(finalMovement.magnitude > .05f){
			storedRot = Quaternion.Slerp (storedRot, Quaternion.LookRotation (finalMovement), rotationSpeed * Time.deltaTime);
		}
		rb.MoveRotation (storedRot);

		Debug.DrawRay (transform.position + Vector3.up, -Vector3.up * 1.1f, Color.red);
		if (Physics.Raycast (transform.position + Vector3.up, -Vector3.up, 1.1f)) {
			anim.SetBool ("Grounded", true);
			verticalSpeed = 0;
			if (Input.GetButtonDown ("Jump")) {
				verticalSpeed = jumpForce;
				anim.SetTrigger ("Jump");
			}
		} else {
			anim.SetBool ("Grounded", false);
		}
		verticalSpeed += gravityMut * Time.deltaTime;
		verticalSpeed = Mathf.Clamp (verticalSpeed, gravityMut, jumpForce);
		finalMovement.y += verticalSpeed;

		rb.MovePosition (transform.position + (finalMovement * moveSpeed * Time.deltaTime));
		anim.SetFloat ("Speed", movement.magnitude * moveSpeed );
	}
}
