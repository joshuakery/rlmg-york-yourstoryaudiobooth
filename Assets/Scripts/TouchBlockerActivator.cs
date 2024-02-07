using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TouchBlockerActivator : MonoBehaviour
    {
        private PageManager pageManager;

        private AudioRecorderProcess recordingProcess;

        [SerializeField]
        private GameObject blocker;

        private WaitForSeconds longWait;
        private WaitForSeconds shortWait;

        private void Awake()
        {
            pageManager = FindObjectOfType<PageManager>();

            recordingProcess = FindObjectOfType<AudioRecorderProcess>();

            longWait = new WaitForSeconds(1.1f);
            shortWait = new WaitForSeconds(0.5f);
        }

        private void OnEnable()
        {
            if (pageManager != null)
                pageManager.onNewPage += OnNewPage;

            if (recordingProcess != null)
            {
                recordingProcess.onStartRequested += OnRecordingStatusChange;
                recordingProcess.onStopRequested += OnRecordingStatusChange;
            }
                
        }

        private void OnDisable()
        {
            if (pageManager != null)
                pageManager.onNewPage -= OnNewPage;

            if (recordingProcess != null)
            {
                recordingProcess.onStartRequested -= OnRecordingStatusChange;
                recordingProcess.onStopRequested -= OnRecordingStatusChange;
            }
        }

        private void Start()
        {
            if (blocker != null)
                blocker.SetActive(false);
        }

        private void OnNewPage(PageManager.Page page)
        {
            if (blocker != null)
            {
                blocker.SetActive(true);
                StopAllCoroutines();

                switch (page)
                {
                    case PageManager.Page.PromptSelection:
                    case PageManager.Page.Recording:
                    case PageManager.Page.Editing:
                        StartCoroutine(SetBlockerActive(longWait));
                        break;
                    default:
                        StartCoroutine(SetBlockerActive(shortWait));
                        break;
                }
                
                
            }
        }

        private void OnRecordingStatusChange()
        {
            if (blocker != null)
            {
                blocker.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(SetBlockerActive(longWait));
            }
        }

        private IEnumerator SetBlockerActive(WaitForSeconds waitForSeconds)
        {
            yield return waitForSeconds;

            if (blocker != null)
                blocker.SetActive(false);
        }
    }
}


