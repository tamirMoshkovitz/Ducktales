using System;
using Cameras;
using DG.Tweening;
using Items;
using State.Interfaces;
using State.Models;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilities;
using StateMachine = State.States.StateMachine;

namespace State.Character
{
    public class PlayerMovementController : MonoBehaviour, IContext
    {
        private static readonly int DirectionStr = Animator.StringToHash("Direction");
        private static readonly int SpeedStr = Animator.StringToHash("Speed");
        private static readonly int Hurt = Animator.StringToHash("Hurt");

        [FormerlySerializedAs("speed")]
        [Header("Movement")]
        [SerializeField] private float movementSpeed = 5.0f;
        [SerializeField] private float maxSpeed = 3.5f;
        
        [Header("Jump")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float bounceForce = 6f;
        [SerializeField] private float coyoteEffect = 0.2f;
        
        [Header("Ground Check")]
        [SerializeField] Vector2 boxSize = new Vector2(0.6f, 0.1f);
        [SerializeField] private float groundCheckRelativePosition = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Two Button Events")] [SerializeField]
        private TwoButtonInput bounceInput;
        
        [Header("State Specific Colliders")]
        [SerializeField] private Collider2D pogoCollider;
        
        //Components
        private Rigidbody2D _rigidBody;
        private Animator _animator;
        
        private MovementData _movementData;

        private float _vinePosition;
        private float _animationSpeed;
        private ItemContainer _puttable;

        public float AnimationSpeed => _animationSpeed;
        
        //Input
        public InputSystem_Actions InputActions { get; private set; }
        private InputAction _jumpAction;
        private InputAction _duckAction;
        private InputAction _upButtonAction;
        private InputAction _puttAction;
        
        private Vector2 _linearVelocityPlaceHolder = Vector2.zero;
        private Vector2 _input;
        private Vector2 _initialBoxCastSize;
        private bool _isSetLinearVelocity;
        private float _gravityScalePlaceHolder;
        private bool _isSetGravityScale;
        
        readonly StateMachine _stateMachine = StateMachine.Instance();

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _movementData = MovementData.Instance();
            _movementData.SetAnimator(_animator);
            InputActions = new InputSystem_Actions();
            _jumpAction = InputActions.Player.Jump;
            _duckAction = InputActions.Player.Duck;
            _upButtonAction = InputActions.Player.UpButton;
            _puttAction = InputActions.Player.Putt;
            _initialBoxCastSize = boxSize;
        }

        private void OnEnable()
        {
            InputActions.Player.Enable();
            GameEvents.Instance().OnMovementDataChanged += OnMovementDataChanged;
            GameEvents.Instance().OnSwitchCameras += OnSwitchCameras;
            GameEvents.Instance().OnPlayerHurt += OnHurt;
            GameEvents.Instance().OnPlayerDeath += OnDeath;
            OnMovementDataChanged();

            _jumpAction.started += OnJumpStarted;
            _jumpAction.canceled += OnJumpCanceled;
            _duckAction.started += OnDuckStarted;
            _duckAction.canceled += OnDuckCanceled;
            _upButtonAction.started += OnUpStarted;
            _upButtonAction.canceled += OnUpCanceled;
            _puttAction.started += OnPuttStarted;
            bounceInput.Enable(_movementData.IsBouncingLocked);
            bounceInput.OnStateEntered += OnBounceStarted;
            bounceInput.OnStateExited += OnBounceCanceled;
            StateMachine.Instance().OnBounceStateEnter += OnEnterPogoState;
            StateMachine.Instance().OnBounceStateExit += OnExitPogoState;
            _animationSpeed = _animator.speed;
        }

        private void OnDisable()
        {
            GameEvents.Instance().OnMovementDataChanged -= OnMovementDataChanged;
            GameEvents.Instance().OnSwitchCameras -= OnSwitchCameras;
            GameEvents.Instance().OnPlayerHurt -= OnHurt;
            GameEvents.Instance().OnPlayerDeath -= OnDeath;
            _jumpAction.started -= OnJumpStarted;
            _jumpAction.canceled -= OnJumpCanceled;
            _duckAction.started -= OnDuckStarted;
            _duckAction.canceled -= OnDuckCanceled;
            _upButtonAction.started -= OnUpStarted;
            _upButtonAction.canceled -= OnUpCanceled;
            _puttAction.started -= OnPuttStarted;
            bounceInput.OnStateEntered -= OnBounceStarted;
            bounceInput.OnStateExited -= OnBounceCanceled;
            StateMachine.Instance().OnBounceStateEnter -= OnEnterPogoState;
            StateMachine.Instance().OnBounceStateExit -= OnExitPogoState;
            InputActions.Player.Disable();
            bounceInput.Disable();
        }

