using System.IO;
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

        void Awake()
        {
            pageManager = FindObjectOfType<PageManager>();

            recordedFilePath = Path.Combine(Application.streamingAssetsPath, RECORDEDFILENAME);
            trimmedFilePath = Path.Combine(Application.streamingAssetsPath, TRIMMEDFILENAME);
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
                File.Delete(recordedFilePath);
                File.Delete(trimmedFilePath);
            }
        }
    }
}


