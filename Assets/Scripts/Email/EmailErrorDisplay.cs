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
        private KeyboardController emailKeyboard;

        [SerializeField]
        private EmailSubmissionHandler emailSubmissionHandler;

        private MainSubmissionHandler mainSubmissionHandler;

        [SerializeField]
        private TMP_Text messageField;

        protected override void Awake()
        {
            base.Awake();

            mainSubmissionHandler = FindObjectOfType<MainSubmissionHandler>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (emailKeyboard != null)
                emailKeyboard.onInit += OnInit;

            if (emailSubmissionHandler != null)
            {
                emailSubmissionHandler.onValidationError += OnValidationError;
                emailSubmissionHandler.onSubmissionError += OnSubmissionError;
                emailSubmissionHandler.onSubmissionSuccess += DismissError;
            }

            if (mainSubmissionHandler != null)
            {
                mainSubmissionHandler.onEmailSubmissionError += OnSubmissionError;
                mainSubmissionHandler.onSubmissionSuccess += DismissError;
            }
                
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (emailKeyboard != null)
                emailKeyboard.onInit -= OnInit;

            if (emailSubmissionHandler != null)
            {
                emailSubmissionHandler.onValidationError -= OnValidationError;
                emailSubmissionHandler.onSubmissionError -= OnSubmissionError;
                emailSubmissionHandler.onSubmissionSuccess -= DismissError;
            }

            if (mainSubmissionHandler != null)
            {
                mainSubmissionHandler.onEmailSubmissionError -= OnSubmissionError;
                mainSubmissionHandler.onSubmissionSuccess -= DismissError;
            }

        }

        private void OnInit()
        {
            Close();
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