        private void OnMovementDataChanged()
        {
            _stateMachine.OnMovementDataChanged(this);
        }

        private void Update()
        {
            bounceInput.Update();
            _input = InputActions.Player.Move.ReadValue<Vector2>();
            _movementData.SideInputDirection = _input.x;
            _movementData.PuttableDirection = NearPuttable();
            _stateMachine.Update(_input, this);
        }

        private void FixedUpdate()
        {
            CheckGroundStatus();
            _stateMachine.FixedUpdate(this); // TODO: animation machine
            if (_isSetLinearVelocity)
            {
                _rigidBody.linearVelocity = _linearVelocityPlaceHolder;
                _isSetLinearVelocity = false;
            }
            
            if (_isSetGravityScale)
            {
                _rigidBody.gravityScale = _gravityScalePlaceHolder;
                _isSetGravityScale = false;
            }

            if (_rigidBody.linearVelocity.y < 0)
            {
                _movementData.IsInAir = true;
                if (MovementData.Instance().GetState().Equals(MovementState.Bounce))
                    OnEnterPogoState();
            }
            else
            {
                OnExitPogoState();
            }
            
            float horizontalSpeed = Math.Abs(_rigidBody.linearVelocity.x);
            if (horizontalSpeed > 0.1f)
                _animator.SetFloat(DirectionStr, _rigidBody.linearVelocity.x);
            _animator.SetFloat(SpeedStr, Math.Abs(_rigidBody.linearVelocity.x));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Vine Position Trigger"))
                _vinePosition = other.transform.position.x;
            
            if (other.CompareTag("Climbable"))
                _movementData
                .CanClimb = true;

        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Climbable"))
            {
                _movementData
                    .CanClimb = false;
            }        
        }

        private void OnHurt()
        {
            _animator.SetTrigger(Hurt);
            SetLinearVelocity(new Vector2(_rigidBody.linearVelocity.x, jumpForce / 2));
            _movementData.IsInAir = true;
        }

        public void SetLinearVelocity(Vector2 velocity)
        {
            if (!_isSetLinearVelocity)
            {
                _linearVelocityPlaceHolder = velocity;
                _isSetLinearVelocity = true;
            }
        }

        public Vector2 GetLinearVelocity()
        {
            return _rigidBody.linearVelocity;
        }
        
        public void SetGravityScale(float gravityScale)
        {
            if (!_isSetGravityScale)
            {
                _gravityScalePlaceHolder = gravityScale;
                _isSetGravityScale = true;
            }
        }

        public float GetGravityScale()
        {
            return _rigidBody.gravityScale;
        }

        public float GetMaxSpeed()
        {
            return maxSpeed;
        }

        public float GetBounceForce()
        {
            return bounceForce;
        }

        public float GetSpeed()
        {
            return movementSpeed;
        }

        public float GetJumpForce()
        {
            return jumpForce;
        }
        
        public void ClingToVine()
        {
            float posY = transform.position.y;
            float posZ = transform.position.z;
            transform.position = new Vector3( _vinePosition, posY, posZ );
        }

        public void SetAnimationSpeed(float animationSpeed)
        {
            _animator.speed = animationSpeed;
        }

        private void OnJumpStarted(InputAction.CallbackContext obj)
        {
            if (_movementData
                .IsGrounded)
            {
                SetLinearVelocity(new Vector2(_rigidBody.linearVelocity.x, jumpForce));
            }

            else if (_movementData
                .GetState() == MovementState.Climb)
            {
                _movementData
                    .TryingToClimb = false;
            }

            _movementData
                .IsInAir = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext obj)
        {
            if (_rigidBody.linearVelocity.y > 0 && !_movementData
                .IsBouncing)
            {
                SetLinearVelocity(new Vector2(_rigidBody.linearVelocity.x,
                    _rigidBody.linearVelocity.y * coyoteEffect));
            }

            _movementData
                .IsInAir = false;
        }

