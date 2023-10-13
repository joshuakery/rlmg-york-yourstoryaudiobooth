using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class WaitingWindow : BaseWindow
    {
        private Emailer emailer;

        protected override void Awake()
        {
            base.Awake();

            emailer = FindObjectOfType<Emailer>();
        }
        private void Update()
        {
            if (emailer == null) { return; }

            if (emailer.currentTask != null &&
                !emailer.currentTask.IsCompleted
                )
            {
                if (!isOpen)
                    Open();
            }
            else
            {
                if (isOpen)
                    Close();
            }
        }
    }
}


