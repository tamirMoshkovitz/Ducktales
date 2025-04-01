using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace GameManagement
{
    public class RumbleManager : MonoSingleton<RumbleManager>
    {
        [Header("General")]
        [SerializeField] private float lowFrequency;
        [SerializeField] private float highFrequency;
        
        [Header("Hurt")]
        [SerializeField] private float hurtRumbleDuration;
        
        [Header("Death")]
        [SerializeField] private float deathRumbleDuration;
        
        private Gamepad _gamepad;

        private void OnEnable()
        {
            GameEvents.Instance().OnPlayerHurt += HurtRumbel;
            GameEvents.Instance().OnPlayerLostLife += DeathRumble;
        }

        private void OnDisable()
        {
            GameEvents.Instance().OnPlayerHurt -= HurtRumbel;
            GameEvents.Instance().OnPlayerLostLife -= DeathRumble;

        }

        private void HurtRumbel()
        {
            RumblePulse(hurtRumbleDuration, lowFrequency, highFrequency);
            Debug.Log("Hurt Rumble Pulse");
        }

        private void DeathRumble()
        {
            RumblePulse(deathRumbleDuration, lowFrequency, highFrequency);
            Debug.Log("Death Rumble Pulse");
        }

        private void RumblePulse(float duration, float lowFrequency, float highFrequency)
        {
            _gamepad = Gamepad.current;
            if (_gamepad == null)
            {
                Debug.Log("No gamepad");
                return;
            }

            _gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            StartCoroutine(Rumble(duration));
        }

        private IEnumerator Rumble(float duration)
        {
            float timePassed = 0f;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                yield return null;
            }
    
            _gamepad.SetMotorSpeeds(0, 0);
            Debug.Log("Rumble Ended");
        }


    }
}