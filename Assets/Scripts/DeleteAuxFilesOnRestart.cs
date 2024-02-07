using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class DeleteAuxFilesOnRestart : MonoBehaviour
    {
        [SerializeField]
        private string RECORDEDFILENAME = "out.mp3";

        private string recordedFilePath;

        [SerializeField]
        private string TRIMMEDFILENAME = "out_trimmed.mp3";

        private string trimmedFilePath;

        private PageManager pageManager;

        private AudioRecorderProcess recordingProcess;

        private AudioTrimProcess trimProcess;

        private WaitForSeconds waitOneSecond;
        private WaitForSeconds waitForSeconds;

        void Awake()
        {
            pageManager = FindObjectOfType<PageManager>();

            recordingProcess = FindObjectOfType<AudioRecorderProcess>();
            trimProcess = FindObjectOfType<AudioTrimProcess>();

            recordedFilePath = Path.Combine(Application.streamingAssetsPath, RECORDEDFILENAME);
            trimmedFilePath = Path.Combine(Application.streamingAssetsPath, TRIMMEDFILENAME);

            waitOneSecond = new WaitForSeconds(1);
            waitForSeconds = new WaitForSeconds(60);
        }

        private void OnEnable()
        {
            if (pageManager != null)
                pageManager.onNewPage += OnNewPage;
        }

        private void OnDisable()
        {
            if (pageManager != null)
                pageManager.onNewPage -= OnNewPage;
        }

        private void OnNewPage(PageManager.Page page)
        {
            if (page == PageManager.Page.PromptSelection)
            {
                StopAllCoroutines();
                StartCoroutine(Timeout());
                StartCoroutine(WaitAndDelete());
            }
        }

        private IEnumerator WaitAndDelete()
        {
            if (recordingProcess != null && trimProcess != null)
            {
                while (recordingProcess.isRunning &&
                    recordingProcess.isRecording &&
                    trimProcess.isRunning)
                    yield return null;

                yield return waitOneSecond;

                try
                {
                    File.Delete(recordedFilePath);
                    File.Delete(trimmedFilePath);
                }
                catch (IOException e)
                {
                    Debug.LogError("Error while deleting file: " + e.ToString());
                }
                catch (Exception e)
                {
                    Debug.LogError("Unkown Error while deleting file: " + e.ToString());
                }
            }

            // Kill timeout
            StopAllCoroutines();
        }

        private IEnumerator Timeout()
        {
            yield return waitForSeconds;

            StopAllCoroutines();
        }
    }
}


