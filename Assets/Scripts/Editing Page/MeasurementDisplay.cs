using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace JoshKery.York.AudioRecordingBooth
{
    public class MeasurementDisplay : MonoBehaviour
    {
        [SerializeField]
        private AudioClipLoader audioClipLoader;

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
            if (audioClipLoader == null) { return; }
            if (textDisplays == null || textDisplays.Length <= 1) { return; }

            //Compensating for double-length Unity bug
            float duration = audioClipLoader.CurrentClip.length / 2f;
            float interval = duration / (textDisplays.Length - 1);

            for (int i=0; i<textDisplays.Length; i++)
            {
                TMP_Text textDisplay = textDisplays[i];

                if (textDisplay != null)
                {
                    float time = i * interval;

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


