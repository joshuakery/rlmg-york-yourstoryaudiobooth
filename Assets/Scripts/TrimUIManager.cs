using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TrimUIManager : MonoBehaviour
    {
        [SerializeField]
        private AudioClipLoader audioClipLoader;

        [SerializeField]
        private MinMaxSlider minMaxSlider;

        [SerializeField]
        private AudioTrimProcess audioTrimProcess;

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
            if (audioClipLoader != null)
            {
                if (minMaxSlider != null)
                {
                    minMaxSlider.SetValues(0f, 1f);
                }
            }
        }

        /// <summary>
        /// Send the startTime and duration values from the slider to the trim process.
        /// NOTE: There is a bug in UnityWebRequest which is resulting in AudioClip.length property returning DOUBLE the actual length.
        /// https://forum.unity.com/threads/audioclip-length-is-incorrect-when-loading-from-webrequest-getaudioclip.1082183/
        /// </summary>
        public void OnTrim()
        {
            if (audioTrimProcess != null && audioClipLoader != null && minMaxSlider != null)
            {
                audioTrimProcess.OnStartTrim(
                    Mathf.RoundToInt(
                        minMaxSlider.Values.minValue * (audioClipLoader.CurrentClip.length / 2f)
                    ).ToString(),
                    Mathf.RoundToInt(
                        (minMaxSlider.Values.maxValue - minMaxSlider.Values.minValue) * (audioClipLoader.CurrentClip.length / 2f)
                    ).ToString()
                );
            }
        }
    }
}


