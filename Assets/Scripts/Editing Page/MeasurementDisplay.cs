using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;

namespace JoshKery.York.AudioRecordingBooth
{
    public class MeasurementDisplay : MonoBehaviour
    {
        [SerializeField]
        private AudioClipLoader audioClipLoader;

        [SerializeField]
        private MinMaxSlider minMaxSlider;

        [SerializeField]
        private PlaybackTrimSwitchManager switchManager;

        [SerializeField]
        private TMP_Text[] textDisplays;

        private void OnEnable()
        {
            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.AddListener(OnClipLoaded);
        }

        private void OnDisable()
        {
            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.RemoveListener(OnClipLoaded);
        }

        private void OnClipLoaded()
        {
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            Debug.Log("updaing display");
            if (audioClipLoader == null) { return; }
            if (switchManager == null) { return; }
            if (minMaxSlider == null) { return; }
            if (textDisplays == null || textDisplays.Length <= 1) { return; }

            //Compensating for double-length Unity bug
            float fraction = minMaxSlider.Values.maxValue - minMaxSlider.Values.minValue;
            float clipDuration = audioClipLoader.CurrentClip.length / 2f;

            //Depending on which mode, playback or trimming, display the time based on the slider or not
            float displayDuration = switchManager.isPlayback ? clipDuration * fraction : clipDuration;
            float startTime = switchManager.isPlayback ? minMaxSlider.Values.minValue * clipDuration : 0;

            float displayInterval = displayDuration / (textDisplays.Length - 1);

            for (int i = 0; i < textDisplays.Length; i++)
            {
                TMP_Text textDisplay = textDisplays[i];

                if (textDisplay != null)
                {
                    float time = startTime + (i * displayInterval);

                    TimeSpan timeSpan = TimeSpan.FromSeconds(time);

                    textDisplay.text = string.Format(
                        "<mspace=0.75em>{0}</mspace>:<mspace=0.75em>{1}</mspace>",
                        timeSpan.ToString("mm"),
                        timeSpan.ToString("ss")
                    );
                }
            }
        }
    }
}


