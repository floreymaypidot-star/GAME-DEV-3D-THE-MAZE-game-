using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType { Battery, Key }

public class Collectables : MonoBehaviour, IInteractable
{
    [SerializeField] private CollectableType type;
    [SerializeField] private string pickUpText = "[E] to pick up";

    public string InteractionText => pickUpText;

    public void Interact()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory == null)
        {
            // Debug.LogWarning("Walang PlayerInventory sa scene!");
            return;
        }

        if (type == CollectableType.Battery) inventory.AddBattery();
        else inventory.AddKey();

        Destroy(gameObject);
    }
}