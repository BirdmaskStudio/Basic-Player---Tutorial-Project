using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour {

	public void LoadLevel(int LevelNumb){
		SceneManager.LoadScene (LevelNumb);

	}
}
