using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using rlmg.utils;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class EmailErrorDisplay : BaseWindow
    {
        [SerializeField]
        private EmailSubmissionHandler emailSubmissionHandler;

        [SerializeField]
        private TMP_Text messageField;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (emailSubmissionHandler != null)
            {
                emailSubmissionHandler.onValidationError += OnValidationError;
                emailSubmissionHandler.onSubmissionError += OnSubmissionError;
                emailSubmissionHandler.onSubmissionSuccess += DismissError;
            }
                
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (emailSubmissionHandler != null)
            {
                emailSubmissionHandler.onValidationError -= OnValidationError;
                emailSubmissionHandler.onSubmissionError -= OnSubmissionError;
                emailSubmissionHandler.onSubmissionSuccess -= DismissError;
            }
                
        }

        private void OnValidationError(int inputField, string message)
        {
            messageField.text = message;

            if (isOpen)
                //pulse animation
                return;
            else
                Open();
        }

        private void OnSubmissionError(string message)
        {
            messageField.text = message;

            if (isOpen)
                //pulse animation
                return;
            else
                Open();
        }

        private void DismissError()
        {
            Close();
        }
    }
}


