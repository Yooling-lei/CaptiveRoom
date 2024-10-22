using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class PlayerInputReceiver : MonoBehaviour
{
    [Header("Character Input Values")] public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool fire;
    public Action ToggleBagAction;

    public Action InteractAction;

    // 右键放大视角(瞄准视角)
    public bool viewFocus;

    [Header("Movement Settings")] public bool analogMovement;
    [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
    public bool cursorInputForLook = true;


#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    // 正在按下右键
    public void OnViewFocus(InputValue value)
    {
        ViewFocusInput(value.isPressed);
    }

    // 按下鼠标左键
    public void OnFire(InputValue value)
    {
        FireInput(value.isPressed);
    }


    // 按下交互键
    public void OnInteract(InputValue value)
    {
        InteractAction();
    }

    // 切换背包显示(默认Tab键)
    public void OnToggleBag(InputValue value)
    {
        ToggleBagAction();
    }


#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void ViewFocusInput(bool newViewFocusState)
    {
        viewFocus = newViewFocusState;
    }

    public void FireInput(bool newFireState)
    {
        fire = newFireState;
    }
}