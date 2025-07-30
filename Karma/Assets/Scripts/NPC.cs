using UnityEngine;

public class NPC : Interactable
{
    [Header("Item Data")]
    [SerializeField] private string itemName;
    [SerializeField] private interactType type; // Used to define what type of interactable the item is

    [Header("Dialogue Data")]


    [Header("Pickup Data")]


    [Header("Putdown Data")]
    [SerializeField] private string heldItem;

    private Player playerScript;
    private bool playerIsClose;
    private GameObject player;

		// Eventually I would like to move each type of interactable to it's own script
		// to allow for more specific mechancis, but I think this works fine for now.
    enum interactType
    {
        DIALOGUE, // NPCs with dialogues
        PICKUP, // Items that can be picked up
        PUTDOWN, // Used mostly for altars
    }

    void Start()
    {
        // Gets a reference to the player character - this is used for getting/setting variables
				// from them (this should be reworked once someone who actually knows how to code gets their hands on the project)
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        // Interact with the object when pressing "E"
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            switch (type)
            {
                case interactType.DIALOGUE:
                    Debug.Log(itemName + " is speaking.");
                    break;
                case interactType.PICKUP:
                    Debug.Log("Picked up " + itemName + ".");
                    playerScript.heldItem = itemName;
                    break;
                case interactType.PUTDOWN:
                    if (playerScript.heldItem == null)
                    {
                        Debug.Log("You are not holding an item.");
                    }
                    else
                    {
                        heldItem = playerScript.heldItem;
                        playerScript.heldItem = null;
                        Debug.Log("Put down " + heldItem + " inside of " + itemName + ".");
                    }
                    break;
                default:
                    Debug.Log("Interact type not defined.");
                    break;

            }
        }
    }

    // Used to determine when the player is close or not, allowing/disallowing interacting
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}
