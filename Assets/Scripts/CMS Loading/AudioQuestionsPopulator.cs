using JoshKery.GenericUI.DOTweenHelpers;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class AudioQuestionsPopulator : BaseWindow
    {
        [SerializeField]
        private AudioQuestionsLoader contentLoader;

        private AudioQuestionsWrapper audioQuestionsWrapper;
        public AudioQuestionsWrapper AudioQuestionsWrapper
        {
            get { return audioQuestionsWrapper; }
            set
            {
                audioQuestionsWrapper = value;

                if (audioQuestionsWrapper.AudioQuestions != null)
                    audioQuestionsDict = audioQuestionsWrapper.AudioQuestions.ToDictionary(q => q.id);
            }
        }

        private Dictionary<int, AudioQuestion> audioQuestionsDict;

        public int SelectedPrompt = -1;

        public string CurrentPrompt
        {
            get
            {
                if (AudioQuestionsWrapper?.AudioQuestions != null &&
                    AudioQuestionsWrapper.AudioQuestions.Count > 0 &&
                    audioQuestionsDict != null &&
                    audioQuestionsDict.ContainsKey(SelectedPrompt))
                {
                    return audioQuestionsDict[SelectedPrompt].text;
                }
                else
                    return null;
            }
        }

        public delegate void PromptSelectedEvent();
        public PromptSelectedEvent OnPrompSelected;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (contentLoader != null)
            {
                contentLoader.onPopulateContent += PopulateContent;
                contentLoader.onPopulateContentFinish.AddListener(OnPopulateContentFinish);
            }   
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (contentLoader != null)
            {
                contentLoader.onPopulateContent -= PopulateContent;
                contentLoader.onPopulateContentFinish.RemoveListener(OnPopulateContentFinish);
            }
        }

        private IEnumerator PopulateContent(string text)
        {
            yield return null;

            AudioQuestionsWrapper = JsonConvert.DeserializeObject<AudioQuestionsWrapper>(text);

            SetPrompts();
        }

        private void SetPrompts()
        {
            if (AudioQuestionsWrapper == null || AudioQuestionsWrapper.AudioQuestions.Count == 0) { return; }

            ClearAllDisplays();

            foreach (AudioQuestion question in AudioQuestionsWrapper.AudioQuestions)
            {
                PromptDisplay promptDisplay = InstantiateDisplay<PromptDisplay>();

                if (promptDisplay != null)
                {
                    promptDisplay.SetContent(question);
                    promptDisplay.onPromptSelected += OnPromptSelected;
                }
            }
        }

        private void OnPromptSelected(int id)
        {
            SelectedPrompt = id;

            OnPrompSelected?.Invoke();
        }

        private void OnPopulateContentFinish()
        {
            // todo put animations in start states
        }
    }
}


