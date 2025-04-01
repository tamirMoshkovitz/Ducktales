using System.Numerics;
using State.Interfaces;
using State.Models;
using Utilities;
using Vector2 = UnityEngine.Vector2;


namespace State.States
{
    public class DuckState : IMovementState
    {
        public void EnterState(IContext context)
        {
            context.SetLinearVelocity(Vector2.zero);
            if (MovementData.Instance().WasInAir)
                GameEvents.Instance().OnLandOnGround?.Invoke();
        }

        public void ExitState(IContext context) { }

        public void Update(Vector2 input, IContext context) { }
        
        public void FixedUpdate(IContext context) { }
    }
}