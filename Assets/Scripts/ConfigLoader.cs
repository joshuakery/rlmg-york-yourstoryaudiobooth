using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JoshKery.GenericUI.ContentLoading;
using UnityEngine.Networking;
using Newtonsoft.Json;


namespace JoshKery.York.AudioRecordingBooth
{
    public class ConfigLoader : ContentLoader
    {
        [System.Serializable]
        public class ConfigData
        {
            /// <summary>
            /// Measured in seconds, but will be converted to milliseconds
            /// </summary>
            public int attractTimeout = 240;

            public string attractFile;

            public string apiServer;

            public string apiKey;

            public string apiEndpoint;

            public string sampleStoryFile1;
            public string sampleStoryFile2;

            public string microphoneName;

            /// <summary>
            /// Measured in seconds, but will be converted to milliseconds
            /// </summary>
            public int recordingDuration = 45;

            /// <summary>
            /// Measured in seconds, but will be converted to milliseconds
            /// </summary>
            public int checkForMicrophoneInterval = 30;

            /// <summary>
            /// Log Level to be brought over to ffmpeg processes
            /// </summary>
            public string loglevel = "error";
        }

        private AttractScreen attractScreen;
        private AttractLoop attractLoop;

        private AudioQuestionsLoader contentLoader;

        [SerializeField]
        private AudioClipLoader sampleStoryLoader1;

        [SerializeField]
        private AudioClipLoader sampleStoryLoader2;

        private AudioRecorderProcess audioRecordingProcess;

        private DeviceMonitor deviceMonitor;

        protected override IEnumerator LoadLocalContentSuccess(string text)
        {
            ConfigData configData = JsonConvert.DeserializeObject<ConfigData>(text);

            if (configData == null)
                yield break;

            if (attractLoop == null)
                attractLoop = FindObjectOfType<AttractLoop>();

            if (attractLoop != null && !string.IsNullOrEmpty(configData.attractFile))
                attractLoop.attractVideoPath = Path.Combine(LocalContentDirectory, configData.attractFile);

            if (attractScreen == null)
                attractScreen = FindObjectOfType<AttractScreen>();

            if (attractScreen != null)
                attractScreen.timeToActivate = configData.attractTimeout;

            if (contentLoader == null)
                contentLoader = FindObjectOfType<AudioQuestionsLoader>();

            if (contentLoader != null)
            {
                //todo set url and query, etc. for graphql request

                contentLoader.LoadContent();
            }

            if (sampleStoryLoader1 != null)
                StartCoroutine(sampleStoryLoader1.ReloadCo(configData.sampleStoryFile1));

            if (sampleStoryLoader2 != null)
                StartCoroutine(sampleStoryLoader2.ReloadCo(configData.sampleStoryFile2));

            if (audioRecordingProcess == null)
                audioRecordingProcess = FindObjectOfType<AudioRecorderProcess>();

            if (audioRecordingProcess != null)
            {
                audioRecordingProcess.MicrophoneName = configData.microphoneName;
                audioRecordingProcess.RecordingDuration = configData.recordingDuration * 1000;
                audioRecordingProcess.Loglevel = configData.loglevel;
            }

            if (deviceMonitor == null)
                deviceMonitor = FindObjectOfType<DeviceMonitor>();

            if (deviceMonitor != null)
            {
                deviceMonitor.MicrophoneName = configData.microphoneName;
                deviceMonitor.Timeout = configData.checkForMicrophoneInterval * 1000;
            }
                
        }

        protected override IEnumerator LoadLocalContentFinally(UnityWebRequest.Result result)
        {
            yield return null;
        }
    }
}


