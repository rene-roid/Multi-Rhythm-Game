using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private void Update() => FrameInput = Gather();

    private PlayerInputActions inputActions;
    private InputAction move, mouseLeft, mouseRight;

    private void Awake() {
        inputActions = new PlayerInputActions();

        move = inputActions.Player.Move;

        mouseLeft = inputActions.Player.MouseLeft;
        mouseRight = inputActions.Player.MouseRight;
    }

    private void OnEnable() {
        inputActions ??= new PlayerInputActions();
        inputActions.Enable();
    }

    private void OnDisable() => inputActions.Disable();

    private FrameInput Gather() {
        if (Time.timeScale == 0) return new FrameInput();

        return new FrameInput {
            Move = move.ReadValue<Vector2>(),

            MouseLeft = mouseLeft.WasPressedThisFrame(),
            MouseLeftHeld = mouseLeft.IsPressed(),
            MouseLeftReleased = mouseLeft.WasReleasedThisFrame(),

            MouseRight = mouseRight.triggered,
            MouseRightHeld = mouseRight.IsPressed(),
            MouseRightReleased = mouseRight.WasReleasedThisFrame()
        };
    }
}

public struct FrameInput {
    public Vector2 Move;

    public bool MouseLeft;
    public bool MouseLeftHeld;
    public bool MouseLeftReleased;

    public bool MouseRight;
    public bool MouseRightHeld;
    public bool MouseRightReleased;
}