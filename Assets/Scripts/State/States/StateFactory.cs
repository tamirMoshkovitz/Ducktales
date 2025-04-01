using State.Interfaces;
using State.Models;
using Unity.VisualScripting;
using UnityEngine.Windows;

namespace State.States
{
    public class StateFactory : MonoSingleton<StateFactory> // TODO: maby move to inner calss of movement data
    // (in ordet to add states i need to update too many scripts)

    {
        public IMovementState Create(MovementState state)
        {
            switch (state)
            {
                case MovementState.OnFoot:
                    return new OnFootState();
                case MovementState.InAir:
                    return new InAirState();
                case MovementState.Duck:
                    return new DuckState();
                case MovementState.Bounce:
                    return new BounceState();
                case MovementState.Climb:
                    return new ClimbState();
                case MovementState.Putt:
                    return new PuttState();
            }
            return null;
        }
    }
}