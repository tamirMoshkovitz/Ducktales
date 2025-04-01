using System;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Cameras
{
    public class VerticalShake : MonoBehaviour
    {
        [Header("Ground Shake")]
        [SerializeField] private float shakeDuration = 1f;
        [SerializeField] private float shakeStrength = 0.5f;
        [SerializeField] private Transform roomTransform;
        
        private Vector3 _originalPos;

        void OnEnable()
        {
            _originalPos = roomTransform.localPosition;
            GameEvents.Instance().OnEarthquakeStarted += TriggerVerticalShake;
        }

        private void OnDisable()
        {
            GameEvents.Instance().OnEarthquakeStarted -= TriggerVerticalShake;
        }

        public void TriggerVerticalShake()
        {
            roomTransform.localPosition = _originalPos;

            roomTransform.DOShakePosition(shakeDuration, new Vector3(0, shakeStrength, 0), vibrato: 10, randomness: 0, 
                    snapping: false, fadeOut: true).SetEase(Ease.Linear)
                .OnComplete(() => roomTransform.localPosition = _originalPos);
        }
    }
}
