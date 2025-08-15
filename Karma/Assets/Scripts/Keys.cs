using UnityEngine;

public class Keys : MonoBehaviour
{
	[Header("Movement Stats")]
	[SerializeField] private float speed = 15f;

	[Header("Camera Settings")]
	[SerializeField] private Camera playerCamera;
	[SerializeField] private float screenEdgeBuffer = 10f; // Distance from screen edge before camera moves
	[SerializeField] private float cameraSpeed = 10f; // How fast the camera follows
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
				playerCamera = FindObjectOfType<Camera>();
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

		// Get player position in screen coordinates
		Vector3 playerScreenPos = playerCamera.WorldToViewportPoint(player.position);

		// Calculate camera offset needed
		Vector3 cameraOffset = Vector3.zero;

		// Check horizontal bounds
		if (playerScreenPos.x < screenEdgeBuffer / 100f) // Left edge
		{
			cameraOffset.x = playerScreenPos.x - (screenEdgeBuffer / 100f);
		}
		else if (playerScreenPos.x > 1f - (screenEdgeBuffer / 100f)) // Right edge
		{
			cameraOffset.x = playerScreenPos.x - (1f - (screenEdgeBuffer / 100f));
		}

		// Check vertical bounds
		if (playerScreenPos.y < screenEdgeBuffer / 100f) // Bottom edge
		{
			cameraOffset.y = playerScreenPos.y - (screenEdgeBuffer / 100f);
		}
		else if (playerScreenPos.y > 1f - (screenEdgeBuffer / 100f)) // Top edge
		{
			cameraOffset.y = playerScreenPos.y - (1f - (screenEdgeBuffer / 100f));
		}

		// Convert offset back to world coordinates
		if (cameraOffset != Vector3.zero)
		{
			Vector3 worldOffset = playerCamera.ViewportToWorldPoint(new Vector3(cameraOffset.x, cameraOffset.y, playerCamera.nearClipPlane)) -
								 playerCamera.ViewportToWorldPoint(Vector3.zero);

			targetCameraPosition += new Vector3(worldOffset.x, worldOffset.y, 0);
		}

		// Apply camera movement
		if (smoothCameraMovement)
		{
			// Smooth camera following
			playerCamera.transform.position = Vector3.Lerp(
				playerCamera.transform.position,
				targetCameraPosition,
				cameraSpeed * Time.deltaTime
			);
		}
		else
		{
			// Instant camera movement
			playerCamera.transform.position = targetCameraPosition;
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
