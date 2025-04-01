using System;
using System.Collections;
using Audio;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public class Boss : Enemy
    {
        private static readonly int Death = Animator.StringToHash("Death");

        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpInterval = 3f;
        [SerializeField] private float earthquakeDuration = 1f;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private float wallCheckDistance = 0.5f;
        [SerializeField] private float deathJumpForce = 6f;
        [SerializeField] private AudioWrapper deathSound;

        
        private SpriteRenderer _spriteRenderer;
        private WaitForSeconds _flickeringEffect = new WaitForSeconds(0.04f);
        private static bool _playingAudio = false;

        private int _moveDirection = 1;
        private bool _isEarthquakeActive;
        private float _jumpTimer;
        private bool _isDead;
        private bool _didDeathJump;
        private bool _wasGrounded = true;

        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _jumpTimer = jumpInterval;
        }

        private void FixedUpdate()
        {
            if (_isDead)
            {
                if (!_didDeathJump && RigidBody.linearVelocity.y <= 0.1f)
                {
                    RigidBody.AddForce(Vector2.up * deathJumpForce, ForceMode2D.Impulse);
                    _didDeathJump = true;
                }
                return;
            }

            if (_isEarthquakeActive)
                return;

            Move();

            // Check if the boss has landed
            bool isGrounded = IsGrounded();
            if (!_wasGrounded && isGrounded)
            {
                StartCoroutine(TriggerEarthquake());
            }
            _wasGrounded = isGrounded;

            _jumpTimer -= Time.deltaTime;
            if (_jumpTimer <= 0f && isGrounded)
            {
                Jump();
                _jumpTimer = jumpInterval;
            }
        }

        protected override void GotHurt()
        {
            base.GotHurt();
            StartCoroutine(HurtAnimation());
        }
        
        private IEnumerator HurtAnimation()
        {
            for (int i = 0; i < 10; i++)
            {
                _spriteRenderer.enabled = false;
                yield return _flickeringEffect;
                _spriteRenderer.enabled = true;
                yield return _flickeringEffect;
            }
        }

        private void Move()
        {
            if (IsHittingWall())
            {
                _moveDirection *= -1;
            }

            Vector2 velocity = RigidBody.linearVelocity;
            velocity.x = moveSpeed * _moveDirection;
            RigidBody.linearVelocity = velocity;

            transform.localScale = new Vector3(_moveDirection, 1, 1);
        }

        private bool IsHittingWall()
        {
            Vector2 direction = Vector2.right * _moveDirection;
            RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
            return hit.collider != null;
        }

        private void Jump()
        {
            RigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private bool IsGrounded()
        {
            // Check if the boss is grounded
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
            return hit.collider != null;
        }

        private IEnumerator TriggerEarthquake()
        {
            _isEarthquakeActive = true;
            GameEvents.Instance().OnEarthquakeStarted?.Invoke();
            yield return new WaitForSeconds(earthquakeDuration);
            GameEvents.Instance().OnEarthquakeFinished?.Invoke();
            _isEarthquakeActive = false;
        }

        protected override void OnPlayerDied()
        {
            RigidBody.simulated = false;
        }

        protected override void Die()
        {
            Animator.SetTrigger(Death);
            gameObject.layer = LayerMask.NameToLayer("Dead Enemies");
            _isDead = true;
            GameEvents.Instance().OnFinishLevel?.Invoke();
        }
        

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 direction = Vector2.right * _moveDirection;
            Gizmos.DrawRay(wallCheck.position, direction * wallCheckDistance);
        }
    }
}
