using System;
using System.Collections;
using State.Models;
using State.States;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace GameManagement
{
    public class LifeManager : MonoSingleton<LifeManager>
    {
        [SerializeField] private int life = 3;
        [SerializeField] private int health = 3;
        [SerializeField] private BoxCollider2D _collider;
        
        private SpriteRenderer _spriteRenderer;
        private WaitForSeconds _flickeringEffect = new WaitForSeconds(0.04f);
        private Vector2 _originalColliderOffset;
        private Vector2 _OriginalSize;
        private int _initialHealth;
        private int _initialLife;
        private Rigidbody2D _rigidbody;

        private void Start()
        {
            _spriteRenderer = GetComponentInParent<SpriteRenderer>();
            _rigidbody = GetComponentInParent<Rigidbody2D>();
            _initialHealth = health;
            _initialLife = life;
        }

        private void OnEnable()
        {
            _originalColliderOffset = _collider.offset;
            _OriginalSize = _collider.size;
            StateMachine.Instance().OnBounceStateEnter += OnBounceStateEnter;
            StateMachine.Instance().OnBounceStateExit += OnBounceStateExit;
            GameEvents.Instance().OnPlayerLostLife += OnPlayerLostLife;
            GameEvents.Instance().OnResetPlayer += OnResetPlayer;
        }

        private void OnDisable()
        {
            StateMachine.Instance().OnBounceStateEnter -= OnBounceStateEnter;
            StateMachine.Instance().OnBounceStateExit -= OnBounceStateExit;
            GameEvents.Instance().OnPlayerLostLife -= OnPlayerLostLife;
            GameEvents.Instance().OnResetPlayer -= OnResetPlayer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Pitt"))
            {
                health -= _initialHealth;
            }

            if (other.CompareTag("Boss"))
            {
                life = 0;
            }
            GameEvents.Instance().OnPlayerHurt?.Invoke();
            StartCoroutine(Hurt());
            if (life <= 0)
                GameEvents.Instance().OnPlayerDeath?.Invoke();
            if (health <= 0)
            {
                life--;
                health = _initialHealth;
                GameEvents.Instance().OnPlayerLostLife?.Invoke();
            }
        }

        private void OnPlayerLostLife()
        {
            transform.parent.position = new Vector3(-2, -1.06f , 0);
            MovementData.Instance().IsGrounded = true;
            StartCoroutine(Incarnation());
        }

        private IEnumerator Incarnation()
        {
            _rigidbody.simulated = false;
            _collider.enabled = false;
            for (int i = 0; i < 15; i++)
            {
                _spriteRenderer.enabled = false; //TODO: handle code mutiplication
                yield return _flickeringEffect;
                _spriteRenderer.enabled = true;
                yield return _flickeringEffect;
            }
            _collider.enabled = true;
            _rigidbody.simulated = true;
        }

        private void OnBounceStateEnter()
        {
            Debug.Log("change pogoState enemy trigger");
            _collider.offset = new Vector2(0, .25f);
            _collider.size = new Vector2(_collider.size.x, .5f);
        }

        private void OnBounceStateExit()
        {
            _collider.offset = _originalColliderOffset;
            _collider.size = _OriginalSize;
        }
        
        

        private IEnumerator Hurt()
        {
            Debug.Log("hurt flicker");
            health--;
            _collider.enabled = false;
            for (int i = 0; i < 10; i++)
            {
                _spriteRenderer.enabled = false;
                yield return _flickeringEffect;
                _spriteRenderer.enabled = true;
                yield return _flickeringEffect;
            }
            _collider.enabled = true;
        }

        private void Update()
        {
            if (health <= 0)
            {
                life--;
                GameEvents.Instance().OnPlayerDeath?.Invoke();
            }
        }

        private void OnResetPlayer()
        {
            GameEvents.Instance().OnPlayerLostLife?.Invoke();
            health = _initialHealth;
            life = _initialLife;
        }

        // private void OnBecameInvisible()
        // {
        //     health -= _initialHealth;
        // }
    }
}