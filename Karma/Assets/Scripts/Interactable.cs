using UnityEngine;

public class Interactable : MonoBehaviour
{
	[Header("Item Data")]
	[SerializeField] private string itemName;

	[Header("Dialogue Data")]


	[Header("Pickup Data")]

	private Player playerScript;
	private GameObject player;
	private bool playerIsClose;
	private Keys keyHandler;
	private UIController ui;

	void Start()
	{
		// Gets a reference to the player character - this is used for getting/setting variables
		// from them (this should be reworked once someone who actually knows how to code gets their hands on the project)
		this.player = GameObject.Find("Player");
		this.playerScript = player.GetComponent<Player>();
		this.keyHandler = GameObject.Find("KeyHandler").GetComponent<Keys>();
		this.ui = GameObject.Find("UI").GetComponent<UIController>();
		if (this.keyHandler == null)
		{
			Debug.Log("bruh");
		}
	}

	void Update()
	{
		// Interact with the object when pressing "E"
		if (playerIsClose)
		{
			this.keyHandler.AddInteractable(this);
		}
	}

	// Used to determine when the player is close or not, allowing/disallowing interacting
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerIsClose = true;
			this.ui.ShowInteractHint();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerIsClose = false;
			this.ui.HideInteractHint();
		}
	}

	public virtual void interact()
	{
		//
	}
}
