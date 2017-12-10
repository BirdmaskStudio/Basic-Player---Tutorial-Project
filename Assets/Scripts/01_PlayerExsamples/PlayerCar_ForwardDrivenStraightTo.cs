using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar_ForwardDrivenStraightTo : MonoBehaviour {

	private enum moveBy
	{
		ForwardDriven, DirectTooMovement, ToyCar
	}
	[SerializeField]
	private moveBy PlayerType;

	[SerializeField]
	[Tooltip("Very important for Forward driven movement")]
	private float rotationSpeed = 12;
	[SerializeField]
	private float moveSpeed = 4;
	[SerializeField]
	[Tooltip("Object used as input offset reference")]
	private Transform cameraRef;

	[SerializeField]
	[Tooltip("Angle need for toy car to invert inputs for reverse automatically - if 'Jump' button is pressed")]
	private float reverseAngle = 110;//

	[SerializeField]
	private LineRenderer line;
	private float lineOffset = .1f;
	private float lineScale = 3f;

	//Needed as constant to refrance when no inputs are given(so player doesn't reset to defult rotation)
	private Quaternion storedRot;
	private Vector3 movementInputs;
	private Rigidbody rb;

	void Start () {
		if (cameraRef == null) {
			cameraRef = Camera.main.transform;
		}
		rb = GetComponent<Rigidbody> ();
		line = GetComponent<LineRenderer> ();
	}
	void Update () {
		//Calucalte movement from world to camera space beforehand
		movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
		AlignToCamera (ref movementInputs);
	}
	void FixedUpdate () {
		//DefultDiplayLine
		line.SetPosition (0, transform.position + Vector3.up * lineOffset);
		line.SetPosition (1, transform.position + Vector3.up * lineOffset);

		switch (PlayerType){
		case moveBy.ToyCar:
			ToyCar (movementInputs);
			break;
		case moveBy.ForwardDriven:
			SteerFromRot(movementInputs);
			break;
		case moveBy.DirectTooMovement:
			MoveToPos(movementInputs);
			break;
		}
	}

	//Alinges inputs to direction the camera is facing + falttening it out on the gloabl Y axis
	void AlignToCamera(ref Vector3 InputsDir ){
		Vector3 camForward = cameraRef.forward;
		camForward.y = 0;
		InputsDir = Quaternion.LookRotation (camForward) * InputsDir; 
	}

	//Here the player moves only in it's local forward axis. In order to reach a spesfic point the player is rotate, over time, to face it.
	void SteerFromRot(Vector3 Inputs){
		if (Inputs.magnitude > .01f) {
			//Slerp/Lerp so you can set the players rotational speed
			storedRot = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Inputs), rotationSpeed * Time.deltaTime);
		
			rb.MoveRotation (storedRot);
			//The vector added here is the players forward direction, mutipled by the inputs magnitude (so you still get speed amount)
			rb.MovePosition (transform.position + (transform.forward * Inputs.magnitude * moveSpeed * Time.deltaTime));
		}
		line.SetPosition (1, transform.position + Vector3.up * lineOffset + (transform.forward * Inputs.magnitude  * lineScale));
	}

	//Movement is driven by positioning the player directly towards a target coordinate. Rotation is added as a visual addition(that in no way effects the movement towards the target point)
	void MoveToPos(Vector3 Inputs){
		if (Inputs.magnitude > .01f) {
			//Calculate new rotation direction from inputs
			storedRot = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Inputs), rotationSpeed * Time.deltaTime);
		
			rb.MoveRotation (storedRot);
			//Move player directly towards the target point
			rb.MovePosition (transform.position + (Inputs * moveSpeed * Time.deltaTime));
		}
		line.SetPosition (1, transform.position + Vector3.up * lineOffset + (Inputs  * lineScale));
	}

	//Not a fundmental, I just like threes and found this fun when scripting - Enjoy the Toy car 8) 
	void ToyCar(Vector3 Inputs){	
		if (Inputs.magnitude > .01f) {
			//We need to be able to move forward or back, so we caluclate this outside the MovePosition so it can be adjusted
			float finalSpeed = Inputs.magnitude * moveSpeed;

			//Check if target direction is diffrent enough for Reverseing, then adjust the inputs as needed -- NOTE: Added button requiment for exsample scene
			if (Vector3.Angle (Inputs, transform.forward) > reverseAngle && Input.GetButton("Jump")) {
				//This inverts left and right input directions when reversing
				Inputs = -Inputs;
				//Making this a neagtive value, will invert the transfrom.forward to a neagtive when multiplied, making the player move backwards
				finalSpeed = -finalSpeed;
				Debug.DrawRay (transform.position, Inputs.normalized, Color.blue);
			} else {
				Debug.DrawRay (transform.position, Inputs.normalized, Color.magenta);
			}

			//Added Inputs.magnitude to this one. This makes it so rotational change is proportional to movement, making it's movement more car like -- (no pivoting on the spot!)
			storedRot = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Inputs), (rotationSpeed * Inputs.magnitude) * Time.deltaTime);

			rb.MoveRotation (storedRot);
			//Same as before except the move speed is calucated above so we could invert it(for reversing) if conditions are met
			rb.MovePosition (transform.position + (transform.forward * finalSpeed * Time.deltaTime));

			//line.SetPosition (1, transform.position + Vector3.up * lineOffset + (transform.forward * (rotationSpeed * Inputs.magnitude) * lineScale));

			line.SetPosition (1, transform.position + Vector3.up * lineOffset + (transform.forward * Inputs.magnitude * lineScale));
		}

	}
}
