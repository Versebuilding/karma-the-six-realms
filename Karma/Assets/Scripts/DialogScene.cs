using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class DialogScene : MonoBehaviour
{
	public static float buttonDelayConst = 0.75f;

	public static float typingSpeed = 0.03f;

	private DialogeTree dTree;

	private UIDocument ui;
	private VisualElement root;
	private Box rootPanel;
	private Label textComponent;
	private List<UnityEngine.UIElements.Button> buttons;
	private List<UnityEngine.UIElements.Label> button_labels;
	private List<System.Action> button_handlers;
	private string dialogTreePath;
	public System.Action<string> uiHandlerCallback;

	void Start()
	{
		if (!string.IsNullOrEmpty(dialogTreePath))
		{
			this.dTree = new DialogeTree(dialogTreePath);
		}
	}

	IEnumerator uicheck()
	{
		yield return new WaitForEndOfFrame();
    yield return new WaitForEndOfFrame(); // Wait 2 frames for UI to settle

		this.ui = GetComponent<UIDocument>();
		this.root = this.ui.rootVisualElement;
		this.rootPanel = this.root.Query<Box>("root-panel");
		this.textComponent = this.root.Query<Label>("speech");
		// Debug.Log("Root panel bounds: " + root.worldBound);
		// Debug.Log("Root panel resolved width: " + root.resolvedStyle.width);
		// Debug.Log("Root panel resolved height: " + root.resolvedStyle.height);
		//
		// Debug.Log("Root children count: " + this.root.childCount);
		foreach(var child in this.root.Children())
		{
			// Debug.Log("Child: " + child.name + " (type: " + child.GetType().Name + ")");
		}

		Box buttonBox = this.rootPanel.Query<Box>(className: "button_container").First();

		// Debug.Log("Container is null: " + (buttonBox == null));
		if (buttonBox != null)
		{
			// Force layout update
			buttonBox.MarkDirtyRepaint();
			yield return new WaitForEndOfFrame();
			// Debug.Log("=== PARENT CHAIN DEBUG ===");
			VisualElement current = buttonBox;
			int level = 0;

			while (current != null)
			{
				string indent = new string(' ', level * 2);
				// Debug.Log($"{indent}{current.name} ({current.GetType().Name})");
				// Debug.Log($"{indent}  Bounds: {current.worldBound}");
				// Debug.Log($"{indent}  Display: {current.resolvedStyle.display}");
				// Debug.Log($"{indent}  Width: {current.resolvedStyle.width}");
				// Debug.Log($"{indent}  Height: {current.resolvedStyle.height}");

				current = current.parent;
				level++;
				if (level > 10) break; // Safety check
			}
		}
		this.buttons =
			root.Query<UnityEngine.UIElements.Button>(className: "option-button")
			.ToList();

		this.button_labels =
			root.Query<UnityEngine.UIElements.Label>(className: "option-label")
			.ToList();

		this.button_handlers = new List<System.Action>();
		for (int i = 0; i < buttons.Count; i++)
		{
			//sorry about the random variables, but c# lambdas work wierd
			//and this is the only way it compiles
			//for more info see: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
			int buttonIndex = i;
			System.Action bPress = () => this.SelectDialougeOption(buttonIndex);
			buttons[i].clicked += bPress;
			buttons[i].style.display = DisplayStyle.None;
			button_handlers.Add(bPress);
		}
	}

	void Awake()
	{
		StartCoroutine(uicheck());
	}

	public void Initialize(string path)
	{
		this.dialogTreePath = path;
		this.dTree = new DialogeTree(path);
	}

	public void StartScene()
	{
		StartCoroutine(StartCurrentEvent());
	}

	IEnumerator StartCurrentEvent()
	{
		yield return uicheck();
		if (!this.dTree.IsOngoing())
		{
			this.End();
			yield break;
		}

		string speech = this.dTree.GetSpeech();
		Debug.Log("speach is (sce): " + speech);

		if (speech == null)
		{
			Debug.Log("NULL SPEACH DETECTED");
		}

		yield return StartCoroutine(RevealAndScrollText(speech));

		if (this.dTree.IsLastEvent())
		{
			StartCoroutine(LastEvent());
			yield break;
		}

		Box buttonBox = this.root.Query<Box>(className: "button_container");

		yield return new WaitForSecondsRealtime(DialogScene.buttonDelayConst);

		List<string> opts = this.dTree.GetResponseOptions();

		for (int i = 0; i < opts.Count; i++)
		{
			// Debug.Log("=== Button " + i + " Debug ===");
			// Debug.Log("Button found: " + (buttons[i] != null));
			// Debug.Log("Before - Display: " + buttons[i].style.display);
			// Debug.Log("Before - text: " + button_labels[i].text);

			buttons[i].style.display = DisplayStyle.Flex;
			button_labels[i].text = opts[i];

			// Debug.Log("After - Display: " + buttons[i].style.display);
			// Debug.Log("Before - text: " + button_labels[i].text);
			// Debug.Log("Resolved display: " + buttons[i].resolvedStyle.display);
			// Debug.Log("World bounds: " + buttons[i].worldBound);
			// Debug.Log("Parent display: " + buttons[i].parent?.resolvedStyle.display);

		}
	}

	IEnumerator RevealAndScrollText(string text)
	{
		if (this.ui == null)
		{
			// Debug.Log("ui is null");
			this.ui = GetComponent<UIDocument>();
		}

		if (this.root == null)
		{
			// Debug.Log("root is null");
			this.root = this.ui.rootVisualElement;
		}

		if (this.textComponent == null)
		{
			// Debug.Log("trying to find text comp");
			this.textComponent = this.root.Query<Label>("speech").First();
			// Debug.Log(this.textComponent == null);
		}

		this.textComponent.text = ""; // Start with empty text
		this.textComponent.MarkDirtyRepaint();

		Debug.Log("speach is " + this.dTree.GetSpeech());
		Debug.Log("printing text " + text);
		for (int i = 0; i < text.Length; i++)
		{
			this.textComponent.text += text[i];
			yield return new WaitForSeconds(typingSpeed);
		}
		Debug.Log($"Text reveal complete. Final text: '{this.textComponent.text}'");
	}

	public void SelectDialougeOption(int option)
	{
		if (!this.dTree.SelectOption(option))
		{
			Debug.Log("Attempted to select an invalid option");
		}

		Debug.Log("new text " + this.dTree.GetSpeech());
		this.ClearDialouge();
		StartCoroutine(StartCurrentEvent());
	}

	void End()
	{
		Debug.Log("Dialouge Ended");
		this.root.style.display = DisplayStyle.None;
		uiHandlerCallback?.Invoke("");
	}

	IEnumerator LastEvent()
	{
		yield return new WaitForSecondsRealtime(DialogScene.buttonDelayConst);
		this.button_labels[0].text = "Ok";
		this.buttons[0].clicked -= this.button_handlers[0];
		this.buttons[0].clicked += () => this.End();
		this.buttons[0].style.display = DisplayStyle.Flex;
		yield break;
	}

	void ClearDialouge()
	{
		Debug.Log("clearing dialouge");
		for (int i = 0; i < this.buttons.Count; i++)
		{
			this.buttons[i].style.display = DisplayStyle.None;
		}

		this.textComponent.text = "";
	}
}
