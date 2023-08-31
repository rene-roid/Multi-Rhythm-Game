using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private void Update() => FrameInput = Gather();

    private PlayerInputActions _inputActions;
    private InputAction _move, _mouseLeft, _mouseRight;

    private void Awake() {
        _inputActions = new PlayerInputActions();

        _move = _inputActions.Player.Move;

        _mouseLeft = _inputActions.Player.MouseLeft;
        _mouseRight = _inputActions.Player.MouseRight;
    }

    private void OnEnable() {
        _inputActions ??= new PlayerInputActions();
        _inputActions.Enable();
    }

    private void OnDisable() => _inputActions.Disable();

    private FrameInput Gather() {
        if (Time.timeScale == 0) return new FrameInput();

        return new FrameInput {
            Move = _move.ReadValue<Vector2>(),

            MouseLeft = _mouseLeft.WasPressedThisFrame(),
            MouseLeftHeld = _mouseLeft.IsPressed(),
            MouseLeftReleased = _mouseLeft.WasReleasedThisFrame(),

            MouseRight = _mouseRight.triggered,
            MouseRightHeld = _mouseRight.IsPressed(),
            MouseRightReleased = _mouseRight.WasReleasedThisFrame()
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