using System;
using System.Collections;
using Enemies;
using Pool;
using UnityEngine;

namespace GameFlow
{
    public class GorillaTimedSpawner : MonoBehaviour
    {
        [SerializeField] float spawnRate = 3f;
        [SerializeField] float distanceFromPlayer = 4;
        [SerializeField] Transform player;
        
        private bool _isSpawning = false;
        private WaitForSeconds _waitForSeconds;

        private void OnEnable()
        {
            _waitForSeconds = new WaitForSeconds(spawnRate);
        }

        private void OnDestroy()
        {
            _isSpawning = false;
        }
        
        private void OnBecameVisible()
        {
            _isSpawning = true;
            StartCoroutine(StartSpawning());
        }

        private void OnBecameInvisible()
        {
            _isSpawning = false;
        }

        private IEnumerator StartSpawning()
        {
            while (_isSpawning)
            {
                SpawnGorilla();
                yield return _waitForSeconds;
            }
        }
        
        private void SpawnGorilla()
        {
            if (!_isSpawning) return;
            Gorilla gorilla = GorillaMonoPool.Instance.Get();
            gorilla.transform.position = calculatePosition();
            gorilla.CalculateDirection();
            gorilla.setInitialVelocity();
        }

        private Vector3 calculatePosition()
        {
            float x = player.position.x + 
                      ((player.position.x - transform.position.x > 0) ? -distanceFromPlayer : distanceFromPlayer);
            return new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}