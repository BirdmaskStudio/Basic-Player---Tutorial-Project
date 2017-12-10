using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig_Lazy : MonoBehaviour {
	[SerializeField]
	private float speed = 1;
	[SerializeField]
	private Transform target;
	private Vector3 targetPos;

	void Update () {
		targetPos = Vector3.Lerp (transform.position, target.position, speed * Time.deltaTime);
		transform.LookAt (targetPos);
	
	}
}
