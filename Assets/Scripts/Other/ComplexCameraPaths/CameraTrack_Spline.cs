using UnityEngine;
using System;

public class CameraTrack_Spline : MonoBehaviour {

	[SerializeField]
	public Vector3[] points;

	[SerializeField]
	public int[] pointsState; // -1 is null,

	[SerializeField]
	public BezierControlPointMode[] modes;

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public int GetPointGroup (int index) {
		return pointsState[(index + 1) / 3];
	}

	public void AddGroup (int index) {
		Debug.Log ("Add Groups");
		int groupIndex = (index + 1) / 3;
		//pointsState[groupIndex] += 1;
		for(int i = groupIndex; i < (pointsState.Length); i++)
		{
			pointsState[i] += 1;
		}
	//	CheckGroup ();
		EnforceMode(index);
	}
	public void MergeGroup (int index) {
		if(GetPointGroup(index) == 0){
			CheckGroup ();
			return;
		}
		int groupIndex = (index + 1) / 3;
		int StartPoint = 0;
		int EndPoint = 0;
		GetGroupRange(GetPointGroup(index) ,ref StartPoint,ref EndPoint);
		int newGroup = GetPointGroup(StartPoint) - 1;

		for(int i = StartPoint/3; i <= EndPoint/3; i++){
			Debug.Log ("point " + i + " Merge Groups " + pointsState[i] + " new " + (newGroup));
			pointsState[i] = newGroup;
		}
		CheckGroup ();
	}
	public void RemoveGroupAndPoints (int index) {
		//Can't remove groups until removePointOnCurve, can also work by removing by the point ahead, not behind
		if(GetPointGroup(index) == 0){
			CheckGroup ();
			return;
		}
		Debug.Log ("Remove Group And Point");
		int StartPoint = 0;
		int EndPoint = 0;
		GetGroupRange(GetPointGroup(index),ref StartPoint,ref EndPoint);

		for(int i = 0; i <= (EndPoint - StartPoint)/3; i ++)
		{
			Debug.Log ("Remove Group Point" + i);
			int r = StartPoint;
			removeCurveAtPoint (ref r);
		}
		CheckGroup ();
	}

