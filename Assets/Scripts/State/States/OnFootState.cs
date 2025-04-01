using State.Interfaces;
using State.Models;
using UnityEngine;
using Utilities;

namespace State.States
{
    public class OnFootState : IMovementState
    {
        private Vector2 _direction;

        public void EnterState(IContext context)
        {
            MovementData.Instance().DidPutt = false;
            if (MovementData.Instance().WasInAir)
                GameEvents.Instance().OnLandOnGround?.Invoke();
        }

        public void ExitState(IContext context) { }

        public void Update(Vector2 input, IContext context)
        {
            _direction = new Vector2(input.x, 0f).normalized;
        }

        public void FixedUpdate(IContext context)
        {
            float speed = context.GetSpeed();
            float maxSpeed = context.GetMaxSpeed();
            context.SetLinearVelocity(new Vector2(_direction.x * speed, context.GetLinearVelocity().y));
            
            if (context.GetLinearVelocity().magnitude > maxSpeed)
            {
                context.SetLinearVelocity(context.GetLinearVelocity().normalized * maxSpeed);
            }
        }
    }
}