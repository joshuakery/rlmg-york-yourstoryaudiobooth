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
        public delegate void PromptSelectEvent(int id);
        public PromptSelectEvent onPromptSelected;

        public int id = -1;

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

        public void SetContent(AudioQuestion question)
        {
            id = question.id;

            if (textDisplay != null)
                textDisplay.text = question.text;
        }
    }
}


