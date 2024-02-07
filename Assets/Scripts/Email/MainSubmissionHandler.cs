using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    public class MainSubmissionHandler : MonoBehaviour
    {
        private string DATETIMEFORMAT = "MMddyy_HHmmss";
        private string DEFAULTNAME = "york_audio";

        [SerializeField]
        private NameSubmissionHandler nameSubmissionHandler;

        [SerializeField]
        private EmailSubmissionHandler emailSubmissionHandler;

        [SerializeField]
        private SaverLogger saverLogger;

        [SerializeField]
        private Emailer emailer;

        public bool doSaveData = false;

        public bool doSendData = false;

        public bool doSubscribe = true;

        public string inputEmail;

        public string inputFirstName;

        public string inputLastName;

        public string savedFilePath;

        private string prompt = null;

        private AudioQuestionsPopulator audioQuestionsPopulator;

        public delegate void OnSubmissionErrorEvent(string message);
        public OnSubmissionErrorEvent onNameSubmissionError;
        public OnSubmissionErrorEvent onEmailSubmissionError;

        public delegate void OnSubmissionSuccessEvent();
        public OnSubmissionSuccessEvent onSubmissionSuccess;

        private void Awake()
        {
            audioQuestionsPopulator = FindObjectOfType<AudioQuestionsPopulator>();
        }

        private void OnEnable()
        {
            if (nameSubmissionHandler != null)
                nameSubmissionHandler.onValidationSuccess += OnNameValidationSucess;

            if (emailSubmissionHandler != null)
                emailSubmissionHandler.onValidationSuccess += OnEmailValidationSucess;

            if (emailer != null)
            {
                emailer.onFail += OnEmailSubmissionError;
                emailer.onSuccess += OnSubmissionSuccess;
            }

            if (audioQuestionsPopulator != null)
                audioQuestionsPopulator.OnPrompSelected += OnPromptSelected;
                
        }

        private void OnDisable()
        {
            if (nameSubmissionHandler != null)
                nameSubmissionHandler.onValidationSuccess -= OnNameValidationSucess;

            if (emailSubmissionHandler != null)
                emailSubmissionHandler.onValidationSuccess -= OnEmailValidationSucess;

            if (emailer != null)
            {
                emailer.onFail -= OnEmailSubmissionError;
                emailer.onSuccess -= OnSubmissionSuccess;
            }

            if (audioQuestionsPopulator != null)
                audioQuestionsPopulator.OnPrompSelected -= OnPromptSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Assumed to be valid</param>
        private void OnEmailValidationSucess(string input)
        {
            inputEmail = input;

            if (!doSaveData)
                Submit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Assumed to be valid</param>
        private void OnNameValidationSucess(string[] input)
        {
            inputFirstName = input[0];
            inputLastName = input[1];

            Submit();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Submit()
        {
            if (saverLogger == null) { return; }
            if (emailer == null) { return; }

            //We must save the file here because it will need to be sent via email
            //We will delete it after email is sent successfully
            if (doSaveData || doSendData)
            {
                string fileName = GetFileName();

                try
                {
                    savedFilePath = saverLogger.SaveFile(fileName);
                }
                catch (Exception e)
                {
                    RLMGLogger.Instance.Log(
                        e.ToString(),
                        MESSAGETYPE.ERROR
                    );

                    OnNameSubmissionError(e.ToString());
                    savedFilePath = null;
                }
            }
            else
            {
                savedFilePath = null;
            }

            // If we ARE saving the recording, log it as well.
            // And if we are NOT sending the recording, then we're done.
            if (doSaveData)
            {
                try
                {
                    saverLogger.LogFile(inputFirstName, inputLastName, inputEmail, Path.GetFileName(savedFilePath), doSubscribe);
                }
                catch (Exception e)
                {
                    RLMGLogger.Instance.Log(
                         e.ToString(),
                         MESSAGETYPE.ERROR
                     );

                    OnNameSubmissionError(e.ToString());
                }
                finally
                {
                    if (!doSendData)
                        OnSubmissionSuccess();
                }
            }

            // If we ARE sending the data, send it in a coroutine
            // 
            if (doSendData)
            {
                Emailer.EmailSettings settings = new Emailer.EmailSettings(
                    inputEmail, inputFirstName, inputLastName, savedFilePath, doSubscribe, !doSaveData, prompt
                );
                emailer.StartSendEmail(settings);
            }
        }

        private string GetFileName()
        {
            if (doSaveData)
            {
                return string.Format("{0}_story_{1}.mp3", GetNameFormatted(), System.DateTime.Now.ToString(DATETIMEFORMAT));
            }
            else
            {
                return string.Format("{0}_story_{1}.mp3", DEFAULTNAME, System.DateTime.Now.ToString(DATETIMEFORMAT));
            }
        }

        private string GetNameFormatted()
        {
            if (string.IsNullOrEmpty(inputFirstName))
            {
                if (string.IsNullOrEmpty(inputLastName))
                    return DEFAULTNAME;
                else
                    return inputLastName;
            }
            else
            {
                if (string.IsNullOrEmpty(inputLastName))
                    return inputFirstName.Substring(0, 1);
                else
                    return string.Format("{0}_{1}", inputFirstName.Substring(0, 1), inputLastName);
            }
        }

        public void OnSubmissionSuccess()
        {
            Debug.Log("Submission success");

            //Delete the file we used as an attachment
            if (!doSaveData && !string.IsNullOrEmpty(savedFilePath))
            {
                File.Delete(savedFilePath);
            }

            //todo show succcess message
            if (onSubmissionSuccess != null)
                onSubmissionSuccess();
        }

        public void OnNameSubmissionError(string message)
        {
            //the page manager makes sure we are on the name keyboard
            //the name error listens and displays

            if (onNameSubmissionError != null)
                onNameSubmissionError(message);
        }

        public void OnEmailSubmissionError(string message)
        {
            RLMGLogger.Instance.Log(
                message,
                MESSAGETYPE.ERROR
            );

            //the page manager needs to listen here
            //and return us to the email keyboard
            //the email error listens and displays

            if (onEmailSubmissionError != null)
                onEmailSubmissionError(message);
        }

        public void SetDoSaveData(bool value)
        {
            doSaveData = value;
        }

        public void SetDoSendData(bool value)
        {
            doSendData = value;
        }

        public void SetDoSubscribe(bool value)
        {
            doSubscribe = value;
        }

        private void OnPromptSelected(string p)
        {
            prompt = p;
        }
    }
}


