using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
	[SerializeField]
	private Text gearUI;
	public float gears;
	
	// Update is called once per frame
	void Start () {
		if(gearUI != null)
			gearUI.text = gears.ToString ();
	}

	void OnTriggerEnter(Collider col){
		//Checks collided objects tag
		if (col.tag == "Gear") {
			//Simply access the player stats script and adds to the gear count
			gears += 1;
			if(gearUI != null)
				gearUI.text = gears.ToString ();
			//Destroys coin object
			Destroy (col.gameObject);
		}

	}
}
