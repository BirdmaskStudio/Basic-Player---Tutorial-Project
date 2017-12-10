using UnityEngine;
using System.Collections;

public class LazyAnimtion : MonoBehaviour {
	[Range(0,1)]
	public float speed;
	[Range(-2,2)]
	public float direction;
	[Range(-180,180)]
	public float angle;
	public Animator anime;

	[SerializeField]
	private float directionRange = 1f;
	[SerializeField]
	private float moveAngleSize = 1f;
	// Use this for initialization
	void Start () {
		anime = GetComponent<Animator> ();
	}
	void FixedUpdate()
	{
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		Vector3 movement = new Vector3 (h,0,v);
	//	QuickAngle ();
		AnimetionUpdate (h,movement.sqrMagnitude);

	}

	void QuickAngle()
	{
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");


	}


	void AnimetionUpdate (float h, float v) {
		//anime.SetFloat ("Speed",Mathf.Abs(v));
		anime.SetFloat ("Speed",speed);
		anime.SetFloat ("Angle",angle);
		anime.SetFloat ("Direction",direction);
	}


}

