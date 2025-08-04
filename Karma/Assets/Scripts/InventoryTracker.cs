using System.Collections.Generic;


public class InventoryTracker
{
	private class ItemOption {
		public bool hasItem = false;
		public Item item;

		public ItemOption (Item item)
		{
			this.hasItem = true;
			this.item = item;
		}

		public ItemOption ()
		{
			this.hasItem = false;
		}
	}

	public static int hotbarMax = 5;
	public static int totalAuxMox = 25;
	public static int totalMax = hotbarMax + totalAuxMox;

	List<ItemOption> aux;
	List<ItemOption> hotbar;

	private int nextFreeAuxSlot = 0;
	private int nextFreeHotbarSlot = 0;
	private int totalStoredItems = 0;

	bool AddItem(Item item)
	{
		if (this.totalStoredItems >= InventoryTracker.totalMax)
		{
			return false;
		}

		ItemOption tmp = new ItemOption (item);
		if (this.hotbar.Count < InventoryTracker.hotbarMax)
		{
			this.hotbar[nextFreeHotbarSlot] = tmp;
			return true;
		}

		aux[this.nextFreeAuxSlot] = new ItemOption (item);
		return true;
	}

	bool RemoveItem(Item item)
	{
		ItemOption tmp = new ItemOption(item);
		if (!hotbar.Remove(tmp))
		{
			return aux.Remove(tmp);
		}
		return true;
	}

	bool HasItem(Item item)
	{
		ItemOption tmp = new ItemOption(item);
		return this.aux.Contains(tmp) || this.hotbar.Contains(tmp);
	}
}
