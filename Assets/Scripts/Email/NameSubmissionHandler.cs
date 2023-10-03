using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rlmg.utils;

namespace JoshKery.York.AudioRecordingBooth
{
    public class NameSubmissionHandler : MonoBehaviour
    {
        [SerializeField]
        private KeyboardController keyboardController;

        [SerializeField]
        private NameValidator nameValidator;

        public delegate void OnValidationErrorEvent(string message);
        public OnValidationErrorEvent onValidationError;

        public delegate void OnValidationSuccessEvent(string[] input);
        public OnValidationSuccessEvent onValidationSuccess;

        public delegate void OnSubmissionErrorEvent(string message);
        public OnSubmissionErrorEvent onSubmissionError;

        public delegate void OnSubmissionSuccessEvent();
        public OnSubmissionSuccessEvent onSubmissionSuccess;

        private void OnEnable()
        {
            if (keyboardController != null)
            {
                keyboardController.onSubmit += ValidateOnSubmit;
            }
        }

        private void OnDisable()
        {
            if (keyboardController != null)
            {
                keyboardController.onSubmit -= ValidateOnSubmit;
            }
        }

        private void ValidateOnSubmit(string[] input)
        {
            if (keyboardController == null) { return; }
            if (nameValidator == null) { return; }
            if (input == null || input.Length != 2) { return; }

            //todo
            //we just want to reject names that are too long
            //this is arbitrary, as the actual limit is really high, but some limit must be imposed

            bool fieldWasInvalid = false;
            for (int i=0; i<input.Length; i++)
            {
                switch (nameValidator.Validate(input[i]))
                {
                    case NameValidator.ValidationResponse.Unknown:
                    case NameValidator.ValidationResponse.Timeout:
                    case NameValidator.ValidationResponse.Empty:
                    case NameValidator.ValidationResponse.InvalidFormat:
                        fieldWasInvalid = true;
                        OnValidationError(i, "An error occurred.");
                        break;
                    case NameValidator.ValidationResponse.TooLong:
                        fieldWasInvalid = true;
                        OnValidationError(i, "This name is too long.");
                        break;
                    case NameValidator.ValidationResponse.Success:
                        break;
                }
            }

            if (!fieldWasInvalid)
                OnValidationSuccess(input);
        }

        public void OnValidationSuccess(string[] input)
        {
            Debug.Log("name validated");

            if (keyboardController != null)
                keyboardController.DisableKeyboard();

            onValidationSuccess(input); // todo listener advances to next page
        }

        public void OnValidationError(int inputField, string message)
        {
            Debug.Log("name error");

            onValidationError(message);
        }

        public void OnSubmissionSuccess()
        {
            //todo show succcess message
            onSubmissionSuccess();
        }

        public void OnSubmissionError(string message)
        {
            onSubmissionError(message);
        }
    }
}


