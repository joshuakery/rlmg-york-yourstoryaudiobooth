using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JoshKery.York.AudioRecordingBooth
{
    /// <summary>
    /// Slider that controls audioSource playback
    /// 
    /// NOTE: There is a bug in UnityWebRequest which is resulting in AudioClip.length property returning DOUBLE the actual length.
    /// https://forum.unity.com/threads/audioclip-length-is-incorrect-when-loading-from-webrequest-getaudioclip.1082183/
    /// </summary>
    public class PlaybackUIManager : Slider
    {
        [SerializeField]
        private AudioSource audioSource;

        private bool isSeekable;

        protected override void Update()
        {
            base.Update();

            //Bug Workaround - the slider should represent the normalized time for HALF the audioClip's returned length
            if (audioSource?.clip != null && !isSeekable)
            {
                value = Mathf.Min(audioSource.time / (audioSource.clip.length / 2f), 1f);
            }

            ///Bug Workaround - don't let the clip act like it's playing beyond the audio length
            if (audioSource?.clip != null && audioSource.isPlaying && audioSource.time > audioSource.clip.length / 2f)
            {
                audioSource.Pause();
                audioSource.time = audioSource.clip.length / 2f;
            }

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            isSeekable = true;

            SeekAudio(value);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            isSeekable = false;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (isSeekable)
            {
                SeekAudio(value);
            }
        }

        private void PlayAudio()
        {
            if (audioSource != null)
                audioSource.Play();
        }

        private void PauseAudio()
        {
            if (audioSource != null)
                audioSource.Pause();
        }

        /// <summary>
        /// Seeks Audio Source to given time.
        /// NOTE: There is a bug in UnityWebRequest which is resulting in AudioClip.length property returning DOUBLE the actual length.
        /// https://forum.unity.com/threads/audioclip-length-is-incorrect-when-loading-from-webrequest-getaudioclip.1082183/
        /// </summary>
        /// <param name="newValue">Normalized time</param>
        private void SeekAudio(float newValue)
        {
            if (audioSource != null && isSeekable)
            {
                if (audioSource?.clip != null)
                {
                    //Bug Workaround - Seek to the position relative to HALF the AudioClip's returned length
                    audioSource.time = (audioSource.clip.length / 2f) * newValue;
                }
            }
        }

        public void OnPlayPauseAudio()
        {
            if (audioSource != null)
            {
                if (audioSource.isPlaying)
                    PauseAudio();
                else
                    PlayAudio();
            }
        }

    }
}


