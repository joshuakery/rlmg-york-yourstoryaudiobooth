using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using rlmg.utils;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class NameEntryPage : GenericPage
    {
        [SerializeField]
        private KeyboardController nameKeyboard;

        [SerializeField]
        private Toggle firstNameInputFieldToggle;

        protected override void _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (nameKeyboard != null)
                nameKeyboard.EnableKeyboard();

            if (firstNameInputFieldToggle != null)
                firstNameInputFieldToggle.isOn = true;

            base._Open(sequenceType, atPosition);
        }
    }
}


