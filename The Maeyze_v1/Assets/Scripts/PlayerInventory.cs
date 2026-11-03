using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private ElectricTorchOnOff torch;
    [SerializeField] private KeyCode useBatteryKey = KeyCode.R;
    [SerializeField] private float rechargeAmount = 1.25f;   // kalahati ng 2.5

    public int Batteries { get; private set; }
    public bool HasKey { get; private set; }

    public void AddBattery() { Batteries++; }
    public void AddKey()     { HasKey = true; }

    private void Update()
    {
        if (Input.GetKeyDown(useBatteryKey) && Batteries > 0 && torch != null)
        {
            Batteries--;
            torch.Recharge(rechargeAmount);
        }
    }
}