using UnityEngine;

public class Keys : MonoBehaviour
{
	[Header("Movement Stats")]
	[SerializeField] private float speed = 5f;

	private Rigidbody2D player;
	private Vector2 movement;

	//This way objects can detect their own player collisions and set themselves
	//as interactable, but there will only every be one object that we interact
	//with at a time
	private Interactable interactObject;

	private bool playerHasKeyControl = true;

	void Start()
	{
		player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (!playerHasKeyControl)
		{
			return;
		}
// 		bool inputPaused =
// 		TODO

		// ============
		// Player Movement
		// ============
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");
		movement.Normalize();

		// ============
		// Interact
		// ============
		bool interact = Input.GetKeyDown(KeyCode.E);
		if (interact)
		{
			InteractNearby();
		}

		// ============
		// Inventory
		// ============
		bool inventory = Input.GetKeyDown(KeyCode.I);
		if (inventory)
		{
			OpenInventory();
		}
	}

	public void TakeKeyControl()
	{
		this.playerHasKeyControl = false;
	}

	public void ReleaseKeyControl()
	{
		this.playerHasKeyControl = true;
	}

	private void OpenInventory()
	{
		return;
	}

	public bool AddInteractable(Interactable toAdd)
	{
		if (this.interactObject == null)
		{
			this.interactObject = toAdd;
			return true;
		}
		return false;
	}

	public bool RemoveInteractable(Interactable toRemove)
	{
		if (this.interactObject == toRemove)
		{
			this.interactObject = null;
			return true;
		}
		return false;
	}

	private void InteractNearby()
	{
		if (this.interactObject == null)
		{
			return;
		}
		else
		{
			this.interactObject.interact();
		}
	}

	private void FixedUpdate()
	{
		player.MovePosition(player.position + movement * speed * Time.fixedDeltaTime);
	}
}
