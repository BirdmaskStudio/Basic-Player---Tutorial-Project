using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig_Controllable : MonoBehaviour {
	[SerializeField]
	private Transform target;
	[SerializeField]
	private float moveSpeed = 1;
	[SerializeField]
	private float followTriggerDis = 2;
	[SerializeField]
	private float mouseSen = 120;

	private Vector3 lookAtPos;
	private bool followActive = true;
	[SerializeField]
	private Transform cameraObj;
	// Use this for initialization
	void Start () {
		lookAtPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (followActive) {
			lookAtPos = Vector3.Lerp (lookAtPos, target.position, moveSpeed * Time.deltaTime);
			transform.position = lookAtPos;
			cameraObj.LookAt (lookAtPos);
			if (Vector3.Distance (target.position, lookAtPos) < .5f) {
				followActive = false;
			}
		} else {
			if (Vector3.Distance (target.position, lookAtPos) > followTriggerDis) {
				
				followActive = true;
			}
		}
		if(Input.GetMouseButton(0) || Mathf.Abs( Input.GetAxis ("Joystick2_X")) > .1f || Mathf.Abs(Input.GetAxis ("Joystick2_Y")) > .1f )
			ObitCam ();
	}

	void ObitCam(){
		//Note you need to make a second set of Mouse Y and X for the second cotroller Joystick (Axis 4 and 5)
		float camPitch = -Input.GetAxis ("Mouse Y");
	//	Debug.Log ("Pitch: " + camPitch + " || Angle: " + cameraObj.localEulerAngles.x);
		if(cameraObj.localEulerAngles.x > 85f && camPitch > 0 && cameraObj.localEulerAngles.x < 270f
			|| cameraObj.localEulerAngles.x < 5f && camPitch < 0 ){
			camPitch = 0;
		}
		if(Mathf.Abs( Input.GetAxis ("Mouse Y")) > .1f){
			cameraObj.RotateAround (lookAtPos, cameraObj.right, camPitch * mouseSen * Time.deltaTime );
		}
		if(Mathf.Abs( Input.GetAxis ("Mouse X")) > .1f){
			cameraObj.RotateAround (lookAtPos, Vector3.up, Input.GetAxis("Mouse X") * mouseSen * Time.deltaTime );
		}

	}
}
