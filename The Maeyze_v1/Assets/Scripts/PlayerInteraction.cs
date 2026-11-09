
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float Range = 5f;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private TMP_Text InteractionText;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Update()
    {
        HandleRaycasting();
    }

    private void HandleRaycasting()
    {
        InteractionText.text = "";   // laging linisin muna

        RaycastHit hit;
        if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Range, LayerMask))
            return;
        if (!hit.collider.TryGetComponent(out IInteractable interactable))
            return;

        InteractionText.text = interactable.InteractionText;
        if (Input.GetKeyDown(KeyCode.E))
            interactable.Interact();
    }
}
