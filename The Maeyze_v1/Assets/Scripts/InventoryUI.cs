using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Image keyIcon;
    [SerializeField] private Image batteryIcon;
    [SerializeField] private TMP_Text batteryCount;

    private static readonly Color dim = new Color(1f, 1f, 1f, 0.25f);

    private void Update()
    {
        keyIcon.color = inventory.HasKey ? Color.white : dim;
        batteryIcon.color = inventory.Batteries > 0 ? Color.white : dim;
        batteryCount.text = "x" + inventory.Batteries;
    }
}