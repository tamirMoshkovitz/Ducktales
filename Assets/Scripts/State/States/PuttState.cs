using System.Collections;
using State.Interfaces;
using State.Models;
using UnityEngine;

namespace State.States
{
    public class PuttState : IMovementState
    {
        public void EnterState(IContext context) { }

        public void ExitState(IContext context) { }

        public void Update(Vector2 input, IContext context) { } // Move near puttable logic here

        public void FixedUpdate(IContext context) { }
        
    }
}