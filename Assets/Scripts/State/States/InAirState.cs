using System;
using State.Interfaces;
using State.Models;
using Unity.Cinemachine;
using UnityEngine;

namespace State.States
{
    public class InAirState : IMovementState
    {
        private Vector2 _direction;
        private float _coyoteTimeCounter;
        private const float CoyoteTime = 0.05f;
        
        public void EnterState(IContext context)
        {
            if (_coyoteTimeCounter > 0f)
            {
                float jumpForce = context.GetJumpForce();
                context.SetLinearVelocity(new Vector2(context.GetLinearVelocity().x, jumpForce));
                _coyoteTimeCounter = 0f;
            }
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
            
            
            Vector2 contextVelocity = context.GetLinearVelocity();
            
            context.SetLinearVelocity(new Vector2(_direction.x * speed, contextVelocity.y));
            
            if (Math.Abs(contextVelocity.magnitude) > maxSpeed)
            {
                context.SetLinearVelocity(contextVelocity.normalized * maxSpeed);
            }
            
            if (!MovementData.Instance().IsInAir)
            {
                _coyoteTimeCounter = CoyoteTime;
            }
            else
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }
        }
    }
}