using UnityEngine;

public class Keys : MonoBehaviour
{
	[Header("Movement Stats")]
	[SerializeField] private float speed = 50f;

	[Header("Camera Settings")]
	[SerializeField] private Camera playerCamera;
	[SerializeField] private float screenEdgeBuffer = 5000f; // Distance from screen edge before camera moves
	[SerializeField] private float cameraSpeed = 8f; // How fast the camera follows
	[SerializeField] private bool smoothCameraMovement = true;

	private Rigidbody2D player;
	private Vector2 movement;
	private Vector3 targetCameraPosition;

	//This way objects can detect their own player collisions and set themselves
	//as interactable, but there will only every be one object that we interact
	//with at a time
	private Interactable interactObject;
	private bool playerHasKeyControl = true;

	void Start()
	{
		player = GameObject.Find("Player").GetComponent<Rigidbody2D>();

		// Get camera reference if not assigned
		if (playerCamera == null)
		{
			playerCamera = Camera.main;
			if (playerCamera == null)
			{
				playerCamera = FindFirstObjectByType<Camera>();
			}
		}

		// Initialize target camera position
		if (playerCamera != null)
		{
			targetCameraPosition = playerCamera.transform.position;
		}
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
		// Camera Movement
		// ============
		UpdateCameraPosition();

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

	private void UpdateCameraPosition()
	{
		if (playerCamera == null || player == null) return;

		// Get player position in viewport space (0–1)
		Vector3 playerViewportPos = playerCamera.WorldToViewportPoint(player.position);

		// Start with current camera position
		Vector3 newCameraPos = targetCameraPosition;

		// Horizontal check
		if (playerViewportPos.x < screenEdgeBuffer / 100f) // Left
		{
			float delta = (screenEdgeBuffer / 100f) - playerViewportPos.x;
			newCameraPos += playerCamera.transform.right * (-delta * cameraMoveDistance());
		}
		else if (playerViewportPos.x > 1f - (screenEdgeBuffer / 100f)) // Right
		{
			float delta = playerViewportPos.x - (1f - (screenEdgeBuffer / 100f));
			newCameraPos += playerCamera.transform.right * (delta * cameraMoveDistance());
		}

		// Vertical check
		if (playerViewportPos.y < screenEdgeBuffer / 100f) // Bottom
		{
			float delta = (screenEdgeBuffer / 100f) - playerViewportPos.y;
			newCameraPos += playerCamera.transform.up * (-delta * cameraMoveDistance());
		}
		else if (playerViewportPos.y > 1f - (screenEdgeBuffer / 100f)) // Top
		{
			float delta = playerViewportPos.y - (1f - (screenEdgeBuffer / 100f));
			newCameraPos += playerCamera.transform.up * (delta * cameraMoveDistance());
		}

		targetCameraPosition = new Vector3(newCameraPos.x, newCameraPos.y, targetCameraPosition.z);

		// Smooth or instant move
		if (smoothCameraMovement)
		{
			playerCamera.transform.position = Vector3.Lerp(
					playerCamera.transform.position,
					targetCameraPosition,
					cameraSpeed * Time.deltaTime
					);
		}
		else
		{
			playerCamera.transform.position = new Vector2(newCameraPos.x, newCameraPos.y);
		}
	}

	// This determines how far the camera should move in world space for a full viewport shift
	private float cameraMoveDistance()
	{
		// Use the vertical size at that distance as scale
		float worldHeight = 2f * 25 * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		return worldHeight;
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
