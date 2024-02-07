using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PromptDisplay : BaseWindow
    {
        public delegate void PromptSelectEvent(string id);
        public PromptSelectEvent onPromptSelected;

        public string id = null;

        [SerializeField]
        private TMP_Text textDisplay;

        [SerializeField]
        private Button button;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (button != null)
                button.onClick.AddListener(OnClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (button != null)
                button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            onPromptSelected?.Invoke(id);
        }

        public void SetContent(string question)
        {
            id = question;

            if (textDisplay != null)
                textDisplay.text = AddNoBreakTagsToLastTwoWords(question);
        }

        private string AddNoBreakTagsToLastTwoWords(string text)
        {
            string[] words = text.Split();

            string partOneWords = System.String.Join(" ", words.SubArray(0, words.Length - 2));
            string partTwoWords = "<nobr>" + string.Join(" ", words.SubArray(words.Length - 2, 2)) + "</nobr>";

            return partOneWords + " " + partTwoWords;
        }
    }
}


