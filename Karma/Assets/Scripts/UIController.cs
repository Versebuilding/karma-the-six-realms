using UnityEngine;

public class UIController : MonoBehaviour
{
	private GameObject dialogBox;
	private DialogScene dialogScene;

	private GameObject interactHint;

	private Keys keys;

	//TODO: Abstract this for multiple UIs
	void Start()
	{
		this.dialogBox = GameObject.Find("DialogBox");
		this.dialogScene = this.dialogBox.GetComponent<DialogScene>();
		this.dialogScene.uiHandlerCallback += HandleUIDone;
		this.dialogBox.SetActive(false);

		this.keys = GameObject.Find("KeyHandler").GetComponent<Keys>();

		this.interactHint = GameObject.Find("InteractHint");
		this.interactHint.SetActive(false);
	}

	public void ShowInteractHint()
	{
		this.interactHint.SetActive(true);
	}

	public void HideInteractHint()
	{
		this.interactHint.SetActive(false);
	}

	public void StartDialog(string FullDialogPath)
	{
		this.keys.TakeKeyControl();
		this.HideInteractHint();
		this.dialogScene.Initialize(FullDialogPath);
		this.dialogBox.SetActive(true);
		this.dialogScene.StartScene();
	}

	public void HandleUIDone(string message)
	{
		this.dialogBox.SetActive(false);
		this.keys.ReleaseKeyControl();
		//return keyboard control here
	}
}
