using System.Collections;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    /// <summary>
    /// Listens to certain events from certain key components
    /// And opens GenericPages based on the event
    /// </summary>
    public class PageManager : MonoBehaviour
    {
        public enum Page
        {
            None = -2,
            Error = -1,
            PromptSelection = 0,
            Recording = 1,
            Editing = 2,
            AgeSelection = 3,
            EmailEntry = 4,
            NameEntry = 5,
            ThankYou = 6
        }

        private AudioQuestionsLoader contentLoader;

        [SerializeField]
        private AudioQuestionsPopulator audioQuestionsPopulator;

        private AudioRecorderProcess audioRecorderProcess;
        private AudioTrimProcess audioTrimProcess;

        [SerializeField]
        private Button reRecordButton;

        [SerializeField]
        private BaseWindow recordingEditingFooterWindow;

        [SerializeField]
        private Button[] startOverButtons;

        [SerializeField]
        private Button doneEditingButton;

        private BaseWindow doneEditingButtonWindow;

        [SerializeField]
        private BaseWindow ageEmailNameContainerWindow;

        [SerializeField]
        private Button closeAgeEmailNameContainerButton;

        private BaseWindow closeAgeEmailNameContainerButtonWindow;

        [SerializeField]
        private Button[] ageSelectButtons;

        private MainSubmissionHandler mainSubmissionHandler;

        private EmailSubmissionHandler emailSubmissionHandler;

        [SerializeField]
        private Button skipEmailButton;

        [SerializeField]
        private Button backFromEmailButton;

        [SerializeField]
        private Button backFromNameButton;

        private AttractLoop attractLoop;

        public Page CurrentPage = Page.None;

        public delegate void OnNewPageEvent(Page page);
        public OnNewPageEvent onNewPage;

        private void Awake()
        {
            contentLoader = FindObjectOfType<AudioQuestionsLoader>();

            audioRecorderProcess = FindObjectOfType<AudioRecorderProcess>();
            audioTrimProcess = FindObjectOfType<AudioTrimProcess>();

            doneEditingButtonWindow = doneEditingButton.gameObject.GetComponent<BaseWindow>();

            mainSubmissionHandler = FindObjectOfType<MainSubmissionHandler>();

            emailSubmissionHandler = FindObjectOfType<EmailSubmissionHandler>();

            closeAgeEmailNameContainerButtonWindow = closeAgeEmailNameContainerButton.gameObject.GetComponent<BaseWindow>();

            attractLoop = FindObjectOfType<AttractLoop>();
        }

        private void OnEnable()
        {
            if (contentLoader != null)
                contentLoader.onPopulateContentFinish.AddListener(OpenPromptSelectionPage);

            if (startOverButtons != null)
                foreach (Button startOverButton in startOverButtons)
                    startOverButton.onClick.AddListener(OpenPromptSelectionPage);

            // Prompt Selection Page
            if (audioQuestionsPopulator != null)
                audioQuestionsPopulator.OnPrompSelected += OpenRecordingPage;


            //Recording Page
            if (audioRecorderProcess != null)
                audioRecorderProcess.onRecordingSuccess += OnRecordingSuccess;


            //Editing Page
            if (reRecordButton != null)
                reRecordButton.onClick.AddListener(OpenRecordingPage);

            if (audioTrimProcess != null)
                audioTrimProcess.onTrimSuccess += OnTrimSucess;


            //Age Email Name Pages
            if (mainSubmissionHandler != null)
            {
                mainSubmissionHandler.onSubmissionSuccess += OnMainSubmissionSucess;
                mainSubmissionHandler.onEmailSubmissionError += OnMainEmailSubmissionError;
                mainSubmissionHandler.onNameSubmissionError += OnMainNameSubmissionError;
            }

            if (closeAgeEmailNameContainerButton != null)
                closeAgeEmailNameContainerButton.onClick.AddListener(OnCloseAgeEmailNamingContainer);


            //Age Select Page
            if (ageSelectButtons != null)
                foreach (Button button in ageSelectButtons)
                    button.onClick.AddListener(OnAgeSelected);


            //Email Entry page
            if (emailSubmissionHandler != null)
                emailSubmissionHandler.onValidationSuccess += OnEmailValidationSuccess;

            if (skipEmailButton != null)
                skipEmailButton.onClick.AddListener(OnSkipEmailEntry);

            if (backFromEmailButton != null)
                backFromEmailButton.onClick.AddListener(OnBackFromEmailEntry);


            //Name Entry Page
            if (backFromNameButton != null)
                backFromNameButton.onClick.AddListener(OnBackFromNameEntry);

            // Attract
            if (attractLoop != null)
                attractLoop.onBlackFadeInComplete.AddListener(OpenPromptSelectionPage);
        }

        private void OnDisable()
        {
            if (contentLoader != null)
                contentLoader.onPopulateContentFinish.RemoveListener(OpenPromptSelectionPage);

            if (startOverButtons != null)
                foreach (Button startOverButton in startOverButtons)
                    startOverButton.onClick.RemoveListener(OpenPromptSelectionPage);


            // Prompt Selection Page
            if (audioQuestionsPopulator != null)
                audioQuestionsPopulator.OnPrompSelected -= OpenRecordingPage;


            // Recording Page
            if (audioRecorderProcess != null)
                audioRecorderProcess.onRecordingSuccess -= OnRecordingSuccess;


            // Editing Page
            if (reRecordButton != null)
                reRecordButton.onClick.RemoveListener(OpenRecordingPage);

            if (audioTrimProcess != null)
                audioTrimProcess.onTrimSuccess -= OnTrimSucess;

            // Age Email Name Pages
            if (closeAgeEmailNameContainerButton != null)
                closeAgeEmailNameContainerButton.onClick.RemoveListener(OnCloseAgeEmailNamingContainer);

            if (mainSubmissionHandler != null)
            {
                mainSubmissionHandler.onSubmissionSuccess -= OnMainSubmissionSucess;
                mainSubmissionHandler.onEmailSubmissionError -= OnMainEmailSubmissionError;
                mainSubmissionHandler.onNameSubmissionError -= OnMainNameSubmissionError;
            }


            //Age Select Page
            if (ageSelectButtons != null)
                foreach (Button button in ageSelectButtons)
                    button.onClick.RemoveListener(OnAgeSelected);

            // Email Entry Page
            if (emailSubmissionHandler != null)
                emailSubmissionHandler.onValidationSuccess -= OnEmailValidationSuccess;

            if (skipEmailButton != null)
                skipEmailButton.onClick.RemoveListener(OnSkipEmailEntry);

            if (backFromEmailButton != null)
                backFromEmailButton.onClick.RemoveListener(OnBackFromEmailEntry);


            // Name Entry Page
            if (backFromNameButton != null)
                backFromNameButton.onClick.RemoveListener(OnBackFromNameEntry);

            // Attract
            if (attractLoop != null)
                attractLoop.onBlackFadeInComplete.RemoveListener(OpenPromptSelectionPage);
        }

        private void OpenPromptSelectionPage()
        {
            OpenPage(Page.PromptSelection);
        }

        private void OpenRecordingPage()
        {
            OpenPage(Page.Recording);
        }

        private void OnRecordingSuccess()
        {
            //OpenPage(Page.Editing);
            StartCoroutine(OnRecordingSucessCo());
        }

        private IEnumerator OnRecordingSucessCo()
        {
            yield return new WaitForSeconds(1.2f);
            OpenPage(Page.Editing);
        }

        private void OnTrimSucess()
        {
            OpenPage(Page.AgeSelection);
        }

        private void OnCloseAgeEmailNamingContainer()
        {
            OpenPage(Page.Editing);
        }

        private void OnAgeSelected()
        {
            OpenPage(Page.EmailEntry);
        }

        private void OnEmailValidationSuccess(string input)
        {
            if (mainSubmissionHandler == null) { return; }

            if (mainSubmissionHandler.doSaveData)
                OpenPage(Page.NameEntry);
        }

        private void OnMainSubmissionSucess()
        {
            OpenPage(Page.ThankYou);
        }

        private void OnMainEmailSubmissionError(string message)
        {
            OpenPage(Page.EmailEntry);
        }

        private void OnMainNameSubmissionError(string message)
        {
            OpenPage(Page.NameEntry);
        }

        private void OnSkipEmailEntry()
        {
            OpenPage(Page.NameEntry);
        }

        private void OnBackFromEmailEntry()
        {
            OpenPage(Page.AgeSelection);
        }

        private void OnBackFromNameEntry()
        {
            OpenPage(Page.EmailEntry);
        }

        /// <summary>
        /// Opens or closes some supporting elements, conditionally,
        /// in addition to the corresponding PageWindow's Open or Close method being called
        /// </summary>
        /// <param name="page"></param>
        public void OpenPage(Page page)
        {
            bool isRecordingOrEditing = (page == Page.Recording || page == Page.Editing);
            bool isAgeEmailOrName = (page == Page.AgeSelection || page == Page.EmailEntry || page == Page.NameEntry);

            if (recordingEditingFooterWindow != null)
                recordingEditingFooterWindow.OpenIfTrueElseClose(isRecordingOrEditing);

            if (doneEditingButtonWindow != null)
                doneEditingButtonWindow.OpenIfTrueElseClose(page == Page.Editing);

            if (ageEmailNameContainerWindow != null)
                ageEmailNameContainerWindow.OpenIfTrueElseClose(isAgeEmailOrName || page == Page.ThankYou);

            if (closeAgeEmailNameContainerButtonWindow != null)
                closeAgeEmailNameContainerButtonWindow.OpenIfTrueElseClose(isAgeEmailOrName);

            CurrentPage = page;
            onNewPage(page);
        }
    }
}


