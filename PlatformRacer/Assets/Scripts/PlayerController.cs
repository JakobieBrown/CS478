using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Rigidbody2D rb;
    private Vector2 horizontalMovementInput;
    private bool jumpHeld;
    public InputActionReference moveReference;

    [SerializeField] private float speed;
    [SerializeField] private float JumpPower;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovementInput = moveReference.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocityX= horizontalMovementInput.x * speed;
        }
        if (jumpHeld)
        {
            rb.gravityScale = 0;
            rb.AddForceY(JumpPower);
        }
        else { rb.gravityScale = 2; }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            horizontalMovementInput = new Vector2(dir.x, 0);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            jumpHeld = context.performed;
        }
    }
}
