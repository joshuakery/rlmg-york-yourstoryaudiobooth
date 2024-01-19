using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System;
using System.Text;
using rlmg.logging;

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
		private const string FFMPEG = "ffmpeg.exe";

        /// <summary>
        /// Template for ffmpeg for recording from an audio input device on Windows with overwrite
        /// </summary>
		private string DOTRIMTEMPLATE = "-ss {0} -t {1} -y -i \"{2}\" -acodec copy \"{3}\" ";

        public bool IsTrimming
        {
            get
            {
                return currentTask != null && !currentTask.IsCompleted;
            }
        }

        public delegate void TrimSuccessEvent();
        public TrimSuccessEvent onTrimSuccess;

        public delegate void TrimErrorEvent();
        public TrimErrorEvent onTrimError;
        



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
                Run(
                    new Settings(
                        ProcessFilePath,
                        string.Format(DOTRIMTEMPLATE, startTime, duration, fileIn, fileOut)
                    )
                );
            }
        }

        protected override void OnAllProcessSuccess(int[] exitCodes, StringBuilder outputStringBuilder)
        {
            RLMGLogger.Instance.Log(
                "All finished trim.",
                MESSAGETYPE.INFO
            );

            foreach (int exitCode in exitCodes)
            {
                RLMGLogger.Instance.Log(
                    "Exit code: " + exitCode,
                    MESSAGETYPE.INFO
                );
            }

            RLMGLogger.Instance.Log(
                "Process Output:\n - " + outputStringBuilder.ToString(),
                MESSAGETYPE.INFO
            );

            base.OnAllProcessSuccess(exitCodes, outputStringBuilder);

            if (exitCodes[0] == 0)
                onTrimSuccess?.Invoke();
            else
                onTrimError?.Invoke();
        }

        /// <summary>
        /// Public method for starting the recording.
        /// </summary>
		public void OnStartTrim(string startTime, string duration)
        {
            StartTrim(startTime, duration, inFilePath, outFilePath);
        }


    }
}


