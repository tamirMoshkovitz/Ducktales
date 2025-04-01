using System.Collections;
using UnityEngine;
using Utilities;

namespace Cameras
{
    
    
    
    public class TriggerChecker : MonoBehaviour
    {
        [SerializeField] private Transform firstTransform;
        [SerializeField] private Transform secondTransform;
        [SerializeField] private float animationTime = 1f;

        private bool _isTriggered;

        public bool IsTriggered => _isTriggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Cameras Switch Collider") && !_isTriggered)
            {
                _isTriggered = true;
                Transform target = CalculatePosition(other.transform);
                GameEvents.Instance().OnSwitchCameras?.Invoke(target, animationTime);
                StartCoroutine(ResetTriggerAfterDelay(1f));
            }
        }


        private IEnumerator ResetTriggerAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _isTriggered = false;
        }


        private Transform CalculatePosition(Transform player)
        {
            float sqrDistanceFromFirstPos = (player.position - firstTransform.position).sqrMagnitude;
            float sqrDistanceFromSecondPos = (player.position - secondTransform.position).sqrMagnitude;
            return sqrDistanceFromFirstPos > sqrDistanceFromSecondPos ? firstTransform : secondTransform;
        }
    }
}