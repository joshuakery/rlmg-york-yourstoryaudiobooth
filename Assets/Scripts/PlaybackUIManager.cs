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

        /// <summary>
        /// When not isSeekable, the slider will jump to the audioSource.clip's position
        /// isSeekable is set to true OnPointerDown,
        /// thereby allowing this to set the audioSource.clip's position during OnDrag
        /// </summary>
        private bool isSeekable;

        public delegate void OnPlayEvent();
        public OnPlayEvent onPlay;

        public delegate void OnPauseEvent();
        public OnPauseEvent onPause;

        public delegate void OnPointerDownEvent();
        public OnPointerDownEvent onPointerDownEvent;

        public delegate void OnPointerUpEvent();
        public OnPointerUpEvent onPointerUpEvent;

        protected override void Update()
        {
            base.Update();

            //Bug Workaround - the slider should represent the normalized time for HALF the audioClip's returned length
            if (audioSource?.clip != null && !isSeekable)
            {
                //Update the slider in parallel with the audioSource's playback
                value = audioSource.time / (audioSource.clip.length / 2f);
            }

            
            if (audioSource?.clip != null && audioSource.isPlaying)
            {
                //Bug Workaround - don't let the clip act like it's playing beyond the audio length
                if (audioSource.time >= audioSource.clip.length / 2f)
                {
                    PauseAudio();
                    audioSource.time = audioSource.clip.length / 2f;
                }
                //And limit audioSource playback to the slider max
                else if (audioSource.time >= maxValue * (audioSource.clip.length / 2f))
                {
                    PauseAudio();
                    audioSource.time = maxValue * (audioSource.clip.length / 2f);
                }
            }

            

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            isSeekable = true;

            SeekAudio(value);

            onPointerDownEvent?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            isSeekable = false;

            onPointerUpEvent?.Invoke();
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
            {
                //Restart if at the end
                if (audioSource.time >= (audioSource.clip.length / 2f) * 0.99f)
                    audioSource.time = minValue * (audioSource.clip.length / 2f);

                audioSource.Play();
            }
                

            onPlay?.Invoke();
        }

        public void PauseAudio()
        {
            if (audioSource != null)
                audioSource.Pause();

            onPause?.Invoke();
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

        public void Init()
        {
            PauseAudio();

            if (audioSource != null)
                audioSource.time = minValue * (audioSource.clip.length / 2f);
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


