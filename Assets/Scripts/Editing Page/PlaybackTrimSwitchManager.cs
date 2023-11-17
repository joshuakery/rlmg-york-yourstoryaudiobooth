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

        [SerializeField]
        private AudioVizWindow audioVizWindow;

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

            //todo kill audioVizWindow current tween here
            //todo manage the sequence here?
            // e.g. call the opens and closes on the respective components, don't bury it in the audioVizWindow
            //Open
            /*
             * Open for trimming
             * border fade in and playback head fade out
             * then shrink border and viz (their open actions)
             * then fade in trim handles
             * 
             * Close from trimming
             * handles fade out and covers fade in
             * then grow border and viz (their close actions)
             * then fade out border and fade in playback head
             * 
             * 
             * */

            if (audioVizWindow != null)
                if (toPlayback)
                    audioVizWindow.Close();
                else
                    audioVizWindow.Open();
            
        }

        private void SwitchHandles(bool toPlayback)
        {
            if (playbackHandle != null && trimSliderWindow != null)
            {
                if (!toPlayback)
                {
/*                    playbackHandle.Close();
                    trimSliderWindow.Open();*/

                    isPlayback = false;

                    onTrim?.Invoke();
                }
                else
                {
/*                    playbackHandle.Open();
                    trimSliderWindow.Close();*/

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

