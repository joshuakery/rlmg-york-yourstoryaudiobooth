using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PromptTextDisplay : MonoBehaviour
    {
        private AudioQuestionsPopulator audioQuestionsPopulator;

        [SerializeField]
        private TMPro.TMP_Text textDisplay;

        private void Awake()
        {
            audioQuestionsPopulator = FindObjectOfType<AudioQuestionsPopulator>();
        }

        private void OnEnable()
        {
            if (audioQuestionsPopulator != null)
                audioQuestionsPopulator.OnPrompSelected += OnPromptSelected;
        }

        private void OnDisable()
        {
            if (audioQuestionsPopulator != null)
                audioQuestionsPopulator.OnPrompSelected -= OnPromptSelected;
        }

        private void OnPromptSelected(string p)
        {
            if (textDisplay != null)
                if (audioQuestionsPopulator != null)
                    textDisplay.text = audioQuestionsPopulator.CurrentPrompt;
        }
    }
}


