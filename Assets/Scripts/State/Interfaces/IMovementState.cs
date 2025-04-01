using UnityEngine;

namespace State.Interfaces
{
    public interface IMovementState
    {
        void EnterState(IContext context);
        void ExitState(IContext context);
        void Update(Vector2 input, IContext context);
        void FixedUpdate(IContext context);
    }
}