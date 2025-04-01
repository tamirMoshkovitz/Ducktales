using System;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private AudioSource soundFXObject;
        private AudioSource _currentAudio;
        private bool canOverrideCurrent;


        public void PlaySoundWithNoLoop(AudioWrapper audioWrapper, Transform parentTransform, bool canBeOverriden)
        {
            if (_currentAudio != null && canOverrideCurrent)
            {
                _currentAudio.Stop();
                Destroy(_currentAudio.gameObject);
            }
            canOverrideCurrent = canBeOverriden;
            _currentAudio = Instantiate(soundFXObject, parentTransform.position, Quaternion.identity);

            _currentAudio.clip = audioWrapper.clip;
            _currentAudio.volume = audioWrapper.volume;
            _currentAudio.loop = false;
            _currentAudio.spatialBlend = 0f;

            _currentAudio.Play();

            var clipDuration = _currentAudio.clip.length;
            Destroy(_currentAudio.gameObject, clipDuration);
        }

        public AudioSource PlaySoundWithLoop(AudioWrapper audioWrapper, Transform parentTransform)
        {
            AudioSource audioSource = Instantiate(soundFXObject, parentTransform.position, Quaternion.identity);
            audioSource.clip = audioWrapper.clip;
            audioSource.volume = audioWrapper.volume;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.Play();
            return audioSource;
        }
    }
    [Serializable]
    public class AudioWrapper
    {
        public AudioClip clip;
        [Range(0f,1f)] public float volume = 1f;
    }
}