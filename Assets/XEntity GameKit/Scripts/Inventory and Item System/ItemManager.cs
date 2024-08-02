using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace XEntity.InventoryItemSystem
{
    public class ItemManager : MonoBehaviour
    {
        public UnityEvent onBlueConsumed; // 
        public UnityEvent onAppleConsumed; 
        public InteractionSettings interactionSettings;
       

        //Singleton instance of this script.
        public static ItemManager Instance { get; private set; }

        //List of all the item scriptable obects.
        public List<Item> itemList = new List<Item>();

        private void Awake()
        {
            //Singleton logic
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            #endregion
        }

        public void UseItem(ItemSlot slot)
        {
            if (slot.IsEmpty) return;

            switch (slot.slotItem.type)
            {
                default: DefaultItemUse(slot); break;
                case ItemType.ToolOrWeapon: EquipItem(slot); break;
                case ItemType.Placeable: PlaceItem(slot); break;
                case ItemType.Consumeable: ConsumeItem(slot); break;
            }
        }

        private void ConsumeItem(ItemSlot slot)
        {
            // Check the name of the item
            switch (slot.slotItem.itemName)
            {
                case "Apple":
                    Debug.Log("You have consumed an apple. One heart gained! " + slot.slotItem.itemName);
                    // Invoke the onAppleConsumed event
                    if (onAppleConsumed != null)
                    {
                        onAppleConsumed.Invoke();
                    }
                    break;
                case "Blue":
                    Debug.Log("You have consumed Blue. Jump height increased! " + slot.slotItem.itemName);
                    // Invoke the onBlueConsumed event
                    if (onBlueConsumed != null)
                    {
                        onBlueConsumed.Invoke();
                    }
                    break;
                // Add cases for other consumable items here
                default:
                    Debug.Log("You have consumed a " + slot.slotItem.itemName);
                    break;
            }

            // Remove the item from the slot
            slot.Remove(1);
        }

        private void EquipItem(ItemSlot slot)
        {
            Debug.Log("Equipping " + slot.slotItem.itemName);
        }

        private void PlaceItem(ItemSlot slot)
        {
            Debug.Log("Placing " + slot.slotItem.itemName);
        }

        private void DefaultItemUse(ItemSlot slot)
        {
            Debug.Log($"Using {slot.slotItem.itemName}.");
        }

        public Item GetItemByIndex(int index)
        {
            return itemList[index];
        }

        public Item GetItemByName(string name)
        {
            foreach (Item item in itemList) if (item.itemName == name) return item;
            return null;
        }

        public int GetItemIndex(Item item)
        {
            for (int i = 0; i < itemList.Count; i++) if (itemList[i] == item) return i;
            return -1;
        }
    }
}
