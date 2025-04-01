using Pool;
using UnityEngine;

namespace Items
{
    public class RockContainer : ItemContainer, IPoolable
    {
        private static readonly int ResetStr = Animator.StringToHash("Reset");

        protected Rigidbody2D _rigidbody;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected override void Return()
        {
            RockContainerMonoPool.Instance?.Return(this);
        }

        public void Reset()
        {
            _animator.ResetTrigger(ResetStr);
        }

        private void OnBecameInvisible()
        {
            Return();
        }
    }
}