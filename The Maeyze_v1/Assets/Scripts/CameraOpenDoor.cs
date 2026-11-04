using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CameraDoorScript
{
    public class CameraOpenDoor : MonoBehaviour
    {
        public float DistanceOpen = 3;
        public GameObject text;
        public PlayerInventory inventory;
        public string openMessage = "[E] to open";
        public string lockedMessage = "Locked - find the key";

        private TMP_Text label;

        void Start()
        {
            if (text != null)
                label = text.GetComponent<TMP_Text>();
        }

        void Update()
        {
            bool showText = false;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DistanceOpen))
            {
                DoorScript.Door door = hit.collider.GetComponentInParent<DoorScript.Door>();
                if (door != null)
                {
                    showText = true;
                    bool canOpen = !door.requiresKey || (inventory != null && inventory.HasKey);

                    if (label != null)
                        label.text = canOpen ? openMessage : lockedMessage;

                    if (Input.GetKeyDown(KeyCode.E) && canOpen)
                        door.OpenDoor();
                }
            }

            if (text != null)
                text.SetActive(showText);
        }
    }
}