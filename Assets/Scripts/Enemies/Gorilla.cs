using System;
using Pool;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public class Gorilla : Enemy, Pool.IPoolable
    {
        private static readonly int DeathStr = Animator.StringToHash("Death");
        private static readonly int ResetStr = Animator.StringToHash("Reset");

        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float wallDetectionDistance = 1f;
        [SerializeField] private float wallDetectionHeight = 1f;
        [SerializeField] private float groundDetectionDistance = 1f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private Vector2 boxSize = new Vector2(0.6f, 0.1f);
        [SerializeField] private float deathJumpForce = 6;
        
        private Vector2 _direction = Vector2.left;
        private Vector2 _wallCheckerPos;
        private Vector2 _boxCheckerPos;
        private float _animatorSpeed;
        private float _slowedDownSpeed = .15f;
        private float _normalSpeed = 2.5f;
        private int _initialHealth = 1;
        
        private bool _setInitialVelocity;
        private bool _isJumping;
        private bool _didDeathJump;
        private bool _isDead;
        private bool IsDead 
        {
            get => _isDead; 
            set { _isDead = value; Animator.SetBool(DeathStr, value); }
        }

        private void Start()
        {
            _animatorSpeed = Animator.speed;
        }

        protected override void  OnEnable()
        {
            base.OnEnable();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // check
            CalculateDirection();
            GameEvents.Instance().OnSwitchCameras += OnSwitchCameras;
            GameEvents.Instance().OnPlayerLostLife += Return;
            GameEvents.Instance().OnResetAllEnemies += Return;
            RigidBody.linearVelocity = new Vector2(speed * _direction.x, RigidBody.linearVelocity.y);
        }

        protected override void OnDisable()
        {
            GameEvents.Instance().OnSwitchCameras -= OnSwitchCameras;
            GameEvents.Instance().OnPlayerLostLife -= Return;
            GameEvents.Instance().OnResetAllEnemies -= Return;
            base.OnDisable();
        }

        public void CalculateDirection()
        {
            _direction = Vector2.left;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = false;
            if (transform.position.x <= playerTransform.position.x)
            {
                _direction = Vector2.right;
                spriteRenderer.flipX = true;
                Debug.Log("gorilla going right");
            }
            UpdateRayCastPosition();
        }

        private void UpdateRayCastPosition()
        {
            _wallCheckerPos = new Vector2(transform.position.x, transform.position.y + wallDetectionHeight);
            _boxCheckerPos = new Vector2(transform.position.x, transform.position.y + wallDetectionHeight +.1f);
        }

        protected void FixedUpdate()
        {
            if (IsDead)
            {
                if (!_didDeathJump)
                {
                    RigidBody.linearVelocity = new Vector2(RigidBody.linearVelocity.x, deathJumpForce);
                    _didDeathJump = true;
                }
                return;
            }

            if (_setInitialVelocity)
            {
                _setInitialVelocity = false;
                RigidBody.linearVelocity = new Vector2(speed * _direction.x, RigidBody.linearVelocity.y);
            }

            UpdateRayCastPosition();
            RaycastHit2D detectWall = Physics2D.Raycast(_wallCheckerPos, _direction, wallDetectionDistance, groundLayer);
            RaycastHit2D detectBox = Physics2D.Raycast(_boxCheckerPos, _direction, wallDetectionDistance, groundLayer);
            RaycastHit2D isWallHigh = Physics2D.Raycast(new Vector3(_wallCheckerPos.x, _wallCheckerPos.y + .6f, 0f), _direction, wallDetectionDistance, groundLayer);
    
            Vector2 boxCenter = CalculateBoxCastCenter();
            RaycastHit2D detectGround = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0f, groundLayer);
    
            if (!detectGround)
                Animator.speed = 0;
            
            if (detectWall || detectBox)
            {
                if (_isJumping)
                    return;
                RigidBody.linearVelocity = Vector2.up * jumpForce;
                if (!isWallHigh)
                    RigidBody.linearVelocity *= 3/4f;
                _isJumping = true;
                Animator.speed = 0;
                return;
            }
            _isJumping = false;
            if (detectGround)
            {
                RigidBody.linearVelocity = new Vector2(speed * _direction.x, RigidBody.linearVelocity.y);
                Animator.speed = _animatorSpeed;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag("Gorilla Falling Trigger"))
            {
                speed = _slowedDownSpeed;
                Debug.Log("gorilla should fall");
            }

            if (other.CompareTag("Gorilla Trun Trigger"))
            {
                ChangeDirections();
            }
        }

        private void ChangeDirections()
        {
            _direction *= -1;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = !spriteRenderer.flipX;
            UpdateRayCastPosition();
        }


        protected override void Die()
        {
            IsDead = true;
            this.GameObject().layer = LayerMask.NameToLayer("Dead Enemies");
        }
        
        protected override void OnPlayerDied()
        {
            RigidBody.simulated = false;
        }
        
        private void OnDrawGizmos()
        {
            if (RigidBody == null) 
                return;
            
            Gizmos.color = Color.red;
            
            // wall detector
            Gizmos.DrawRay(_wallCheckerPos, _direction * wallDetectionDistance);
            Gizmos.DrawRay(new Vector3(_wallCheckerPos.x, _wallCheckerPos.y + .6f, 0f), _direction * wallDetectionDistance);
            
            // ground detector
            var boxCenter = CalculateBoxCastCenter();

            Gizmos.DrawWireCube(boxCenter, boxSize);
        }

        private Vector2 CalculateBoxCastCenter()
        {
            Vector2 boxCenter = RigidBody.position + Vector2.up * groundDetectionDistance;
            return boxCenter;
        }

        public void Reset()
        {
            health = _initialHealth;
            IsDead = false;
            Animator.SetTrigger(ResetStr);
            RigidBody.simulated = true;
            this.GameObject().layer = LayerMask.NameToLayer("Enemies");
            CalculateDirection();
            speed = _normalSpeed;
        }

        public void setInitialVelocity()
        {
            _setInitialVelocity = true;
        }

        private void OnBecameInvisible()
        {
            Return();
        }

        private void Return()
        {
            GorillaMonoPool.Instance?.Return(this);
        }

        private void OnSwitchCameras(Transform t, float f)
        {
            Return();
        }
    }
}