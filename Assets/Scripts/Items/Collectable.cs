using System;
using JetBrains.Annotations;
using Pool;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Items
{
    public class Collectable : MonoBehaviour, IPoolable
    {
        [SerializeField] private float spawnBounceForce;
        [SerializeField] private LayerMask landOn;
    
        private static readonly int Reset1 = Animator.StringToHash("Reset");
        private readonly float _initialGravityScale = 2;

        protected Rigidbody2D Rigidbody;
        protected Animator Animator;
        [CanBeNull] private CapsuleCollider2D _collider;

        protected virtual void OnEnable()
        {
            Rigidbody = gameObject.GetComponent<Rigidbody2D>();
            Animator = gameObject.GetComponent<Animator>();
            _collider = gameObject.GetComponent<CapsuleCollider2D>();
            Rigidbody.AddForce(Vector2.up * spawnBounceForce , ForceMode2D.Impulse);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Item Collector"))
            {
                gameObject.SetActive(false);
                Debug.Log("collected");
            }
        }

        private void FixedUpdate()
        {
            float rayLength = _collider?.size.y / 2 ?? .15f;
            RaycastHit2D onGround = Physics2D.Raycast(transform.position, Vector2.down, rayLength , 
                landOn);   
            Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);

            if (onGround && Rigidbody.linearVelocity.y < 0)
            {
                Rigidbody.gravityScale = 0;
                Rigidbody.linearVelocity = Vector2.zero;
            }
        }

        public virtual void Reset()
        {
            Rigidbody.gravityScale = _initialGravityScale;
        }
    }
}
