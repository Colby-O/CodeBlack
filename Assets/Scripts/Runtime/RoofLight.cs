using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack
{
    public class RoofLight : MonoBehaviour
    {

        [SerializeField] private AudioSource _audioSrc;
        [SerializeField] private List<AudioClip> _clips;

        private bool _started = false;

        private void Start()
        {
            if (_audioSrc == null) _audioSrc = GetComponent<AudioSource>();

            _audioSrc.clip = _clips[Random.Range(0, _clips.Count)];
        }

        private void Update()
        {
            if (CodeBlackGameManager.hasStarted && !_started)
            {
                _started = true;
                _audioSrc.Play();
            }
        }
    }
}