using Interactable;
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
	private Interactable interactable;

	void Start()
	{
		player = GameObject.Find("Player");
	}

	void Update()
	{
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

	public bool AddInteractable(Interactable toAdd)
	{
		if (this.interactable == null)
		{
			this.interactable = toAdd;
			return true;
		}
		return false;
	}

	public bool RemoveInteractable(Interactable toRemove)
	{
		if (this.interactable == toRemove)
		{
			this.interactable = null;
			return true;
		}
		return false;
	}

	private void InteractNearby()
	{
		if (this.interactable == null)
		{
			return;
		}
		else
		{
			this.interactable.interact();
		}
	}

	private void FixedUpdate()
	{
		player.MovePosition(player.position + movement * speed * Time.fixedDeltaTime);
	}
}
