using UnityEngine;
using System.Collections;

public class CameraRig_SplineTracker : MonoBehaviour {
	[Header("Current Camera Rig Splines")]
	[SerializeField]
	private CameraTrack_Spline playerSpline;
	[SerializeField]
	private CameraTrack_Spline cameraSpline;
	[SerializeField]
	private CameraTrack_Spline lookAtSpline;
	[Header("Player Check Smoothness")]
	[SerializeField]
	private int lineCheckFrequency = 22;

	[Header("Move on Spline Iteration Settings")]
	[SerializeField]
	private float cameraSplineDamp = 5f;
	[SerializeField]
	private float lookAtSplineDamp = 5f;
	[SerializeField]
	private float cameraLookAtDamp = 55;
	[Header("Off Track Move Iteration Settings")]
	[SerializeField]
	private bool offTrackMovement;
	[SerializeField]
	private float cameraLerpDamp =5 ;
	[SerializeField]
	private float lookAtLerpDamp =5 ;

	[Header("Spline Objects")]
	[SerializeField]
	public Transform playerRefreance;
	[SerializeField]
	private Transform cameraObject;
	[SerializeField]
	private Transform cameraAxisObject;
	[SerializeField]
	private Transform lookAtObj;

//Used Values
	[Header("Temp--Inner Workings")]
	[SerializeField]
	public int currentGroup;
	[SerializeField]
	private int currentLine;
	[SerializeField]
	public float currentPoint;	//The placement from the player
	[SerializeField]
	private float modifiedPoint; //If modifiers are on this is their placment
	[SerializeField]
	public float cameraPoint;
	[SerializeField]
	private float lookAtPoint;

	[Header("Selected Modifier")]
	[SerializeField]
	private CameraModifier_SplineTracker modifier;

	// Update is called once per frame
	void Start(){
		if(GetComponent<CameraModifier_SplineTracker> ())
			modifier = GetComponent<CameraModifier_SplineTracker> ();

		if(lookAtSpline == null)
			lookAtSpline = playerSpline;

		if (!playerRefreance)
			playerRefreance = gameObject.transform;
	}

	void Update () {
	//Get cloest point info
		currentLine = playerSpline.GetClosestPointToPos (playerRefreance.position);
		currentGroup = playerSpline.GetPointGroup (currentLine);
		int StartPoint = currentLine;
		int EndPoint = currentLine;
		playerSpline.GetGroupRange (currentGroup, ref StartPoint, ref EndPoint);
	//Calucalte Player Posistion On Line Group
		MakeCheckpoints (StartPoint,EndPoint);
	//Apply progress to output lines	
		Debug.DrawLine (playerSpline.GetPointWithinGroup(currentPoint,StartPoint,EndPoint), playerRefreance.position, Color.red);
		if (modifiedPoint < .05f && cameraPoint > .95f || modifiedPoint > .95f && cameraPoint < .05f ) {
			cameraPoint = modifiedPoint;
			lookAtPoint = modifiedPoint;
		}
		Vector3 camTrackPos = lerpPoint (cameraObject, cameraSpline, currentGroup, ref cameraPoint, modifiedPoint, cameraSplineDamp);
		Vector3 lookTrackPos = lerpPoint (lookAtObj, lookAtSpline, currentGroup, ref lookAtPoint, modifiedPoint, lookAtSplineDamp);
		if(offTrackMovement){
			lookAtObj.position = Vector3.Lerp(lookAtObj.position,lookTrackPos,lookAtLerpDamp * Time.deltaTime);
			cameraObject.position = Vector3.Lerp(cameraObject.position,camTrackPos,cameraLerpDamp * Time.deltaTime);


		}else{
			lookAtObj.position = lookTrackPos;
			cameraObject.position = camTrackPos;
		}
		if (cameraAxisObject != null) {
			cameraSpline.GetGroupRange (currentGroup, ref StartPoint, ref EndPoint);
			cameraAxisObject.position = cameraSpline.GetPoint(cameraSpline.GetPointWithinGroupFull (modifiedPoint, (float)StartPoint, (float)EndPoint));
		}
		//Check Range
		Debug.DrawRay (cameraSpline.GetPoint( cameraSpline.GetPointWithinGroupFull (1, (float)StartPoint, (float)EndPoint)), transform.up, Color.white);
		Debug.DrawRay (cameraSpline.GetPoint( cameraSpline.GetPointWithinGroupFull (0, (float)StartPoint, (float)EndPoint)), transform.up, Color.white);
		CameraLookAt ();
	}

	void MakeCheckpoints(int StartPoint,int EndPoint){
		float tempDis = 10000f;
		float tempPoint = currentPoint;
		for (float i = 0; i < lineCheckFrequency; i++) {
			Vector3 checkPoint = playerSpline.GetPointWithinGroup((i/lineCheckFrequency),StartPoint,EndPoint);
			Debug.DrawRay (checkPoint, transform.up, Color.yellow);
			if (Vector3.Distance (checkPoint, playerRefreance.position) < tempDis) {
				Debug.DrawLine (checkPoint, playerRefreance.position, Color.magenta);
				tempDis = Vector3.Distance (checkPoint, playerRefreance.position);
				tempPoint = i/lineCheckFrequency;
			}
		}
		currentPoint = tempPoint;
		if(modifier != null){
			//Converts point from group to the full line - THis prevents jumps when transition between groups
			modifiedPoint = modifier.ModifyValue (currentPoint,currentGroup);
		}
	}
	//Due to group transistions, this method jump around when the target posistion that was 0 becomes 1
	Vector3 lerpPoint(Transform target,CameraTrack_Spline spline,int currentGroup,ref float currentPoint, float targetPoint,float damping){ // make this genral - to place all features by a inputed(float placment, float damping, CameraTrack_Spline spline)
		int StartPoint = currentLine;
		int EndPoint = currentLine;
		spline.GetGroupRange (currentGroup, ref StartPoint, ref EndPoint);

		targetPoint = spline.GetPointWithinGroupFull (targetPoint, (float)StartPoint, (float)EndPoint);
		currentPoint = Mathf.Lerp (currentPoint, targetPoint, damping * Time.deltaTime);
		Debug.DrawRay (spline.GetPointWithinGroup (targetPoint, StartPoint, EndPoint), transform.up *2, Color.white);
	//	Debug.DrawRay (spline.GetPointWithinGroup (currentPoint, StartPoint, EndPoint), transform.up, Color.green);
		//return spline.GetPoint (currentPoint, StartPoint, EndPoint);
		return spline.GetPoint (currentPoint);
	}

	void CameraLookAt(){
		Quaternion rotation = Quaternion.LookRotation (lookAtObj.position - cameraObject.position);
		cameraObject.rotation = Quaternion.Slerp (cameraObject.rotation, rotation, Time.deltaTime * cameraLookAtDamp);
	}

	public void RevealInfo(){
		//playerSpline.get
	}
}
