using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectedText : MonoBehaviour, ISelectHandler {
	[SerializeField]
	private string sceneText;
	[SerializeField]
	private Text textBox;

	public void OnSelect (BaseEventData eventData) {
		textBox.text = sceneText;
	}
}
