using UnityEngine;

namespace Items
{
    public class FallingRock : RockContainer
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float rayLength = 3f;
        [SerializeField] private float fallingGravityScale = 2f;
        
        private static readonly int FallingStr = Animator.StringToHash("Falling");

        private bool _isFalling = false;

        private void Update()
        {
            if (_isFalling) return;
            
            Vector2 rayDirection = Vector2.down;
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);
            
            Color color = hit.collider ? Color.red : Color.green;
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, color);
            
            if (hit.collider != null)
            {
                _animator.SetTrigger(FallingStr);
                _isFalling = true;
            }
        }

        private void FixedUpdate()
        {
            if (_isFalling)
                _rigidbody.gravityScale = fallingGravityScale;
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isFalling)
            {
                _animator.SetTrigger(ExplodeStr);
            }
        }

        protected override void Return()
        {
            _isFalling = false;
            gameObject.SetActive(false);
        }
    }

}