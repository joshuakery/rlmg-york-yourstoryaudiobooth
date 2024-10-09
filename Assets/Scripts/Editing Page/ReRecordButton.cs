using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class ReRecordButton : MonoBehaviour
    {
        private PlaybackTrimSwitchManager switchManager;

        [SerializeField]
        private Button button;

        private void Awake()
        {
            switchManager = FindObjectOfType<PlaybackTrimSwitchManager>();
        }

        private void OnEnable()
        {
            if (switchManager != null)
            {
                switchManager.onSwitchStart += OnSwitchStart;
                switchManager.onSwitchComplete += OnSwitchComplete;
            }

        }

        private void OnDisable()
        {
            if (switchManager != null)
            {
                switchManager.onSwitchStart -= OnSwitchStart;
                switchManager.onSwitchComplete -= OnSwitchComplete;
            }
        }

        private void OnSwitchStart(bool isToPlayback)
        {
            if (button != null)
                button.interactable = false;
        }

        private void OnSwitchComplete(bool isPlayback)
        {
            if (button != null)
                button.interactable = isPlayback;
        }


    }
}


