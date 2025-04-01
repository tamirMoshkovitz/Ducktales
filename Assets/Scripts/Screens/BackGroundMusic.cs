using System;
using Audio;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioWrapper audioWrapper;
    private bool _playing = false;
    private void Start()
    {
        if (!_playing)
        {
            _playing = true;
            AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);
            audioSource.clip = audioWrapper.clip;
            audioSource.volume = audioWrapper.volume;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.Play();
        }
    }
}
