using System.Collections;
using Pool;
using UnityEngine;

namespace GameFlow
{
    public abstract class OnViewSpawner<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {   
        [SerializeField] Transform position;
        [SerializeField] float spawnDelay = 1f;
        
        private bool _isSpawning;
        
        private void OnDestroy()
        {
            _isSpawning = false;
        }

        private void OnBecameVisible() //note: add renderer to inspector for this event to invoke
        {
            _isSpawning = true;
            StartCoroutine(Spawn());
        }
        
        private void OnBecameInvisible()
        {
            _isSpawning = false;
        }

        private IEnumerator Spawn()
        {
            if (_isSpawning)
            {
                yield return new WaitForSeconds(spawnDelay);
                GameObject obj = MonoPool<T>.Instance.Get().gameObject;
                obj.transform.position = transform.position;
            }
        }
    }
}