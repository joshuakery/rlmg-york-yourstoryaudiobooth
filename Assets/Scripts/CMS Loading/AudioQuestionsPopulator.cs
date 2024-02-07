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

        public GraphQLResponseWrapper ResponseWrapper;

        public int SelectedPrompt = -1;

        public string CurrentPrompt
        {
            get
            {
                if (ResponseWrapper?.data?.audioQuestions != null)
                    return ResponseWrapper.data.audioQuestions.GetPrompt(SelectedPrompt);
                else
                    return null;
            }
        }

        public delegate void PromptSelectedEvent(string text);
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

            ResponseWrapper = JsonConvert.DeserializeObject<GraphQLResponseWrapper>(text);

            SetPrompts();
        }

        private void SetPrompts()
        {
            if (ResponseWrapper?.data?.audioQuestions == null) { return; }

            ClearAllDisplays();

            for (int i=0; i<6; i++)
            {
                PromptDisplay promptDisplay = InstantiateDisplay<PromptDisplay>();

                if (promptDisplay != null)
                {
                    promptDisplay.SetContent(ResponseWrapper.data.audioQuestions.GetPrompt(i));
                    promptDisplay.onPromptSelected += OnPromptSelected;
                }
            }
        }

        private void OnPromptSelected(string id)
        {
            if (ResponseWrapper?.data?.audioQuestions != null)
                SelectedPrompt = ResponseWrapper.data.audioQuestions.GetIndex(id);
            else
                SelectedPrompt = -1;

            OnPrompSelected?.Invoke(id);
        }

        private void OnPopulateContentFinish()
        {
            // todo put animations in start states
        }
    }
}


