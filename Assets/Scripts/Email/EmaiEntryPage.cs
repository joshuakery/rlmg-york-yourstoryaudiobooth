using JoshKery.GenericUI.DOTweenHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rlmg.utils;

namespace JoshKery.York.AudioRecordingBooth
{
    public class EmaiEntryPage : GenericPage
    {
        private MainSubmissionHandler mainSubmissionHandler;

        [SerializeField]
        private KeyboardController emailKeyboard;

        private EmailErrorDisplay errorDisplay;

        [SerializeField]
        private GameObject doSaveSubmitContainer;

        [SerializeField]
        private GameObject doNotSaveSubmitContainer;

        [SerializeField]
        private UnityEngine.UI.Toggle subscribeToggle;

        protected override void Awake()
        {
            base.Awake();

            mainSubmissionHandler = FindObjectOfType<MainSubmissionHandler>();

            errorDisplay = FindObjectOfType<EmailErrorDisplay>();
        }

        protected override void _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (errorDisplay != null)
                errorDisplay.Close(SequenceType.CompleteImmediately);

            if (emailKeyboard != null)
                emailKeyboard.EnableKeyboard();

            if (mainSubmissionHandler != null)
            {
                //By default set to true so that the keyboard submit logic needn't have this added
                mainSubmissionHandler.doSendData = true;
                
                //Show the correct Submit & Skip options
                if (doSaveSubmitContainer != null)
                    doSaveSubmitContainer.SetActive(mainSubmissionHandler.doSaveData);

                if (doNotSaveSubmitContainer != null)
                    doNotSaveSubmitContainer.SetActive(!mainSubmissionHandler.doSaveData);
            }

            if (subscribeToggle != null)
                subscribeToggle.isOn = true;

            base._Open(sequenceType, atPosition);
        }
    }
}


