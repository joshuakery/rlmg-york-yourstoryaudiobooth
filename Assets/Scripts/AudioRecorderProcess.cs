
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    /// <summary>
    /// Parameters for ffmpeg process to use microphone and record
    /// Along with parameters for a stopwatch timeout (and backup timeout)
    /// </summary>
	public class AudioRecorderProcess : ExternalProcess
	{
        public string MicrophoneName = "Microphone Array (AMD Audio Device)";

        /// <summary>
        /// Filename for saving recording audio file
        /// </summary>
        [SerializeField]
        private string OUTFILENAME = "out.mp3";

        /// <summary>
        /// Save path for recording audio file
        /// </summary>
        private string outFilePath;

        /// <summary>
        /// Executable file for external processes.
        /// </summary>
		private const string FFMPEG = "ffmpeg.exe";

        /// <summary>
        /// Template for ffmpeg for recording from an audio input device on Windows with overwrite
        /// </summary>
		private string STARTRECORDINGTEMPLATE = "-f dshow -y -i audio=\"{0}\" {1}";

        /// <summary>
        /// Template to issue to ffmpeg process to exit
        /// </summary>
		private string STOPRECORDINGTEMPLATE = "q";

        /// <summary>
        /// Backup timeout enforced on the Main Task thread, should this MonoBehaviour be Destroyed.
        /// Milliseconds.
        /// </summary>
        private int ProcessTimeout
        {
            get
            {
                return RecordingDuration + 5000;
            }
        }

        /// <summary>
        /// Time from which stopwatch counts
        /// </summary>
        public DateTime? StartTime = null;

        /// <summary>
        /// True if the current process is the first process
        /// </summary>
        public bool isRecording
        {
            get
            {
                // The recording process should be the first and only command in the ExternalProcessAction Task
                return currentProcess == 0;
            }
        }

        /// <summary>
        /// Because the process and Task do not necessarily end as soon as the Unity main thread asks them to
        /// </summary>
        public delegate void StopRequestedEvent();

        /// <summary>
        /// Subscribe to this to listen to when the stop is requested on the Unity main thread
        /// </summary>
        public StopRequestedEvent onStopRequested;

        public delegate void RecordingSuccessEvent();

        public RecordingSuccessEvent onRecordingSuccess;

        public delegate void RecordingErrorEvent();

        public RecordingErrorEvent onRecordingError;

        /// <summary>
        /// Was the StopRecording method called?
        /// Reset to false with process onSuccess and onFail methods
        /// </summary>
        public bool wasStopRequested = false;

        /// <summary>
        /// Milliseconds.
        /// </summary>
        public int RecordingDuration = 45000;

        private void OnEnable()
        {
            
        }

        /// <summary>
        /// Setup process parameters
        /// </summary>
        void Start()
		{
			defaultProcessFileName = FFMPEG;

            outFilePath = Path.Combine(Application.streamingAssetsPath, OUTFILENAME);
		}

        /// <summary>
        /// Check if stopwatch has passed RecordingDuration and StopRecording if so
        /// </summary>
        private void Update()
        {
            if (isRecording && StartTime != null)
            {
                TimeSpan duration = (TimeSpan)(DateTime.Now - StartTime);

                if (duration.TotalMilliseconds > RecordingDuration && !wasStopRequested)
                {
                    StopRecording();
                }
            }
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
                // Reset so the standardInput commands are not immediately input
                standardInputEvent.Reset();

                // Send the settings and timeout over to be run
                Run(
                    new Settings(
                        ProcessFilePath,
                        string.Format(STARTRECORDINGTEMPLATE, deviceName, fileOut),
                        STOPRECORDINGTEMPLATE
                    ),
                    ProcessTimeout
                );
            }
        }

        /// <summary>
        /// Set the timer StartTime and invoke callback
        /// </summary>
        /// <param name="onFirstProcessStartedWrapper"></param>
        protected override void OnFirstProcessStarted()
        {
            StartTime = DateTime.Now;

            base.OnFirstProcessStarted();
        }

        /// <summary>
        /// Handle success and invoke callback
        /// </summary>
        /// <param name="onAllProcessFinishedWrapper"></param>
        /// <param name="exitCodes"></param>
        protected override void OnAllProcessSuccess(int[] exitCodes)
        {
            //Debug.Log("Finished recording after milliseconds: " + ((TimeSpan)(DateTime.Now - StartTime)).TotalMilliseconds);
            RLMGLogger.Instance.Log(
                "Finished recording after milliseconds: " + ((TimeSpan)(DateTime.Now - StartTime)).TotalMilliseconds,
                MESSAGETYPE.INFO
            );

            foreach (int exitCode in exitCodes)
            {
                //UnityEngine.Debug.Log("Exit code: " + exitCode);
                RLMGLogger.Instance.Log(
                    "Exit code: " + exitCode,
                    MESSAGETYPE.INFO
                );
            }

            base.OnAllProcessSuccess(exitCodes);

            //The first exit code should be 0 for a successful recording
            //A code like -22 means an error occurred
            if (exitCodes[0] == 0)
                onRecordingSuccess?.Invoke();
            else
                onRecordingError?.Invoke();
        }

        /// <summary>
        /// Reset wasStopRequested and invoke callback
        /// </summary>
        /// <param name="onSuccessWrapper"></param>
        protected override void OnSuccess()
        {
            wasStopRequested = false;

            StartTime = null;

            base.OnSuccess();
        }

        /// <summary>
        /// Reset wasStopRequested and invoke callback
        /// </summary>
        /// <param name="onFailWrapper"></param>
        /// <param name="e"></param>
        protected override void OnFail(Exception e)
        {
            wasStopRequested = false;

            StartTime = null;

            base.OnFail(e);
        }

        /// <summary>
        /// Signals a System.Threading.AutoResetEvent to let the external process thread continue
        /// and issue the STOPRECORDINGTEMPLATE command to the active process
        /// </summary>
		private void StopRecording()
        {
            RLMGLogger.Instance.Log(
                "Stop Recording invoked.",
                MESSAGETYPE.INFO
            );

            standardInputEvent.Set();

            wasStopRequested = true;

            onStopRequested?.Invoke();
        }

        /// <summary>
        /// Public method for starting the recording.
        /// </summary>
		public void OnStartRecording()
        {
			StartRecording(MicrophoneName, outFilePath);
        }

        /// <summary>
        /// Public method for stopping the recording.
        /// </summary>
        public void OnStopRecording()
        {
            StopRecording();
        }

        

        
	}

}

