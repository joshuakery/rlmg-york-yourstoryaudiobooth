using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
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
        private BaseWindow trimSliderWindow;

        [SerializeField]
        private MinMaxSlider trimSlider;

        public bool isPlayback = true;

        public delegate void OnSwitchToTrimEvent();
        public OnSwitchToTrimEvent onTrim;

        public delegate void OnSwitchToPlaybackEvent();
        public OnSwitchToPlaybackEvent onPlayback;

        public void Switch(bool toPlayback)
        {
            if (playbackManager != null)
            {
                if (trimSlider != null)
                {
                    playbackManager.minValue = trimSlider.Values.minValue;
                    playbackManager.maxValue = trimSlider.Values.maxValue;
                }

                if (toPlayback) //Pause AND start start to min
                    playbackManager.Init();
                else //Only pause here so we don't jump the slider around
                    playbackManager.PauseAudio();

            }

            //just hide the handles
            SwitchHandles(toPlayback);
            
        }

        private void SwitchHandles(bool toPlayback)
        {
            if (playbackHandle != null && trimSliderWindow != null)
            {
                if (!toPlayback)
                {
                    playbackHandle.Close();
                    trimSliderWindow.Open();

                    isPlayback = false;

                    onTrim?.Invoke();
                }
                else
                {
                    playbackHandle.Open();
                    trimSliderWindow.Close();

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

