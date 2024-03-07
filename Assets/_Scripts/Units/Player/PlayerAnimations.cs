using System;
using UnityEngine;

namespace _Scripts.Units.Player
{
    public enum AnimatonState { Idle, Airbone, Crouch, Dodge }
    public class PlayerAnimations : MonoBehaviour
    {
        public AnimatonState animationState = AnimatonState.Idle;

        private void Update()
        {
            switch (animationState)
            {
                case AnimatonState.Idle:
                    break;
                case AnimatonState.Airbone:
                    break;
                case AnimatonState.Crouch:
                    break;
                case AnimatonState.Dodge:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void SetAnimationState(AnimatonState state){
            // exit
            switch (animationState)
            {
                case AnimatonState.Idle:
                    break;
                case AnimatonState.Airbone:
                    break;
                case AnimatonState.Crouch:
                    break;
                case AnimatonState.Dodge:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            animationState = state;
            
            // enter
            switch (state)
            {
                case AnimatonState.Idle:
                    break;
                case AnimatonState.Airbone:
                    break;
                case AnimatonState.Crouch:
                    break;
                case AnimatonState.Dodge:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
