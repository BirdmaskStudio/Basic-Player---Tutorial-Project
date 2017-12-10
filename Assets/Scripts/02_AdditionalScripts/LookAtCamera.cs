using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
	[SerializeField]
	private Transform hoverTarget;
	[SerializeField]
	private float hoverAmount = 1.5f;
	[SerializeField]
	private Transform cameraRef;
	// Use this for initialization
	void Start () {
		if (cameraRef == null) {
			cameraRef = Camera.main.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = hoverTarget.position + Vector3.up * hoverAmount;
		transform.LookAt (cameraRef.position);
	}
}
