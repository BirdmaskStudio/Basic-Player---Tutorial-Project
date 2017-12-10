using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MoveTypes : MonoBehaviour {
	private enum playerMovementType
	{
		MovePosistion, AddForce, ChararcterController
	}
	[SerializeField]
	private playerMovementType type;

	//basic settings mutipliers for movement
	[SerializeField]
	private float moveSpeed = 4;
	[SerializeField]
	private float jumpForce = 4;
	[SerializeField]
	[Tooltip("This will defult to world graivty if not set")]
	private float gravityMut;

	[SerializeField]
	[Tooltip("Player will use this to offset user input by the cameras foward Axis")]
	private Transform cameraRef;

	//calculation values
	private Vector3 movement;
	private float verticalSpeed;
	private Quaternion storedRot;

	//required components
	private Rigidbody rb;
	private CharacterController charController;

	void Start () {
		//If these arn't set to something do it automatically to the defult
		if (cameraRef == null) {
			cameraRef = Camera.main.transform;
		}
		if(gravityMut == 0){
			gravityMut = Physics.gravity.y;
		}
		//rotation needs to be set to a valid defult
		storedRot = transform.rotation;

		//Get script essential components from object if they are there
		if(GetComponent<Rigidbody> ())
			rb = GetComponent<Rigidbody> ();
		if(GetComponent<CharacterController> ())
			charController = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Get player movement input and adjust for camera axis
		Vector3 movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
		AlignToCamera (ref movementInputs);
		movement = movementInputs;

		//Since chararter controller isn't physics driven you can just call for it in update
		if (type == playerMovementType.ChararcterController) {
			CharController (movement);
		}
	}

	/*Beacuse these systems move using the Rigidbody, like all physic movements, you should apply them within a FixedUpdate
		,not doing this can cause drastic diffrances in movement speed at higher frame rates
		You still want to calculate your user inputs in Update though	*/
	void FixedUpdate(){
		switch (type){
		case playerMovementType.MovePosistion:
			MovePosition (movement);
			break;
		case playerMovementType.AddForce:
			AddForce (movement);
			break;
		}
	}


	void AlignToCamera(ref Vector3 InputsDir ){
		Vector3 camForward = cameraRef.forward;
		camForward.y = 0;
		InputsDir = Quaternion.LookRotation (camForward) * InputsDir; 
	}

	void MovePosition(Vector3 FinalMovement){	
	//Set snapping rotation
		if (FinalMovement.magnitude > .05f) {
			//Make rotation toward target direction -- Do this before applying vertical movement to FinalMovement
			storedRot = Quaternion.LookRotation (FinalMovement);
		}
		rb.MoveRotation (storedRot);

	//Calculate vertical movement of gravity and Jump
		//You can use AddForce to jump with MovePosition, some choose to keep track of both jump and graivty like this though since the two systems can sometimes conflict
		//Check if player is grounded with raycast down
		if (Physics.Raycast (transform.position, -Vector3.up, 1.1f)) {
			//Reset gravity before adding to it bellow,
			verticalSpeed = 0;
			//If character is grounded allow jump to be triggered
			if (Input.GetButtonDown ("Jump")) {
				//Set vertical speed to jump
				verticalSpeed = jumpForce;
			}
		}
		//Add gravity overtime, when not grounded it will gradually increase to full momentum
		verticalSpeed += gravityMut * Time.deltaTime;
		//You want to clamp the gravity so it doesn't keep incressing constantly when player is not grounded
		verticalSpeed = Mathf.Clamp (verticalSpeed, gravityMut, jumpForce);
		//Add vertical motion to be offset to the FinalMovement so it's added along with the basic inputs
		FinalMovement.y += verticalSpeed;
	//Apply movement
		rb.MovePosition (transform.position + (FinalMovement * moveSpeed * Time.deltaTime));
	}	

	void CharController(Vector3 FinalMovement){
	//Set snapping rotation
		if (FinalMovement.magnitude > .05f) {
			//Make rotation toward target direction -- Do this before applying vertical movement to inputs
			storedRot = Quaternion.LookRotation (FinalMovement);
		}
		transform.rotation = storedRot;

	//Calculate vertical movement of gravity and Jump
		//Character Controllers do not follow physics. They can't go through walls but forces don't act on them. Beacuse of that you have to manually apply gravity.
		//The chararcterController has it's own grounded check function, it is better than raycast.
		if(charController.isGrounded){ 
			//Reset gravity before adding to it bellow.
			verticalSpeed = 0;
			if(Input.GetButtonDown ("Jump")){
				verticalSpeed = jumpForce;
			}
		}
		//Add gravity overtime, when not grounded it will gradually increase to full momentum.
		verticalSpeed += gravityMut * Time.deltaTime;
		//You want to clamp the gravity so it doesn't keep incressing constantly when player is not grounded.
		verticalSpeed = Mathf.Clamp (verticalSpeed, gravityMut, jumpForce);
		//Add vertical motion to be offset to the FinalMovement so it's added along with the basic inputs.
		FinalMovement.y += verticalSpeed;

	//Apply movement
		charController.Move( (FinalMovement * moveSpeed  * Time.deltaTime));
	}
		
	void AddForce(Vector3 finalMovement){
	//Apply movement
		rb.AddForce ((finalMovement * moveSpeed));

	//Set snapping rotation
		if (finalMovement.magnitude > .05f) {
			storedRot = Quaternion.LookRotation (finalMovement);
		}
		rb.MoveRotation (storedRot);

	//Jump
		//Check if player is grounded with raycast down
		if (Physics.Raycast (transform.position, -Vector3.up, 1.2f)) {
			//If character is grounded allow jump to be triggered
			if(Input.GetButtonDown ("Jump")){
				//ForceMode.Impulse is ment for AddForce events, ment to be triggered in a single frame, like jumps
				rb.AddForce (Vector3.up * jumpForce, ForceMode.Impulse);
			}
		}
	}
}
