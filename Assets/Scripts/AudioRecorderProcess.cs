
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JoshKery.York.AudioRecordingBooth
{
	public class AudioRecorderProcess : ExternalProcess
	{
        [SerializeField]
        private string OUTFILENAME = "out.mp3";

        private string outFilePath;

        /// <summary>
        /// Executable file for external processes.
        /// </summary>
		private const string FFMPEG = "ffmpeg";

        /// <summary>
        /// Template for ffmpeg for recording from an audio input device on Windows with overwrite
        /// </summary>
		private string STARTRECORDINGTEMPLATE = "-f dshow -y -i audio=\"{0}\" {1}";

        /// <summary>
        /// Template to issue to ffmpeg process to exit
        /// </summary>
		private string STOPRECORDINGTEMPLATE = "q";

        private Task currentTask;

        public bool isRecording
        {
            get
            {
                return currentTask != null && !currentTask.IsCompleted;
            }
        }
            

        #region Public UnityEvents
        private UnityEvent _onRecordingStarted;
        /// <summary>
        /// Invoked AFTER private RecordingStarted callback is invoked
        /// </summary>
        public UnityEvent onRecordingStarted
        {
            get
            {
                if (_onRecordingStarted == null)
                    _onRecordingStarted = new UnityEvent();

                return _onRecordingStarted;
            }
        }

        private UnityEvent _onRecordingFinished;
        /// <summary>
        /// Invoked AFTER private RecordingAllFinished callback is invoked
        /// </summary>
        public UnityEvent onRecordingFinished
        {
            get
            {
                if (_onRecordingFinished == null)
                    _onRecordingFinished = new UnityEvent();

                return _onRecordingFinished;
            }
        }
        #endregion

        void Start()
		{
			defaultProcessFileName = FFMPEG;

            outFilePath = Path.Combine(Application.streamingAssetsPath, OUTFILENAME);
		}

        /// <summary>
        /// Format the parameters into the STARTRECORDINGTEMPLATE
        /// And pass that command string along to the external process thread via Run
        /// </summary>
        /// <param name="deviceName">Name of audio input device</param>
        /// <param name="fileOut">Path to save audio file to</param>
        private void StartRecording(string deviceName, string fileOut)
        {
            if (currentTask == null || currentTask.IsCompleted)
            {
                currentTask = Run(
                    new Settings(
                        FFMPEG,
                        string.Format(STARTRECORDINGTEMPLATE, deviceName, fileOut),
                        STOPRECORDINGTEMPLATE,
                        RecordingAllFinished
                    )
                );
            }
        }

        /// <summary>
        /// Signals a System.Threading.AutoResetEvent to let the external process thread continue
        /// and issue the STOPRECORDINGTEMPLATE command to the active process
        /// </summary>
		private void StopRecording()
        {
            standardInputEvent.Set();
        }

        /// <summary>
        /// Public method for starting the recording.
        /// </summary>
		public void OnStartRecording()
        {
			StartRecording("Microphone Array (AMD Audio Device)", outFilePath);
        }

        /// <summary>
        /// Public method for stopping the recording.
        /// </summary>
        public void OnStopRecording()
        {
            StopRecording();
        }

        

        /// <summary>
        /// Callback invoked after all processes in the StartRecording command strings are complete.
        /// </summary>
        /// <param name="exitCodes"></param>
        private void RecordingAllFinished(int[] exitCodes)
        {
            UnityEngine.Debug.Log("all finished");
            foreach (int exitCode in exitCodes)
            {
                UnityEngine.Debug.Log(exitCode);
            }

            onRecordingFinished.Invoke();
        }
	}

}

