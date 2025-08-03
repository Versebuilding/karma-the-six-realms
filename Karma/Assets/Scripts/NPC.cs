using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC : Interactable
{
	[SerializeField] private static string dialogPath = "template.json";
	[SerializeField] private VisualTreeAsset dialogUIAsset; // Assign your .uxml file here
	[SerializeField] private StyleSheet dialogStyleSheet; // Optional: assign your .uss file here

	private static string FullDialogPath = Path.Combine(Application.dataPath, "DialogeScenes", dialogPath);
	private DialogScene dialogBox;

	public override void interact()
	{
		UIController uiController = GameObject.Find("UIHandler").GetComponent<UIController>();
		uiController.StartDialog(FullDialogPath);
	}
}
