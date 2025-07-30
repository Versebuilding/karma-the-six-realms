using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections;
using TMPro;

public class DialogeScene : MonoBehaviour
{
	public float typingSpeed = 0.03f;

	private static string JSONPath = Path.Combine(Application.dataPath, "DialogeScenes", "template.json");
	private DialogeTree dTree = new DialogeTree(JSONPath);

	private UIDocument ui;
	private VisualElement root;
	private Label textComponent;
	private List<UnityEngine.UIElements.Button> buttons;
	private List<UnityEngine.UIElements.Label> button_labels;

	void Start()
	{
		this.ui = GetComponent<UIDocument>();
		this.root = this.ui.rootVisualElement;
		// First, check if root is valid
		Debug.Log($"Root is null: {this.root == null}");
		Debug.Log($"Root name: {this.root.name}");
		Debug.Log($"Root type: {this.root.GetType()}");

		// List all children of root
		foreach (var child in this.root.Children())
		{
			Debug.Log($"Child: {child.name} (Type: {child.GetType()})");
		}

		// Try querying without type constraint
		var element = this.root.Query("BILLY").First();
		Debug.Log($"Found element: {element != null}");

		// If found, check its type
		if (element != null)
		{
			Debug.Log($"Element type: {element.GetType()}");
		}

		// Then try the specific Label query
		this.textComponent = this.root.Query<Label>("BILLY").First();

		this.buttons =
			root.Query<UnityEngine.UIElements.Button>(className: "option-button")
			.ToList();

		this.button_labels =
			root.Query<UnityEngine.UIElements.Label>(className: "option-label")
			.ToList();

		for (int i = 0; i < buttons.Count; i++)
		{
			//sorry about the random variable, but c# lambdas work wierd
			//and this is the only way it compiles
			//for more info see: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
			int buttonIndex = i;
			buttons[i].clicked += () => this.SelectDialougeOption(buttonIndex);
		}

		Debug.Log("Starting");

		StartCoroutine(StartCurrentEvent());
	}

	IEnumerator StartCurrentEvent()
	{
		if (!this.dTree.IsOngoing())
		{
			this.End();
			yield break;
		}

		Debug.Log("Starting first co");
		yield return StartCoroutine(RevealAndScrollText(this.dTree.GetSpeech()));

		Box buttonBox = this.root.Query<Box>(className: "button_container");

		yield return new WaitForSecondsRealtime(1f);

		List<string> opts = this.dTree.GetResponseOptions();

		Debug.Log("before loop");

		//show the options
		for (int i = 0; i < opts.Count; i++)
		{
			button_labels[i].text = opts[i];
			buttons[i].style.display = DisplayStyle.Flex;
		}
	}

	IEnumerator RevealAndScrollText(string text)
	{
		Debug.Log("2nd co");
		Debug.Log(text);
		this.textComponent.text = ""; // Start with empty text

		for (int i = 0; i < text.Length; i++)
		{
			this.textComponent.text += text[i];

			yield return new WaitForSeconds(typingSpeed);
		}
	}

	void ClearDialouge()
	{
		for (int i = 0; i < this.buttons.Count; i++)
		{
			this.buttons[i].style.display = DisplayStyle.None;
		}

		this.textComponent.text = "";
	}

	public void SelectDialougeOption(int option)
	{
		if (!this.dTree.SelectOption(option))
		{
			Debug.Log("Attempted to select an invalid option");
		}

		this.ClearDialouge();

		StartCoroutine(StartCurrentEvent());
	}

	void End()
	{
		Debug.Log("Ending the scene");
		this.root.style.display = DisplayStyle.None;
	}
}
