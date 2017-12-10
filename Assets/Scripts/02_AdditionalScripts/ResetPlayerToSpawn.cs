using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerToSpawn : MonoBehaviour {
	[SerializeField]
	private Transform playerObj;
	[SerializeField]
	private Transform spawnObj;
	[SerializeField]
	private float resetDis = -10;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (playerObj.position.y < resetDis || Input.GetButtonDown("Reset")) {
			playerObj.position = spawnObj.position;

		}
	}
}
