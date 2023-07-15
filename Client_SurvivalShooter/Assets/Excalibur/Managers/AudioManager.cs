using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public class AudioManager : Singleton<AudioManager>
    {
        private AudioListener _audioListener;
        private float _volume;

        public float GetVolume()
        {
            return _volume;
        }

        public void SetVolume(float volume)
        {
            _volume = Mathf.Clamp01(volume);
        }
    }
}
