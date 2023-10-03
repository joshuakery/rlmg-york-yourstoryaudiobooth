using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace JoshKery.York.AudioRecordingBooth
{
    public class AudioTrimProcess : ExternalProcess
    {
        [SerializeField]
        private string INFILENAME = "out.mp3";

        private string inFilePath;

        [SerializeField]
        private string OUTFILENAME = "out_trimmed.mp3";

        private string outFilePath;

        /// <summary>
        /// Executable file for external processes.
        /// </summary>
		private const string FFMPEG = "ffmpeg";

        /// <summary>
        /// Template for ffmpeg for recording from an audio input device on Windows with overwrite
        /// </summary>
		private string DOTRIMTEMPLATE = "-ss {0} -t {1} -y -i \"{2}\" -acodec copy \"{3}\" ";

        private Task currentTask;

        public bool IsTrimming
        {
            get
            {
                return currentTask != null && !currentTask.IsCompleted;
            }
        }


        #region Public UnityEvents
        private UnityEvent _OnTrimStarted;
        /// <summary>
        /// Invoked AFTER private RecordingStarted callback is invoked
        /// </summary>
        public UnityEvent OnTrimStarted
        {
            get
            {
                if (_OnTrimStarted == null)
                    _OnTrimStarted = new UnityEvent();

                return _OnTrimStarted;
            }
        }

        private UnityEvent _OnTrimFinished;
        /// <summary>
        /// Invoked AFTER private RecordingAllFinished callback is invoked
        /// </summary>
        public UnityEvent OnTrimFinished
        {
            get
            {
                if (_OnTrimFinished == null)
                    _OnTrimFinished = new UnityEvent();

                return _OnTrimFinished;
            }
        }
        #endregion

        void Start()
        {
            defaultProcessFileName = FFMPEG;

            inFilePath = Path.Combine(Application.streamingAssetsPath, INFILENAME);
            outFilePath = Path.Combine(Application.streamingAssetsPath, OUTFILENAME);
        }

        /// <summary>
        /// Format the parameters into the STARTRECORDINGTEMPLATE
        /// And pass that command string along to the external process thread via Run
        /// </summary>
        /// <param name="deviceName">Name of audio input device</param>
        /// <param name="fileOut">Path to save audio file to</param>
        private void StartTrim(string startTime, string duration, string fileIn, string fileOut)
        {
            if (currentTask == null || currentTask.IsCompleted)
            {
                currentTask = Run(
                    new Settings(
                        FFMPEG,
                        string.Format(DOTRIMTEMPLATE, startTime, duration, fileIn, fileOut),
                        TrimAllFinished
                    )
                );
            }

            OnTrimStarted.Invoke();
        }

        /// <summary>
        /// Public method for starting the recording.
        /// </summary>
		public void OnStartTrim(string startTime, string duration)
        {
            StartTrim(startTime, duration, inFilePath, outFilePath);
        }



        /// <summary>
        /// Callback invoked after all processes in the StartRecording command strings are complete.
        /// </summary>
        /// <param name="exitCodes"></param>
        private void TrimAllFinished(int[] exitCodes)
        {
            UnityEngine.Debug.Log("all finished trim");
            foreach (int exitCode in exitCodes)
            {
                UnityEngine.Debug.Log(exitCode);
            }

            OnTrimFinished.Invoke();
        }
    }
}


