using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientateInputsToCamera : MonoBehaviour {
	[SerializeField]
	[Tooltip("You can use Camera.Main to get the active camera, this is just for demonstrating")]
	private Transform standInCameraObj;
	[SerializeField]
	private LineRenderer localInputsDisplay;
	[SerializeField]
	private LineRenderer calculatedInputsDisplay;
	[SerializeField]
	private LineRenderer camDirLine01;
	[SerializeField]
	private LineRenderer camDirLine02;

	private Vector3 movementInputs;
	[SerializeField]
	private float camMoveSpeed = 3;
	private Vector3 mouseDragStore;

	//("This script converts the direction made from movement inputs, to align themselves to the forward direction of the camera.")]
	void Start () {
		localInputsDisplay.SetPosition (0,localInputsDisplay.transform.position);
		calculatedInputsDisplay.SetPosition (0, calculatedInputsDisplay.transform.position);
	}
	
	void Update () {
		//Gets the raw inputs as a basic Direction.
		movementInputs = new Vector3 (Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));

		//Gets the cameras forward to use as a refrance direction, we will use to offset the inputs raw direction.
		Vector3 camForward = standInCameraObj.forward; // Camera.main.transform.position;
		//Sets the cameras forward refrance's Y to 0, we don't want the cameras up or down rotations in the final offest.
		camForward.y = 0;

		//The new direction, the input offset by the cameras forward
		//Inputs from the world axis, to the local axis of the camera(- x rotation)
		Vector3 calculatedDirection = Quaternion.LookRotation (camForward) * movementInputs; 


		/*Using Quaternion.LookRotation we turn a vector direction into a Quaternion rotation.

		Turning one of the two directions into a rotation means we can mutiply one vector by the other.
		This, instead of just combining two points, applies the converted directions diffrance to the remaining vector.
		
		Basicly if you took Quaternion.LookRotation(Vector.Right) and mutiplied it with a direction
		The the result would be the direction would be offset 90* to the right.

		This is simular to the transform.TransformDirection however doesn't require a extra transform object as a refrance.

		NOTE: You must go in order of Quaternion * Vector. Not Vector * Quaterion, the latter will simply result in a ERROR.
		*/


		Display (calculatedDirection, camForward);
	}














	void EditCamera(){
		mouseDragStore = mouseDragStore - Input.mousePosition;
		mouseDragStore = mouseDragStore.normalized;
		standInCameraObj.RotateAround (standInCameraObj.position, Vector3.up, camMoveSpeed * Time.deltaTime * -mouseDragStore.x);
		standInCameraObj.RotateAround (standInCameraObj.position, standInCameraObj.right, camMoveSpeed * Time.deltaTime * mouseDragStore.y);
		mouseDragStore = Input.mousePosition;
	}

	void Display(Vector3 calculatedDirection, Vector3 camForward){
		calculatedInputsDisplay.SetPosition (1, calculatedInputsDisplay.transform.position + (calculatedDirection * 3));

		localInputsDisplay.SetPosition (1,localInputsDisplay.transform.position + (movementInputs * 3));
		camDirLine01.SetPosition(0,camDirLine01.transform.position);
		camDirLine01.SetPosition(1,camDirLine01.transform.position  + (camForward.normalized * 2));
		camDirLine02.SetPosition(0,camDirLine02.transform.position);
		camDirLine02.SetPosition(1,camDirLine02.transform.position + (camForward.normalized * 5));

		//localInputsDisplay.transform.rotation = ();
		calculatedInputsDisplay.transform.rotation = (Quaternion.LookRotation( camForward));


		if (Input.GetMouseButton (0)) {
			EditCamera ();
		}
	} 
}
