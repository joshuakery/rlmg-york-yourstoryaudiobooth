using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TestProcess : ExternalProcess
    {
        /// <summary>
        /// Executable file for external processes.
        /// </summary>
        private const string FFMPEG = "ffmpeg.exe";

        /// <summary>
        /// Template for ffmpeg for recording from an audio input device on Windows with overwrite
        /// </summary>
        private string TESTTEMPLATE = "-f dshow -y -i audio=\"Microphone Array (AMD Audio Device)\" \"test.mp3\"";

        private string STOPTEMPLATE = "q";

        /// <summary>
        /// Setup process parameters
        /// </summary>
        void Start()
        {
            defaultProcessFileName = FFMPEG;
        }

        /// <summary>
        /// ...
        /// </summary>
        private void Test()
        {
            if (currentTask == null || currentTask.IsCompleted)
            {
                // Reset so the standardInput commands are not immediately input
                standardInputEvent.Reset();

                // Send the settings and timeout over to be run
                Run(
                    new Settings(
                        ProcessFilePath,
                        TESTTEMPLATE,
                        STOPTEMPLATE
                    )
                );
            }
        }

        /// <summary>
        /// ...
        /// </summary>
        private void StopTest()
        {
            standardInputEvent.Set();
        }

        /// <summary>
        /// Handle success and invoke callback
        /// </summary>
        /// <param name="onAllProcessFinishedWrapper"></param>
        /// <param name="exitCodes"></param>
        protected override void OnAllProcessSuccess(int[] exitCodes, StringBuilder outputStringBuilder)
        {
            foreach (int exitCode in exitCodes)
            {
                //UnityEngine.Debug.Log("Exit code: " + exitCode);
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
        }

        public void OnRunTest()
        {
            Test();
        }

        public void OnStopTest()
        {
            StopTest();
        }
    }
}


