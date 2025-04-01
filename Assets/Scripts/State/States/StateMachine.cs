using System;
using State.Interfaces;
using State.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace State.States
{
    public class StateMachine
    {
        public Action OnBounceStateEnter;
        public Action OnBounceStateExit;
        
        private IMovementState _currentState;
        private static StateMachine _instance;
        private MovementState _currentStateEnum;
        
        public static StateMachine Instance()
        {
            if (_instance == null)
            {
                _instance = new StateMachine();
            }
            return _instance;
        }

        private StateMachine()
        {
            _currentState = new OnFootState();
        }

        public void OnMovementDataChanged(IContext context)
        {
            var newStateEnum = MovementData.Instance().GetState();
            IMovementState newState = StateFactory.Instance.Create(newStateEnum);
            if (newState == null)
                Debug.LogWarning("new state is null instead of " + newStateEnum);
            if (newState.GetType() != _currentState.GetType())
            {
                
                _currentState?.ExitState(context);
                if (_currentStateEnum == MovementState.Bounce)
                    OnBounceStateExit?.Invoke();
                if (_currentStateEnum == MovementState.InAir)
                    MovementData.Instance().WasInAir = true;
                else
                    MovementData.Instance().WasInAir = false;
                _currentState = newState;
                
                _currentState?.EnterState(context);
                if (newStateEnum == MovementState.Bounce)
                    OnBounceStateEnter?.Invoke();
                
                _currentStateEnum = newStateEnum;
                Debug.Log(_currentState?.GetType().Name);
            }
        }

        public void Update(Vector2 direction, IContext context)
        {
            _currentState.Update(direction, context);
        }

        public void FixedUpdate(IContext context)
        {
            _currentState.FixedUpdate(context);
        }
    }
}