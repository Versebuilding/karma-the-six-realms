using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections;
using TMPro; // If using TextMeshPro

public class DialougeScene : MonoBehaviour
{
	public string[] options =
	{
		"run in a cirle",
		"kill some wild animals",
		"hunt your own father for sport",
		"give a homeless man some money"
	};

	public string fullText = "this is some sample text";
	public float typingSpeed = 0.03f;

	private Label textComponent;

	void Start()
	{
		UIDocument ui = GetComponent<UIDocument>();
		this.textComponent = ui.rootVisualElement.Q<Label>("speach");
// 		List<Button> buttons = ui.Q<Button>().toList();
//
// 		for (int i = 0; i < buttons.Length; i++) {
// 			buttons[i].text = this.options[i];
// 		}

		//for testing purposes only
		Debug.Log(this.textComponent.text);

		StartCoroutine(RevealAndScrollText());
	}

	IEnumerator RevealAndScrollText()
	{
		this.textComponent.text = ""; // Start with empty text

		for (int i = 0; i < fullText.Length; i++)
		{
			Debug.Log(fullText[i]);
			this.textComponent.text += fullText[i];

			yield return new WaitForSeconds(typingSpeed);
		}
	}
}
