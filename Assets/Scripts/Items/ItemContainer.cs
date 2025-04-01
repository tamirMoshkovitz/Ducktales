using System.Collections;
using GameFlow;
using UnityEngine;
using IPoolable = Pool.IPoolable;

namespace Items
{
    
    
    
    
    public class ItemContainer : MonoBehaviour
    {
        [SerializeField] private CollectableSpawnerBase spawner;
        
        protected static readonly int ExplodeStr = Animator.StringToHash("Explode");
        protected Animator _animator;

        protected virtual void OnEnable()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Pogo"))
            {
                StartCoroutine(Explode(new WaitForSeconds(0f)));
            }
        }

        public IEnumerator Explode(WaitForSeconds delay) //TODO: change to void methos that is being invoked at the end of the putt animation
        {
            // GetComponent<BoxCollider2D>().//TODO: set collider off so diamonds wont stop on thir own box
            spawner?.Spawn();
            yield return delay;
            _animator.SetTrigger(ExplodeStr);
        }

        protected virtual void Return() //TODO: why it that here
        {
            gameObject.SetActive(false);
        }
    }
}
