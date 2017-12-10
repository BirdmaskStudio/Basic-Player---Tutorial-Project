using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickCompass : MonoBehaviour {
	[SerializeField]
	private Image compass;
	[SerializeField]
	private Vector3 angle;
	[SerializeField]
	private float angles;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 movementInputs = new Vector3 (Input.GetAxis("Horizontal") , Input.GetAxis("Vertical"),0);



		if (movementInputs.magnitude > .05f) {
			compass.color = Color.green;
			transform.LookAt(transform.position + movementInputs ,- Vector3.forward);
			Debug.DrawRay (transform.position, movementInputs * 90,Color.green);
		} else {

			compass.color = Color.white;
		}
		Vector3 newScale = new Vector3 (1,1,(movementInputs.magnitude/2 )+ .5f);
		transform.localScale = newScale;
	}
}
