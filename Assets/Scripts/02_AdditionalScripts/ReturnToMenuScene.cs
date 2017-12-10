using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuScene : MonoBehaviour {
	[SerializeField]
	private int levelNumb = 0;

	void Update () {
		if(Input.GetButtonDown("Menu")){
			SceneManager.LoadScene (levelNumb);
		}
	}
}