	public void CheckGroup(){
		int[] oldStates = new int[pointsState.Length];
		int currentGroup = 0;
		pointsState[0] = 0;
		for (int i = 0; i < (pointsState.Length); i++) {
			oldStates[i] = currentGroup;
			if (i < pointsState.Length - 1 && pointsState [i] != pointsState [i + 1]) {
				currentGroup += 1;
			}
		}
		pointsState = oldStates;
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	public int GetClosestPointToPos(Vector3 position){
		float dis = 100000000f;
		int nearestPoint = 0;
		for(int i = 0; i < points.Length; i += 3){
			float pointDis = Vector3.Distance (transform.position + points[i], position);
			if(pointDis < dis){
				//Debug.Log ("Line Pos " + i + ": " + (transform.position + points[i]) + " Object Pos: " + position);
				dis = pointDis;
				nearestPoint = i;
			}
		}
		return nearestPoint;
	}

	public void GetGroupRange(int SelectedGroup,ref int GroupStart ,ref int GroupEnd){
		GroupStart = -1;
		GroupEnd = -1;
		for(int i = 0; i < points.Length; i += 3){
			//Debug.Log ("CurrentLine: " + GetPointGroup (i) + " Selected Group " + SelectedGroup);
			if (GetPointGroup (i) == SelectedGroup) {
			//	Debug.Log ("Line " + i + " Is Valid");
				if (GroupStart == -1) {
					GroupStart = i;
					GroupEnd = i + 1;
				} else {
				//	Debug.Log ("Line " + i + " Set As End");
					GroupEnd = i;
				}
			}
		}
		//Debug.Log ("Start " + GroupStart + " End " + GroupEnd);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Length - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Length) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Length - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
		int i;
		//Place at end
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

	public Vector3 GetPointWithinGroup (float t,int start, int end) {
		int i;
		//Place at end
		if (t >= 1f) {
			t = 1f;
			i = ((end - start) -3);
		}
		else {
			t = Mathf.Clamp01 (t) * ((end - start) / 3);//CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		int offset = start;//(int)start/3;
		return transform.TransformPoint(Bezier.GetPoint(points[offset + i], points[offset + i + 1], points[offset + i + 2], points[offset + i + 3], t));
	}

	public float GetPointWithinGroupFull (float value,float start, float end) {
		
		//return ((1 / CurveCount) * (start)) ;
		float valueLarge = value * ((end - start)/3);
		return (((start / 3) + valueLarge) / (float)CurveCount);
	}

	public int GetPointAndEnds (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		//pointOne = i;
		return i;//transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}

	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}

	public void AddCurveAtPoint(ref int selectedIndex){
		selectedIndex = selectedIndex / 3 * 3;
		if (selectedIndex == 0) {
			selectedIndex = 3;
		}
	//Make new arrays to store calucaltions	
		Vector3[] StoredPoints = new Vector3[points.Length + 3];
		int[] StoredStates = new int[pointsState.Length + 1];
		BezierControlPointMode[] StoredModes = new BezierControlPointMode[modes.Length + 1];
	//Beacuse the begaining and end have two points, not 3, them must be set here.
		//Set values at start
		StoredPoints [0] = points [0];
		StoredPoints [1] = points [1];
		StoredModes[0] = modes[0];
		StoredStates[0] = pointsState[0];
		//Set values at end
		StoredPoints [StoredPoints.Length - 1] = points [points.Length - 1];
		StoredPoints [StoredPoints.Length - 2] = points [points.Length - 2];
		StoredModes[StoredModes.Length -1] = modes[modes.Length -1];
		StoredStates[StoredStates.Length -1] = pointsState[pointsState.Length -1];
		float newPoint = (1f/(float)CurveCount) * ((float)selectedIndex /3  - .5f) ; 
	//Itrate through array
		for (int i = 3; i < StoredPoints.Length - 3; i += 3) {
			if (i < selectedIndex) {										// i mid - -1 last point - +1 next point
				Debug.Log ("Point " + i + " Being Added");
				StoredPoints [i] = points [i];
				StoredPoints [i +1] = points [i +1];
				StoredPoints [i - 1] = points [i - 1];

				StoredModes[i/3] = modes[i/3];
				StoredStates[i/3] = pointsState[i/3];
			}else if(i == selectedIndex){
				Debug.Log ("Fresh Point " + i + " Being Added");
				Vector3 dir = transform.InverseTransformDirection( GetDirection (newPoint));
				StoredPoints [i] = transform.InverseTransformPoint (GetPoint (newPoint));// - transform.position;//Vector3.Lerp( points [i] , points [i-3], .5f);
				StoredPoints [i - 1] = StoredPoints [i] - dir;
				StoredPoints [i + 1] = StoredPoints [i] + dir;

				StoredModes[i/3] = modes[i/3 - 1];
				StoredStates[i/3] = pointsState[i/3 - 1];

				StoredPoints [i - 1] = StoredPoints [i] - dir*.5f;
				StoredPoints [i + 1] = StoredPoints [i] + dir*.5f;


				selectedIndex = i;
			}else{
				if(i+1 < StoredPoints.Length -1)
					StoredPoints [i+1] = points [i -2];
				StoredPoints [i] = points [i -3];
				StoredPoints [i-1] = points [i -4];

				StoredModes[(i/3)] = modes[(i/3) - 1];
				StoredStates[i/3] = pointsState[i/3 -1];
			}
		}
		points = StoredPoints;
		modes = StoredModes;
		pointsState = StoredStates;
		CheckGroup ();
	}

	public void removeCurveAtPoint(ref int selectedIndex){
		selectedIndex = selectedIndex / 3 * 3;
		if(points.Length == 4){
			Debug.Log("Can not reduce spline to dot");
			return;
		}else if(selectedIndex == points.Length -1){ //removes end - due to the for loop direction this must be done this way
			selectedIndex = points.Length - 4;
			Array.Resize(ref points, points.Length - 3);
			Array.Resize(ref modes, modes.Length - 1);
			Array.Resize (ref pointsState, pointsState.Length - 1);
			return;
		}
		//Make new arrays to store calucaltions	
		Vector3[] StoredPoints = new Vector3[points.Length - 3];
		int[] StoredStates = new int[pointsState.Length - 1];
		BezierControlPointMode[] StoredModes = new BezierControlPointMode[modes.Length - 1];
		//Itrate through array
		for (int i = 0; i < StoredPoints.Length; i += 3) {
			if (i < selectedIndex) {				
				StoredPoints [i] = points [i];
				StoredPoints [i +1] = points [i +1];
				if(i - 1 > 0)						//Acounts for first point
					StoredPoints [i - 1] = points [i - 1];

				StoredModes[i/3] = modes[i/3];
				StoredStates[i/3] = pointsState[i/3];
			}else{
				StoredPoints [i] = points [i + 3];
				if(i+1 < StoredPoints.Length -1)	//acounts for first point
					StoredPoints [i+1] = points [i + 4];
				if(i - 1 > 0)						//acounts for last point
					StoredPoints [i-1] = points [i +2];

				StoredModes[(i/3)] = modes[(i/3) + 1];
				StoredStates[i/3] = pointsState[i/3 +1];
			}
		}
		points = StoredPoints;
		modes = StoredModes;
		pointsState = StoredStates;
		CheckGroup ();
	}

	public void AddCurve (bool addAtEnd) {
		Vector3 dir = GetDirection ((addAtEnd) ? 1 : 0);
		//Make new arrays to store calucaltions	
		Vector3[] StoredPoints = new Vector3[points.Length + 3];
		int[] StoredStates = new int[pointsState.Length + 1];
		BezierControlPointMode[] StoredModes = new BezierControlPointMode[modes.Length + 1];


		Vector3 point = points[points.Length - 1];				//Save End Point to refrance
		int pointState = pointsState [pointsState.Length - 1];	//Get current length
		Array.Resize (ref pointsState, pointsState.Length + 1);	//
		Array.Resize(ref points, points.Length + 3);			//Change array size
		Array.Resize(ref modes, modes.Length + 1);




		if (!addAtEnd) {
			for (int i = 3; i < StoredPoints.Length; i ++) {
				Debug.Log ("Point Being swaped " + i + " to " + points [i - 3]);
				StoredPoints [i] = points [i - 3];
			}
			for (int i = 1; i < StoredStates.Length; i ++) {
				StoredStates [i] = pointsState[i - 1];
				StoredModes[i] = modes[i - 1]; //copys priovuse last mode
			}
			point = points[0] -(dir * 3);
			StoredPoints[0] = point;
			point += dir;
			StoredPoints[1] = point;
			point += dir;
			StoredPoints[2] = point;
			StoredStates [0] = pointsState[0];
			StoredModes[0] = modes[0]; //copys priovuse last mode

			points = StoredPoints;
			modes = StoredModes;
			pointsState = StoredStates;
		} else {
			point += dir;
			points[points.Length - 3] = point;
			point += dir;
			points[points.Length - 2] = point;
			point += dir;
			points[points.Length - 1] = point;
			pointsState [pointsState.Length -1] = pointState;
			modes[modes.Length - 1] = modes[modes.Length - 2]; //copys priovuse last mode
		}




	//	EnforceMode(points.Length - 4);	//Check all vectors are in line??
		CheckGroup ();
		if (loop) {
			points[points.Length - 1] = points[0];
			pointsState[pointsState.Length - 1] = pointsState[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}

	public void Reset () {
		points = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
		pointsState = new int[] {
			0,
			0
		};
	}
}