        private void OnUpStarted(InputAction.CallbackContext obj)
        {
            if (_movementData
                .GetState() != MovementState.Climb)
            {
                _movementData
                    .TryingToClimb = true;
            }
        }

        private void OnUpCanceled(InputAction.CallbackContext obj)
        {
            if (_movementData
                .GetState() != MovementState.Climb)
            {
                _movementData
                    .TryingToClimb = false;
            }
        }
        
        private void OnDuckStarted(InputAction.CallbackContext obj)
        {
            _movementData
                .IsDucked = true;
        }

        private void OnDuckCanceled(InputAction.CallbackContext obj)
        {
            _movementData
                .IsDucked = false;
        }

        private void OnBounceStarted()
        {
            if (_movementData.GetState() == MovementState.InAir)
            {
                _movementData.IsBouncing = true;
                
            }
        }

        private void OnBounceCanceled()
        {
            if (_rigidBody.linearVelocity.y > 0 && _movementData
                .IsBouncing)
            {
                SetLinearVelocity(new Vector2(_rigidBody.linearVelocity.x,
                    _rigidBody.linearVelocity.y * coyoteEffect));
            }
            _movementData.IsBouncing = false;
            boxSize = _initialBoxCastSize;
        }

        private void OnPuttStarted(InputAction.CallbackContext obj)
        {
            if (_movementData.GetState() == MovementState.Putt)
            {
                _movementData.SetPutt();
                if (_puttable != null)
                {
                    StartCoroutine(_puttable.Explode(new WaitForSeconds(.2f)));
                    _movementData.SetPuttSuccess();
                    GameEvents.Instance().OnPuttSuccess?.Invoke();
                }
                else
                {
                    _movementData.SetPuttFail();
                    GameEvents.Instance().OnPuttFailed?.Invoke();
                }
            }
        }

        private void OnEnterPogoState()
        {
            pogoCollider.enabled = true;
            boxSize = new Vector2(.35f, _initialBoxCastSize.y);
        }

        private void OnExitPogoState()
        {
            pogoCollider.enabled = false;
            boxSize = _initialBoxCastSize;
        }
        
        private void CheckGroundStatus()
        {
            Vector2 boxCenter = CalculateGroundCheckPos();

            RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0f, groundLayer);
            
            _movementData.IsGrounded = hit.collider != null && hit.normal.y > 0.5f;
        }

        private void OnDeath()
        {
            _rigidBody.simulated = false;
        }

        private float NearPuttable()
        {
            Vector2 direction = _input.x > 0? Vector2.right : _input.x < 0 ? Vector2.left : Vector2.zero;
            Vector3 castPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            RaycastHit2D hit = Physics2D.Raycast(castPosition, direction, .45f , groundLayer );
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Container"))
                {
                    _puttable = hit.collider.gameObject.GetComponent<ItemContainer>();
                }
                else
                    _puttable = null;
                return direction.x;
            }
            _puttable = null;
            return 0f;
        }

        private void OnSwitchCameras(Transform target, float duration)
        {
            _rigidBody.linearVelocity = Vector2.zero;
            // CinemachineBrain cinemachineBrain= CamerasManager.Instance.GetComponentInChildren<CinemachineBrain>();
            // var activeBlend = cinemachineBrain.ActiveBlend;
            // var duration = activeBlend?.Duration ?? 1f;
            //// float duration = (target.position - transform.position).normalized.Abs().Equals( Vector2.up) 
            ////     ? 1f : 3f; // TODO: fix this hard-coded plaster if there is time
            transform.DOMove(target.position, duration).SetEase(Ease.Linear);
        }


        private void OnDrawGizmos()
        {
            if (_rigidBody == null) 
                return;

            Vector2 boxCenter = CalculateGroundCheckPos();

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCenter, boxSize);
            Vector2 direction = _input.x > 0? Vector2.right : _input.x < 0 ? Vector2.left : Vector2.zero;
            Vector3 castPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Gizmos.DrawRay(castPosition, direction * .5f);
        }

        private Vector2 CalculateGroundCheckPos()
        {
            return _rigidBody.position + Vector2.up * groundCheckRelativePosition;
        }
    }
}