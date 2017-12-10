using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraTrack_Spline))]
public class CameraTrack_SplineInspector : Editor {

	private const int stepsPerCurve = 10;
	private const float directionScale = 0.5f;
	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private CameraTrack_Spline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private int selectedIndex = -1;

	private Vector3 targetPos;
	private float distance;

	public override void OnInspectorGUI () {
		spline = target as CameraTrack_Spline;
		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
	//	targetPos = EditorGUILayout.Vector3Field ("TargetPos",targetPos);
	//	EditorGUILayout.FloatField ("Distance", distance);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.Loop = loop;
		}
		if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
			DrawSelectedPointInspector();
		}
			
		EditorGUILayout.LabelField("Curve Editing");
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Insert Curve")) {
			Undo.RecordObject(spline, "Add Curve At Point");
			spline.AddCurveAtPoint(ref selectedIndex);
			EditorUtility.SetDirty(spline);
		}
		EditorGUI.BeginDisabledGroup (spline.points.Length == 4);
		if (GUILayout.Button("Remove Curve")) {
			Undo.RecordObject(spline, "Add Curve At Point");
			spline.removeCurveAtPoint(ref selectedIndex);
			EditorUtility.SetDirty(spline);
		}
		EditorGUI.EndDisabledGroup ();
		GUILayout.EndHorizontal ();
		GUILayout.Space (5);
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Add Curve Start")) {
			Undo.RecordObject(spline, "Add Curve Start");
			spline.AddCurve(false);
			selectedIndex = 0;
			EditorUtility.SetDirty(spline);
		}
		if (GUILayout.Button("Add Curve End")) {
			Undo.RecordObject(spline, "Add Curve End");
			spline.AddCurve(true);
			selectedIndex = spline.points.Length -1;
			EditorUtility.SetDirty(spline);
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.LabelField("Group Editing");
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Start New Group")) {
			Undo.RecordObject(spline, "Add Spline Group");
			spline.AddGroup(selectedIndex);
			EditorUtility.SetDirty(spline);
		}
		if (GUILayout.Button("Merge Current Group")) {
			Undo.RecordObject(spline, "Merge Spline Group");
			spline.MergeGroup(selectedIndex);
			EditorUtility.SetDirty(spline);
		}
		GUILayout.EndHorizontal ();
		if (GUILayout.Button("Erase Group")) {
			Undo.RecordObject(spline, "Erase Current Group");
			spline.RemoveGroupAndPoints(selectedIndex);
			EditorUtility.SetDirty(spline);
		}
		if (GUILayout.Button("Check Groups")) {
			Undo.RecordObject(spline, "Check All Groups");
			spline.CheckGroup();
			EditorUtility.SetDirty(spline);
		}
	}

	private void DrawSelectedPointInspector() {
		GUILayout.Label("Selected Info:");
		GUILayout.BeginHorizontal ();
		GUILayout.Label("Current Point: " + selectedIndex);
		GUILayout.Label("Main Point: " + (((selectedIndex + 1) / 3) * 3));
		GUILayout.Label("Group: " + spline.GetPointGroup(selectedIndex));
		GUILayout.EndHorizontal ();
		//int poinrGroup = EditorGUILayout.in("Mode", spline.GetControlPointMode(selectedIndex));


		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}
		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}

	}

	private void OnSceneGUI () {
		spline = target as CameraTrack_Spline;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);
		for (int i = 1; i < spline.ControlPointCount; i += 3) {
			Vector3 p1 = ShowPoint(i);
			Vector3 p2 = ShowPoint(i + 1);
			Vector3 p3 = ShowPoint(i + 2);
			
			Handles.color = Color.gray;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p3);

			Color lineColour = Color.grey;
			int groupIndex = spline.GetPointGroup(i);

			if (groupIndex != spline.GetPointGroup (i + 1)) {
				lineColour = Color.gray;
			}
			else if(groupIndex == 0){
				lineColour = Color.blue;
			}else if(groupIndex == 1){
				lineColour = Color.green;
			}else if (groupIndex == 2){
				lineColour = Color.magenta;
			}else if(groupIndex == 3){
				lineColour = Color.yellow;
			}else if(groupIndex == 4){
				lineColour = Color.red;
			}else if(groupIndex > 4){
				lineColour = Color.cyan;
			}


				Handles.DrawBezier(p0, p3, p1, p2, lineColour, null, 2f);

			p0 = p3;
		}

	//	ShowDistanceToVector ();
	}
	/*
	void LateUpdate (){
		if (targetPos != null) {
			ShowDirections ();
		}
	}
*/
	private void ShowDirections () {
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0f);
	//	Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint(i / (float)steps);
		//	Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
		}
	}

	private void ShowDistanceToVector () {
		Handles.color = Color.red;
		Vector3 point = spline.GetPoint(0f);
		distance = Vector3.Distance (point, targetPos);
		Handles.DrawLine(point, targetPos);
		distance = 10000;
		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint(i / (float)steps);
			if(Vector3.Distance (point, targetPos) < distance ){
				distance = Vector3.Distance (point, targetPos);
				Handles.DrawLine(point, targetPos);
			}
		}

	}

	private Vector3 ShowPoint (int index) {
		Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
		float size = HandleUtility.GetHandleSize(point);
		if (index == 0) {
			size *= 2f;
		}
		Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
		if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap)) {
			selectedIndex = index;
			Repaint();
		}
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
			}
		}
		return point;
	}
}