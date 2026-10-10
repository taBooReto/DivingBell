using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRayCast : MonoBehaviour
{
    public InputAction interactAction;
    public Transform cam;
    public float rayLength;

    private void OnEnable()
    {
        interactAction.Enable();
        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        interactAction.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, rayLength))
        {
            if (hit.collider.TryGetComponent<Interactive>(out Interactive interactive))
            {
                interactive.Interact(gameObject);
            }
        }
        Debug.Log("Interact pressed");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (cam != null)
            Gizmos.DrawLine(cam.position, cam.position + cam.forward * rayLength);
    }
}