using System;
using UnityEngine;
using Utilities;

namespace Audio
{
    public class BackgroundSound : MonoBehaviour
    {
        [SerializeField] private AudioWrapper level;
        [SerializeField] private AudioWrapper bossFight;
        
        private AudioSource _currentAudio;
        private bool _isBossFight;

        private void Start()
        {
            _currentAudio = AudioManager.Instance.PlaySoundWithLoop(level, transform);
        }

        private void OnEnable()
        {
            GameEvents.Instance().OnEnterBossRoomFinished += OnBossFight;
            GameEvents.Instance().OnPlayerLostLife += OnRespawn;
        }
        
        private void OnDisable()
        {
            GameEvents.Instance().OnEnterBossRoomFinished -= OnBossFight;
            GameEvents.Instance().OnPlayerLostLife -= OnRespawn;
        }

        private void OnBossFight()
        {
            Destroy(_currentAudio.gameObject);
            PlayNewAudio(bossFight);
            _isBossFight = true;
        }

        private void OnRespawn()
        {
            if (_isBossFight)
            {
                Destroy(_currentAudio.gameObject);
                PlayNewAudio(level);
                _isBossFight = false;
            }
        }
        
        private void PlayNewAudio(AudioWrapper audioWrapper)
        {
            if (_currentAudio != null)
            {
                _currentAudio.Stop();
                Destroy(_currentAudio.gameObject);
            }
    
            _currentAudio = AudioManager.Instance.PlaySoundWithLoop(audioWrapper, transform);
        }
    }
}