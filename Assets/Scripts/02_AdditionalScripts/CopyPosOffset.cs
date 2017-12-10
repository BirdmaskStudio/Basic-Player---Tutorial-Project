using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosOffset : MonoBehaviour {
	[SerializeField]
	private Transform target;
	private Vector3 lastPos;
	[SerializeField]
	private Vector3 offset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//transform.rotation = Quaternion.LookRotation(target.position - lastPos);// * Quaternion.LookRotation(-Vector3.up);
		Debug.DrawRay(target.position, (target.position - lastPos).normalized, Color.white);
		transform.position = target.position;

		lastPos = transform.position + offset;
	}
}
