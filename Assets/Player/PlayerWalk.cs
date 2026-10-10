using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    [Header("Walk Settings")]
    public InputAction moveAction;
    public Rigidbody rb;
    public float speed;
    public bool canWalk;


    private void OnEnable()
    {
        moveAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void FixedUpdate()
    {

        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input != Vector2.zero)
        {
            Vector3 move = transform.forward * input.y + transform.right * input.x;
            move *= speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

        }
    }
}
