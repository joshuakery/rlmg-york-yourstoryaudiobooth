using JoshKery.GenericUI.DOTweenHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class ThankYouPage : GenericPage
    {
        [SerializeField]
        private GameObject noSaveThankYou;

        [SerializeField]
        private GameObject saveThankYou;

        [SerializeField]
        private MainSubmissionHandler mainSubmissionHandler;

        protected override void _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (mainSubmissionHandler != null)
            {
                if (saveThankYou != null)
                    saveThankYou.SetActive(mainSubmissionHandler.doSaveData);
                if (noSaveThankYou != null)
                    noSaveThankYou.SetActive(!mainSubmissionHandler.doSaveData);
            }

            base._Open(sequenceType, atPosition);
        }
    }
}


