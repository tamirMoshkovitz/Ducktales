using State.Interfaces;
using State.Models;
using UnityEngine;
using Utilities;

namespace State.States
{
    public class ClimbState : IMovementState
    {
        private Vector2 _direction;
        private float _originalGravity;

        public static bool IsDOTweenAnimating { get; set; }

        public void EnterState(IContext context)
        {
            context.SetLinearVelocity(Vector2.zero);
            _originalGravity = context.GetGravityScale();
            context.SetGravityScale(0f);
            context.ClingToVine();
        }

        public void ExitState(IContext context)
        {
            context.SetGravityScale(_originalGravity);
            MovementData.Instance().TryingToClimb = false;
            context.SetAnimationSpeed(context.AnimationSpeed);
            GameEvents.Instance().OnStopedClimbing?.Invoke();
        }

        public void Update(Vector2 input, IContext context)
        {
            _direction = new Vector2(0f, input.y).normalized;
            float animationSpeed;
            if (_direction == Vector2.zero || IsDOTweenAnimating)
            {
                animationSpeed = 0f;
                GameEvents.Instance().OnStopedClimbing?.Invoke();
            }
            else
            {
                animationSpeed = context.AnimationSpeed;
                GameEvents.Instance().OnClimbing?.Invoke();
            }
            context.SetAnimationSpeed(animationSpeed);
        }

        public void FixedUpdate(IContext context)
        {
            float speed = context.GetSpeed();
            float maxSpeed = context.GetMaxSpeed();
            context.SetLinearVelocity(new Vector2(0f, _direction.y *  speed));
            
            if (context.GetLinearVelocity().magnitude > maxSpeed)
            {
                context.SetLinearVelocity(context.GetLinearVelocity().normalized * maxSpeed);
            }
        }
    }
}