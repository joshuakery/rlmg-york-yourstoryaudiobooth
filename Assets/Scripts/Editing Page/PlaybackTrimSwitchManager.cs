using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PlaybackTrimSwitchManager : MonoBehaviour
    {
        [SerializeField]
        private PlaybackUIManager playbackManager;

        [SerializeField]
        private BaseWindow playbackHandle;

        [SerializeField]
        private BaseWindow trimSlider;

        public bool isPlayback = true;

        public delegate void OnSwitchToTrimEvent();
        public OnSwitchToTrimEvent onTrim;

        public delegate void OnSwitchToPlaybackEvent();
        public OnSwitchToPlaybackEvent onPlayback;

        public void Switch(bool toPlayback)
        {
            if (playbackManager != null)
                playbackManager.PauseAudio();

            //just hide the handles
            SwitchHandles(toPlayback);
            
        }

        private void SwitchHandles(bool toPlayback)
        {
            if (playbackHandle != null && trimSlider != null)
            {
                if (!toPlayback)
                {
                    playbackHandle.Close();
                    trimSlider.Open();

                    isPlayback = false;

                    onTrim?.Invoke();
                }
                else
                {
                    playbackHandle.Open();
                    trimSlider.Close();

                    isPlayback = true;

                    onPlayback?.Invoke();
                }
            }
        }
        public void OnSwitch()
        {
            //Toggle
            Switch(!isPlayback);
        }
    }
}

