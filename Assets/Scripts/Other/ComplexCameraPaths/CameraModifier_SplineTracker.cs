using UnityEngine;
using System.Collections;

public class CameraModifier_SplineTracker : MonoBehaviour {


	public enum CameraSpline_Modifiers
	{
		NoEffects,
		EaseIn,
		EaseOut,
		EaseInOut,
		CustomEase,
		JumpCutStart,
		JumpCutEnd,	
		JumpCutInOut,		
	}
	[SerializeField]
	private int currentGroups;
	[SerializeField]
	private CameraRig_SplineTracker cameraRig;
	[SerializeField]
	private CameraSpline_Modifiers[] modifiers;
//	[SerializeField]
	private float[] cameraDamp;
	[SerializeField]
	private AnimationCurve[] customCurve;

	[SerializeField]
	private AnimationCurve easeIn;
	[SerializeField]
	private AnimationCurve easeOut;
	[SerializeField]
	private AnimationCurve easeBoth;

//	[SerializeField]
//	private float input;
	[SerializeField]
	private float output;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public float ModifyValue (float input,int currentGroup) {
		currentGroups = currentGroup;
		switch (modifiers[currentGroup]) 
		{
		case CameraSpline_Modifiers.NoEffects:
			output = input;
			break;
		case CameraSpline_Modifiers.EaseIn:
			//EaseEffects ();
			output = easeIn.Evaluate (input);
			break;
		case CameraSpline_Modifiers.EaseOut:
			output = easeOut.Evaluate (input);
		//	EaseEffects ();
			break;
		case CameraSpline_Modifiers.EaseInOut:
			output = easeBoth.Evaluate (input);
		//	EaseEffects ();
			break;
		case CameraSpline_Modifiers.CustomEase:
			output = customCurve[currentGroups].Evaluate (input);
			//	EaseEffects ();
			break;
		case CameraSpline_Modifiers.JumpCutStart:
			output = input;
			break;
		case CameraSpline_Modifiers.JumpCutEnd:
			output = input;
			break;
		case CameraSpline_Modifiers.JumpCutInOut:
			output = input;
			break;
		}
		return output;
	}

	void EaseEffects(){



	}
	void JumpCutEffects(){

	}
}
