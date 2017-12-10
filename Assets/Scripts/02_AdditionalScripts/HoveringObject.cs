using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringObject : MonoBehaviour {
	private Vector3 posRef;
	[SerializeField]
	private float SpinSpeed = 20;
	[SerializeField]
	private float rotateSpeed = 20;
	[SerializeField]
	private AnimationCurve hoverCurve;

	// Use this for initialization
	void Start () {
		hoverCurve.postWrapMode = WrapMode.Loop;
		posRef = transform.localPosition;	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = posRef + Vector3.up * hoverCurve.Evaluate (Time.time);
		transform.RotateAround (transform.position,Vector3.up , SpinSpeed * Time.deltaTime);
		transform.RotateAround (transform.position,transform.right , rotateSpeed * Time.deltaTime);
		//transform.Rotate (transform.right * rotateSpeed * Time.deltaTime);
	}
}
