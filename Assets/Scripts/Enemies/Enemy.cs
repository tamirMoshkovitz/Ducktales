using System;
using System.Collections;
using Audio;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected Transform playerTransform;
        [SerializeField] protected int health = 1;
        [SerializeField] protected LayerMask groundLayer;

        protected Rigidbody2D RigidBody;
        protected Animator Animator;

        protected virtual void OnEnable()
        {
            RigidBody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            GameEvents.Instance().OnPlayerDeath += OnPlayerDied;
        }

        protected virtual void OnDisable()
        {
            GameEvents.Instance().OnPlayerDeath -= OnPlayerDied;
        }

        protected virtual void Update()
        {
            if (health <= 0)
            {
                Die();
            }
        }

        protected virtual void GotHurt()
        {
            health -= 1;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Pogo"))
            {
                this.GotHurt();
            }
        }
        
        protected abstract void OnPlayerDied();

        protected abstract void Die();
    }
}