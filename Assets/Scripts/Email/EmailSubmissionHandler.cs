using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rlmg.utils;

namespace JoshKery.York.AudioRecordingBooth
{
    public class EmailSubmissionHandler : MonoBehaviour
    {
        [SerializeField]
        private KeyboardController keyboardController;

        [SerializeField]
        private EmailValidator emailValidator;

        public delegate void OnValidationErrorEvent(int inputField, string message);
        public OnValidationErrorEvent onValidationError;

        public delegate void OnValidationSuccessEvent(string input);
        public OnValidationSuccessEvent onValidationSuccess;

        public delegate void OnSubmissionErrorEvent(string message);
        public OnSubmissionErrorEvent onSubmissionError;

        public delegate void OnSubmissionSuccessEvent();
        public OnSubmissionSuccessEvent onSubmissionSuccess;

        private void OnEnable()
        {
            if (keyboardController != null)
                keyboardController.onSubmit += ValidateOnSubmit;
        }

        private void OnDisable()
        {
            if (keyboardController != null)
                keyboardController.onSubmit -= ValidateOnSubmit;
        }

        private void ValidateOnSubmit(string[] input)
        {
            if (keyboardController == null) { return; }
            if (emailValidator == null) { return; }
            if (input == null || input.Length != 1) { return; }

            switch(emailValidator.Validate(input[0]))
            {
                case EmailValidator.ValidationResponse.Unknown:
                    OnValidationError(0, "An error occurred.");
                    break;
                case EmailValidator.ValidationResponse.Timeout:
                    OnValidationError(0, "This email address took too long to validate.");
                    break;
                case EmailValidator.ValidationResponse.Empty:
                    OnValidationError(0, "This email address is empty.");
                    break;
                case EmailValidator.ValidationResponse.InvalidFormat:
                    OnValidationError(0, "The format of this email address is invalid.");
                    break;
                case EmailValidator.ValidationResponse.TooLong:
                    OnValidationError(0, "This email address is too long.");
                    break;
                case EmailValidator.ValidationResponse.Success:
                    OnValidationSuccess(input[0]);
                    break;
            }  
        }

        public void OnValidationSuccess(string input)
        {
            Debug.Log("email validated");

            if (keyboardController != null)
                keyboardController.DisableKeyboard();

            if (onValidationSuccess != null)
                onValidationSuccess(input); // todo listener advances to next page
        }

        public void OnValidationError(int inputField, string message)
        {
            Debug.Log("email validation error");

            if (onValidationError != null)
                onValidationError(inputField, message);
        }

        public void OnSubmissionSuccess()
        {
            //todo show succcess message
            if (onSubmissionSuccess != null)
                onSubmissionSuccess();
        }

        public void OnSubmissionError(string message)
        {
            if (onSubmissionError != null)
                onSubmissionError(message);
        }
    }
}


