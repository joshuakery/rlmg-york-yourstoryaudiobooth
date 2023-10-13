using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PromptSelectPage : GenericPage
    {
        [SerializeField]
        private BaseWindow samplesWindow;

        protected override void _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (samplesWindow != null)
                samplesWindow.Close(SequenceType.CompleteImmediately);

            base._Open(sequenceType, atPosition);
        }
    }
}


