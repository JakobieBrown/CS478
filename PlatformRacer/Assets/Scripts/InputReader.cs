using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using static PlayerInputActions;
using System;

[CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    PlayerInputActions inputActions;
    public Vector2 Move => inputActions.Player.Move.ReadValue<Vector2>();
    public bool jumpTriggered;
    public bool jumpHeld;
    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);

        }
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    public void Enable()
    {
        inputActions.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        return;

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        return;


    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        return;


    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        return;

    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        return;


    }
    public event Action OnJumpPerformed;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpTriggered = true;
            jumpHeld = true;
            OnJumpPerformed?.Invoke();
        }
        else
        {
            jumpTriggered = false;
        }

        if (context.canceled)
        {
            jumpHeld = false;
        }
    }


    public void OnPrevious(InputAction.CallbackContext context)
    {
        return;


    }

    public void OnNext(InputAction.CallbackContext context)
    {
        return;


    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        return;


    }
}
