using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace Audio
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioWrapper bounce;
        [SerializeField] private AudioWrapper climb;
        [SerializeField] private AudioWrapper land;
        [SerializeField] private AudioWrapper hurt;
        [SerializeField] private AudioWrapper puttSuccess;
        [SerializeField] private AudioWrapper puttFailed;
        [SerializeField] private AudioWrapper lifeLost;
        [SerializeField] private AudioWrapper gameOver;
        [SerializeField] private AudioWrapper finish;

        private AudioSource _currentClimbAudio;


        private void OnEnable()
        {
            GameEvents.Instance().OnPlayerHurt += PlayHurt;
            GameEvents.Instance().OnPlayerLostLife += PlayLifeLost;
            GameEvents.Instance().OnPlayerDeath += PlayGameOver;
            GameEvents.Instance().OnPuttFailed += PlayPuttFailed;
            GameEvents.Instance().OnPuttSuccess += PlayPuttSuccess;
            GameEvents.Instance().OnBounceHit += PlayBounce;
            GameEvents.Instance().OnLandOnGround += PlayLand;
            GameEvents.Instance().OnFinishLevel += PlayFinish;
            GameEvents.Instance().OnClimbing += PlayClimb;
            GameEvents.Instance().OnStopedClimbing += StopPlayingClimb;
        }

        private void OnDisable()
        {
            GameEvents.Instance().OnPlayerHurt -= PlayHurt;
            GameEvents.Instance().OnPlayerLostLife -= PlayLifeLost;
            GameEvents.Instance().OnPlayerDeath -= PlayGameOver;
            GameEvents.Instance().OnPuttFailed -= PlayPuttFailed;
            GameEvents.Instance().OnPuttSuccess -= PlayPuttSuccess;
            GameEvents.Instance().OnBounceHit -= PlayBounce;
            GameEvents.Instance().OnLandOnGround -= PlayLand;
            GameEvents.Instance().OnFinishLevel -= PlayFinish;
            GameEvents.Instance().OnClimbing -= PlayClimb;
            GameEvents.Instance().OnStopedClimbing -= StopPlayingClimb;
        }

        private void PlayHurt() { StartCoroutine(PlayHurtHelper()); }
        
        private void PlayLand() => AudioManager.Instance.PlaySoundWithNoLoop(land, transform, true); 
        
        private void PlayPuttSuccess() => AudioManager.Instance.PlaySoundWithNoLoop(puttSuccess, transform, true); 
        
        private void PlayPuttFailed() => AudioManager.Instance.PlaySoundWithNoLoop(puttFailed, transform, true); 
        
        private void PlayLifeLost() => AudioManager.Instance.PlaySoundWithNoLoop(lifeLost, transform, false); 

        private void PlayBounce() => AudioManager.Instance.PlaySoundWithNoLoop(bounce, transform, true); 
        
        private void PlayGameOver() => AudioManager.Instance.PlaySoundWithNoLoop(gameOver, transform, true);
        private void PlayFinish() => AudioManager.Instance.PlaySoundWithNoLoop(finish, transform, true);

        private IEnumerator PlayHurtHelper()
        {
            yield return new WaitForSeconds(0.05f);
            AudioManager.Instance.PlaySoundWithNoLoop(hurt, transform, true);
        }

        private void PlayClimb()
        {
            if (_currentClimbAudio != null && _currentClimbAudio.isPlaying) 
                return;
            _currentClimbAudio = AudioManager.Instance.PlaySoundWithLoop(climb, transform);
        }

        private void StopPlayingClimb()
        {
            if (_currentClimbAudio != null)
            {
                _currentClimbAudio.Stop();
                Destroy(_currentClimbAudio.gameObject);
            }
        }
    }
}