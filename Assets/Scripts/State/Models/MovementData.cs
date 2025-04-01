using System;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace State.Models
{
    public enum MovementState {
        OnFoot,
        InAir,
        Duck,
        Bounce,
        Climb,
        Putt
    }
    
    public class MovementData
    {
        private static readonly int IsOnGroundStr = Animator.StringToHash("IsGrounded");
        private static readonly int IsDuckedStr = Animator.StringToHash("IsDucked");
        private static readonly int IsClimbingStr = Animator.StringToHash("IsClimbing");
        private static readonly int IsBouncingStr = Animator.StringToHash("IsBouncing");
        private static readonly int BounceHitStr = Animator.StringToHash("BounceHit");
        private static readonly int CanPuttStr = Animator.StringToHash("Can Putt");
        private static readonly int PuttStr = Animator.StringToHash("Putt");
        private static readonly int PuttSuccessStr = Animator.StringToHash("Putt Success");
        private static readonly int PuttFailedStr = Animator.StringToHash("Putt Failed");
        private static readonly int DidPuttStr = Animator.StringToHash("Did Putt");

        
        private bool _isInAir;
        private bool _isOnGround;
        private bool _isDucked;
        private bool _isBouncing;
        private bool _canClimb;
        private bool _tryingToClimb;
        private float _puttableDirection;
        private float _sideInputDirection;
        private bool _didPutt;
        private Animator _animator;
        
        public BoolFlag IsBouncingLocked = new();
        
        private static MovementData _instance;
        public static MovementData Instance()
        {
            if (_instance == null)
            {
                _instance = new MovementData();
            }
            return _instance;
        }

        public class BoolFlag // boolean wrapper class
        {
            private bool _flag;
            public void SetFlag(bool flag)
            {
                _flag = flag;
            }
            public bool GetFlag()
            {
                return _flag;
            }
        }

        public bool WasInAir { get; set; }

        public bool IsDucked
        {
            get => _isDucked;
            set
            {
                if (_isDucked == value)
                    return;
                _isDucked = value;
                _animator.SetBool(IsDuckedStr, _isDucked);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }

        public bool IsGrounded
        {
            get => _isOnGround;
            set
            {
                if (_isOnGround == value)
                    return;
                _isOnGround = value;
                _animator.SetBool(IsOnGroundStr, _isOnGround);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }

        public bool IsBouncing
        {
            get => _isBouncing;
            set
            {
                if (_isBouncing == value)
                    return;
                _isBouncing = value;
                _animator.SetBool(IsBouncingStr, _isBouncing);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }

        public float SideInputDirection
        {
            get => _sideInputDirection;
            set
            {
                if (_sideInputDirection == value)
                    return;
                _sideInputDirection = value;
                _animator.SetBool(CanPuttStr, _sideInputDirection == _puttableDirection && _sideInputDirection != 0);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }

        public float PuttableDirection
        {
            get => _puttableDirection;
            set
            {
                if (_puttableDirection == value)
                    return;
                _puttableDirection = value;
                _animator.SetBool(CanPuttStr, _sideInputDirection == _puttableDirection && _puttableDirection != 0);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }
        
        public bool CanClimb
        {
            get => _canClimb;
            set
            {
                if (_canClimb == value)
                    return;
                _canClimb = value;
                _animator.SetBool(IsClimbingStr, _canClimb && _tryingToClimb);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }
        
        public bool TryingToClimb
        {
            get => _tryingToClimb;
            set
            {
                if (_tryingToClimb == value)
                    return;
                _tryingToClimb = value;
                _animator.SetBool(IsClimbingStr, _tryingToClimb && _canClimb);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }

        public bool DidPutt
        {
            get => _didPutt;
            set
            {
                if (_didPutt == value)
                    return;
                _didPutt = value;
                _animator.SetBool(DidPuttStr, _didPutt);
                GameEvents.Instance().OnMovementDataChanged?.Invoke();
            }
        }

        public bool IsInAir { get; set; }

        public void SetAnimator(Animator animator)
        {
            if (animator == null)
                throw new ArgumentNullException(nameof(animator), "Animator cannot be null");
            this._animator = animator;
        }

        public void SetPutt()
        {
            _animator.SetTrigger(PuttStr);
        }

        public void SetPuttSuccess()
        {
            _animator.SetTrigger(PuttSuccessStr);
            DidPutt = true;
        }

        public void SetPuttFail()
        {
            _animator.SetTrigger(PuttFailedStr);
            DidPutt = true;
        }

        public MovementState GetState()
        {
            if (TryingToClimb && CanClimb && !IsGrounded)
                return MovementState.Climb;

            if (IsInAir && !IsGrounded && !IsBouncing)
                return MovementState.InAir;
            
            if (IsBouncing)
                return MovementState.Bounce;
            
            if (IsDucked && IsGrounded)
                return MovementState.Duck;
            
            if (IsGrounded && SideInputDirection == PuttableDirection  && SideInputDirection != 0 && !DidPutt)
                return MovementState.Putt;

            return MovementState.OnFoot;
        }

        public void BounceHItGround() => _animator.SetTrigger(BounceHitStr);
    }
}