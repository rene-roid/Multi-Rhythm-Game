using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Managers.Input
{
    public class PlayerInput : MonoBehaviour
    {
        public FrameInput frameInput { get; private set; }

        private void Update() => frameInput = Gather();

        private PlayerInputActions inputActions;
        private InputAction runnerUp, runnerDown, dodgeLeft, dodgeRight, dodgeJump;

        private void Awake() {
            inputActions = new PlayerInputActions();

            runnerUp = inputActions.Runner.Up;
            runnerDown = inputActions.Runner.Down;
        
            dodgeLeft = inputActions.Dodge.Left;
            dodgeRight = inputActions.Dodge.Right;
            dodgeJump = inputActions.Dodge.Jump;
        }

        private void OnEnable() {
            inputActions ??= new PlayerInputActions();
            inputActions.Enable();
        }

        private void OnDisable() => inputActions.Disable();

        private FrameInput Gather() {
            if (Time.timeScale == 0) return new FrameInput();

            return new FrameInput {
                RunnerUp = runnerUp.WasPressedThisFrame(),
                RunnerUpHeld = runnerUp.IsPressed(),
            
                RunnerDown = runnerDown.WasPressedThisFrame(),
                RunnerDownHeld = runnerDown.IsPressed(),
            
                DodgeLeft = dodgeLeft.WasPressedThisFrame(),
                DodgeLeftHeld = dodgeLeft.IsPressed(),
            
                DodgeRight = dodgeRight.WasPressedThisFrame(),
                DodgeRightHeld = dodgeRight.IsPressed(),
            
                DodgeJump = dodgeJump.WasPressedThisFrame(),
                DodgeJumpHeld = dodgeJump.IsPressed()
            };
        }
    }

    public struct FrameInput {
        public bool RunnerUp;
        public bool RunnerUpHeld;
    
        public bool RunnerDown;
        public bool RunnerDownHeld;
    
        public bool DodgeLeft;
        public bool DodgeLeftHeld;
    
        public bool DodgeRight;
        public bool DodgeRightHeld;
    
        public bool DodgeJump;
        public bool DodgeJumpHeld;
    }
}