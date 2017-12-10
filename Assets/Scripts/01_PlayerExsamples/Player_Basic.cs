using UnityEngine;

public class Player_Basic : MonoBehaviour {
	[SerializeField]
	private float acceleration = 4;
	[SerializeField]
	private float moveSpeed = 4;
	[SerializeField]
	private float rotationSpeed = 12;
	[SerializeField]
	private float jumpForce = 4;
	[SerializeField]
	private float jumpBuffer = .1f;
	[SerializeField]
	[Tooltip("Well auto set if left at 0")]
	private float gravityMut;

	private Vector3 movement; 
	private Quaternion storedRot;
	[SerializeField]
	private float verticalSpeed;
	private float jumpAllowance;
	private bool grounded;

	private Rigidbody rb;
	[SerializeField]
	private Animator anim;
	[SerializeField]
	private Transform forRef;


	void Start () {
		//If no base gravity had been set just take the defult
		if(gravityMut == 0){
			gravityMut = Physics.gravity.y;
		}
		rb = GetComponent<Rigidbody> ();
		storedRot = transform.rotation;
	}

	void Update () {
		//Get inputs - While movements are applied in FixedUpdate you still want to get your inputs from normal Update (It's more responsive)
		Vector3 movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
		//Orientate Inputs to cameras direction - Ref makes it so any changes done to value in function are actrally applied
		AlignToCamera ( ref movementInputs);
		//Extra + Adds some drag to our inputs, makes the chararters movements feel more weighted
		movement = Vector3.Lerp (movement, movementInputs, acceleration * Time.deltaTime);

		//In game visual feedback and Gizmos, not essential, just helpful
		FeedBack ();
	}

	//When moving physics objects do the actal movement through FixedUpdate - Not doing so can cause drastic speed changes at diffrent frame rates.
	void FixedUpdate () {
		//gives visual feedback for jump in Gizmos, not needed for actral movement
		JumpFeedbackInfo ();

		MovePosition (movement);

	}

	void AlignToCamera(ref Vector3 InputsDir ){
		//Gets cameras forward as a direction we can use and then apply to our input direction
		Vector3 camForward = forRef.forward;
		//Flattens direction for use, stops our input being offset up or down
		camForward.y = 0;
		// Mutliply a Quaternion by a Vector and it offsets it. NOTE - Vector multiplied by Quaternion won't work, must be ordered Quaternion multiplied by Vector.
		InputsDir = Quaternion.LookRotation (camForward) * InputsDir; 
	}

	void MovePosition(Vector3 FinalMovement){	
		//Applies Rotation - Note in this set up, rotation has no baring on actral movement direction
		//Only applies new rotation when there is substanial input, stops chararter being set to unexspected directions when the joystick is released
		if(FinalMovement.magnitude > .05f){
			//Make rotation toward target direction -- Do this before applying vertical movement to FinalMovement
			storedRot = Quaternion.Slerp (storedRot, Quaternion.LookRotation (FinalMovement), rotationSpeed * Time.deltaTime);
		}
		rb.MoveRotation (storedRot);

		//Calculate vertical movement of gravity and Jumping and if Jumping is valid
		//Check if player is grounded with raycast down
		if (Physics.Raycast (transform.position + Vector3.up, -Vector3.up, 1.1f)) {
			anim.SetBool ("Grounded", true);
			//Reset gravity before adding to it bellow,
			verticalSpeed = 0;
			//Reset jump buffer for next use
			jumpAllowance = 0;
			grounded = true;
			//This addintinal middle 'else if' statment allows us to give the player some leeway when jumping off edges where jumping can still be valid for some time
		} else if (jumpAllowance > -jumpBuffer) {
			jumpAllowance -= Time.deltaTime;
			//Once buffer is over, set player to not grounded
		} else {
			grounded = false;
			anim.SetBool ("Grounded", false);
		}
		//If character is grounded(Or still within jump buffer time) allow jump to be triggered
		if (Input.GetButtonDown ("Jump") && grounded) {
			//Set vertical speed to jump
			verticalSpeed = jumpForce;
			anim.SetTrigger ("Jump");
			grounded = false;
		}
		//Apply continuous Gravity
		//Add gravity overtime, when not grounded it will gradually increase to full momentum
		verticalSpeed += gravityMut * Time.deltaTime;
		//You want to clamp the gravity so it doesn't keep incressing constantly when player is not grounded
		verticalSpeed = Mathf.Clamp (verticalSpeed, gravityMut, jumpForce);
		//Add vertical motion to be offset to the FinalMovement so it's added along with the basic inputs
		FinalMovement.y += verticalSpeed;

		//Apply movement
		rb.MovePosition (transform.position + (FinalMovement * moveSpeed * Time.deltaTime));
		anim.SetFloat ("Speed", movement.magnitude * moveSpeed );
	}




	void FeedBack(){
		Vector3 movementInputs = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		Debug.DrawRay (transform.position + Vector3.up * 2, Vector3.up * (movementInputs.magnitude - movement.magnitude) * 2, Color.magenta);
		Debug.DrawRay (transform.position, movement * 2, Color.magenta);

		Vector3 camForward = Camera.main.transform.forward;
		camForward.y = 0;

		Vector3 movementInputWorld = Quaternion.LookRotation (camForward) * movementInputs;

		Debug.DrawRay (transform.position, movementInputWorld * 2, Color.green);
		if (Physics.Raycast (transform.position + (Vector3.up * 1), -Vector3.up, 1.1f)) {
			Debug.DrawRay (transform.position + (Vector3.up * 1), -Vector3.up * 1.1f, Color.blue);
		} else {
			Debug.DrawRay (transform.position + (Vector3.up * 1), -Vector3.up * 1.1f, Color.red);
		}

	}
	void JumpFeedbackInfo(){
		if (Input.GetButtonDown ("Jump")) {
			if (Physics.Raycast (transform.position + Vector3.up, -Vector3.up, 1.1f)) {
				Debug.DrawRay (transform.position + (Vector3.up * 1), -Vector3.up * 1.1f, Color.blue, 3);
			} else if (grounded) {

				Debug.DrawRay (transform.position + (Vector3.up * 1), -Vector3.up * 1.1f, Color.magenta, 3);

			} else {
				Debug.DrawRay (transform.position + (Vector3.up * 1), -Vector3.up * 1.1f, Color.red, 3);

			}
		}
	}
}


//Jump Simply Using AddForce - Which I prefer but isn't recomend for MovePosition(though i've found no real issue 8|) NOTE:Use with Rigidbody graivty on
/*	if (Input.GetButtonDown ("Jump")) {
			//Check is there is any surface bellow the player to allow jump
			if (Physics.Raycast (transform.position + (Vector3.up * groundedRayOffset), -Vector3.up,  groundedRay)) {
				//ForceMode.Impulse is made for one frame burst, perfact for this
				rb.AddForce (Vector3.up * jumpForce, ForceMode.Impulse);
				anim.SetTrigger ("Jump");
			} 
		}
*/