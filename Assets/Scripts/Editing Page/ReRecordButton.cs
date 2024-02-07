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
                switchManager.onPlayback += OnSwitchToPlayback;
                switchManager.onTrim += OnSwitchToTrim;
            }

        }

        private void OnDisable()
        {
            if (switchManager != null)
            {
                switchManager.onPlayback -= OnSwitchToPlayback;
                switchManager.onTrim -= OnSwitchToTrim;
            }
        }

        private void OnSwitchToPlayback()
        {
            if (button != null)
                button.interactable = true;
        }

        private void OnSwitchToTrim()
        {
            if (button != null)
                button.interactable = false;
        }


    }
}


