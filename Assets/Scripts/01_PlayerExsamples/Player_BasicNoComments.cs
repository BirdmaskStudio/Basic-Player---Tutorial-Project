using UnityEngine;

public class Player_BasicNoComments : MonoBehaviour {
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
		if(gravityMut == 0){
			gravityMut = Physics.gravity.y;
		}
		rb = GetComponent<Rigidbody> ();
		storedRot = transform.rotation;
	}

	void Update () {
		Vector3 movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
		AlignToCamera ( ref movementInputs);
		movement = Vector3.Lerp (movement, movementInputs, acceleration * Time.deltaTime);
	}

	void FixedUpdate () {
		MovePosition (movement);
	}

	void AlignToCamera(ref Vector3 InputsDir ){
		Vector3 camForward = forRef.forward;
		camForward.y = 0;
		InputsDir = Quaternion.LookRotation (camForward) * InputsDir; 
	}

	void MovePosition(Vector3 FinalMovement){	
		if(FinalMovement.magnitude > .05f){
			storedRot = Quaternion.Slerp (storedRot, Quaternion.LookRotation (FinalMovement), rotationSpeed * Time.deltaTime);
		}
		rb.MoveRotation (storedRot);

		if (Physics.Raycast (transform.position + Vector3.up, -Vector3.up, 1.1f)) {
			anim.SetBool ("Grounded", true);
			verticalSpeed = 0;
			jumpAllowance = 0;
			grounded = true;
		} else if (jumpAllowance > -jumpBuffer) {
			jumpAllowance -= Time.deltaTime;
		} else {
			grounded = false;
			anim.SetBool ("Grounded", false);
		}
		if (Input.GetButtonDown ("Jump") && grounded) {
			verticalSpeed = jumpForce;
			anim.SetTrigger ("Jump");
			grounded = false;
		}
		verticalSpeed += gravityMut * Time.deltaTime;
		verticalSpeed = Mathf.Clamp (verticalSpeed, gravityMut, jumpForce);
		FinalMovement.y += verticalSpeed;

		rb.MovePosition (transform.position + (FinalMovement * moveSpeed * Time.deltaTime));
		anim.SetFloat ("Speed", movement.magnitude * moveSpeed );
	}
}