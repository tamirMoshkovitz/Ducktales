using State.Interfaces;
using State.Models;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace State.States
{
    public class BounceState : IMovementState
    {
        private bool StateLocked { get; set; }
        private Vector2 _direction;
        private float _coyoteTimeCounter;
        private const float CoyoteTime = 0.05f;
        private bool _isEarthquakeActive;

        public void EnterState(IContext context)
        {
            GameEvents.Instance().OnEarthquakeStarted += SetEarthquakeActive;
            GameEvents.Instance().OnEarthquakeFinished += SetEarthquakeInactive;
        }

        public void ExitState(IContext context)
        {
            GameEvents.Instance().OnEarthquakeStarted += SetEarthquakeActive;
            GameEvents.Instance().OnEarthquakeFinished += SetEarthquakeInactive;
        }

        public void Update(Vector2 input, IContext context)
        {
            _direction = new Vector2(input.x, 0f).normalized;
        }

        public void FixedUpdate(IContext context)
        {
            if (MovementData.Instance().IsGrounded)
            {
                if (_isEarthquakeActive)
                {
                    MovementData.Instance().IsBouncing = false;
                    context.SetLinearVelocity(new Vector2(context.GetLinearVelocity().x, context.GetJumpForce()));
                }
                else
                {
                    StateLocked = true;
                    MovementData.Instance().BounceHItGround();
                    context.SetLinearVelocity(new Vector2(context.GetLinearVelocity().x, context.GetBounceForce()));
                    GameEvents.Instance().OnBounceHit?.Invoke();
                }
            }

            float speed = context.GetSpeed(); //TODO: handle code duplication from InAir
            float maxSpeed = context.GetMaxSpeed();
            Vector2 contextVelocity = context.GetLinearVelocity();
            
            context.SetLinearVelocity(new Vector2(_direction.x * speed, contextVelocity.y));
            
            if (contextVelocity.magnitude > maxSpeed)
            {
                context.SetLinearVelocity(contextVelocity.normalized * maxSpeed);
                //TODO: limit only horizontal velocity and not vertical.
            }
        }

        private void SetEarthquakeActive()
        {
            _isEarthquakeActive = true;
        }

        private void SetEarthquakeInactive()
        {
            _isEarthquakeActive = false;
        }
    }
}