namespace State.Models
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [Serializable]
    public class TwoButtonInput
    {
        public InputAction primaryButton;
        public InputAction secondaryButton;

        private MovementData.BoolFlag _stateLockedIn;
        private bool _stateActive;
        
        public Action OnStateEntered;
        public Action OnStateExited;
        
        public void Enable(MovementData.BoolFlag stateLockedIn)
        {
            _stateLockedIn = stateLockedIn;
            primaryButton.Enable();
            secondaryButton.Enable();
        }

        public void Disable()
        {
            primaryButton.Disable();
            secondaryButton.Disable();
        }

        public void Update()
        {
            if (!_stateActive && MovementData.Instance().IsGrounded)
                return;
            
            if (!_stateLockedIn.GetFlag())
            {
                UpdateNotLockedIn();
            }
            else // state locked in
            {
                if (!primaryButton.IsPressed() && _stateActive)
                {
                    ExitState();
                }
            }
        }

        private void UpdateNotLockedIn()
        {
            if (primaryButton.IsPressed() && secondaryButton.IsPressed())
            {
                if (!_stateActive)
                {
                    _stateActive = true;
                    OnStateEntered?.Invoke();
                }

                if (!_stateLockedIn.GetFlag() && MovementData.Instance().IsGrounded)
                {
                    _stateLockedIn.SetFlag(true);
                }
            }
            else if (_stateActive)
            {
                ExitState();
            }
        }

        private void ExitState()
        {
            _stateLockedIn.SetFlag(false);
            _stateActive = false;
            OnStateExited?.Invoke();
        }
    }

}
