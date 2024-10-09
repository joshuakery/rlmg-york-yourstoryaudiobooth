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

        private AudioRecorderProcess audioRecorderProcess;

        [SerializeField]
        private AudioTrimProcess audioTrimProcess;

        private void Awake()
        {
            audioRecorderProcess = FindObjectOfType<AudioRecorderProcess>();
        }

        private void OnEnable()
        {
            if (audioRecorderProcess != null)
                audioRecorderProcess.onStartRequested += OnAudioRecorderStartRequested;

            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.AddListener(OnClipLoaded);
        }

        private void OnDisable()
        {
            if (audioRecorderProcess != null)
                audioRecorderProcess.onStartRequested -= OnAudioRecorderStartRequested;

            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.RemoveListener(OnClipLoaded);
        }

        private void OnAudioRecorderStartRequested()
        {
            Init();
        }

        private void OnClipLoaded()
        {
/*            if (audioClipLoader != null)
            {
                if (minMaxSlider != null)
                {
                    minMaxSlider.SetValues(0f, 1f);
                }
            }*/
        }

        public void Init()
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
        /// Formats seconds to HOURS:MM:SS.MILLISECONDS format
        /// https://stackoverflow.com/questions/463642/how-can-i-convert-seconds-into-hourminutessecondsmilliseconds-time
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private string FormatSexagesimal(float seconds)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

            return string.Format(
                "{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds
            );
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
                string seek = FormatSexagesimal(minMaxSlider.Values.minValue * (audioClipLoader.CurrentClip.length / 2f));
                string duration = FormatSexagesimal((minMaxSlider.Values.maxValue - minMaxSlider.Values.minValue) * (audioClipLoader.CurrentClip.length / 2f));

                audioTrimProcess.OnStartTrim(seek,duration);
            }
        }
    }
}


