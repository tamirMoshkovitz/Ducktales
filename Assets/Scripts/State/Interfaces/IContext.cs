using UnityEngine;

namespace State.Interfaces
{
    public interface IContext
    {
        InputSystem_Actions InputActions { get; }
        float AnimationSpeed { get; }

        void SetLinearVelocity(Vector2 velocity);
        Vector2 GetLinearVelocity();
        void SetGravityScale(float gravityScale);
        float GetGravityScale();
        float GetJumpForce();
        float GetBounceForce();
        float GetSpeed();
        float GetMaxSpeed();
        void ClingToVine();
        void SetAnimationSpeed(float animationSpeed);
    }
}