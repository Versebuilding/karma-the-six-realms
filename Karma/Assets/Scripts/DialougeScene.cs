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
	public static float buttonDelayConst = 0.75f;

	public static float typingSpeed = 0.03f;

	private static string JSONPath = Path.Combine(Application.dataPath, "DialogeScenes", "template.json");
	private DialogeTree dTree = new DialogeTree(JSONPath);

	private UIDocument ui;
	private VisualElement root;
	private Label textComponent;
	private List<UnityEngine.UIElements.Button> buttons;
	private List<UnityEngine.UIElements.Label> button_labels;
	private List<System.Action> button_handlers;

	void Start()
	{
		this.ui = GetComponent<UIDocument>();
		this.root = this.ui.rootVisualElement;
		this.textComponent = this.root.Query<Label>("BILLY").First();

		this.buttons =
			root.Query<UnityEngine.UIElements.Button>(className: "option-button")
			.ToList();

		this.button_labels =
			root.Query<UnityEngine.UIElements.Label>(className: "option-label")
			.ToList();

		this.button_handlers = new List<System.Action>();
		for (int i = 0; i < buttons.Count; i++)
		{
			//sorry about the random variable, but c# lambdas work wierd
			//and this is the only way it compiles
			//for more info see: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
			int buttonIndex = i;
			System.Action bPress = () => this.SelectDialougeOption(buttonIndex);
			buttons[i].clicked += bPress;
			button_handlers.Add(bPress);
		}

		StartCoroutine(StartCurrentEvent());
	}

	IEnumerator LastEvent()
	{
		yield return new WaitForSecondsRealtime(DialogeScene.buttonDelayConst);
		this.button_labels[0].text = "Ok";
		this.buttons[0].clicked -= this.button_handlers[0];
		this.buttons[0].clicked += () => this.End();
		this.buttons[0].style.display = DisplayStyle.Flex;
		yield break;
	}

	IEnumerator StartCurrentEvent()
	{
		Debug.Log(this.dTree.IsOngoing());
		if (!this.dTree.IsOngoing())
		{
			Debug.Log("not ongoing");
			this.End();
			yield break;
		}


		string speech = this.dTree.GetSpeech();

		if (speech == null)
		{
			Debug.Log("NULL SPEACH DETECTED");
		}

		yield return StartCoroutine(RevealAndScrollText(speech));

		if (this.dTree.IsLastEvent())
		{
			Debug.Log("starting corr");
			StartCoroutine(LastEvent());
			yield break;
		}

		Box buttonBox = this.root.Query<Box>(className: "button_container");

		yield return new WaitForSecondsRealtime(DialogeScene.buttonDelayConst);

		List<string> opts = this.dTree.GetResponseOptions();

		for (int i = 0; i < opts.Count; i++)
		{
			button_labels[i].text = opts[i];
			buttons[i].style.display = DisplayStyle.Flex;
		}
	}

	IEnumerator RevealAndScrollText(string text)
	{
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
