using System;
using UnityEngine;

namespace _Scripts.Managers.Sfx
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayAudioOnEnable : MonoBehaviour
    {
        private AudioSource audioSource;
        
        [Range(-3f, 3f)] public float pitchMin = .3f;
        [Range(-3f, 3f)] public float pitchMax = 1.5f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            audioSource ??= GetComponent<AudioSource>();
            audioSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
            audioSource.Play();
        }
    }
}
