using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class CancelButton : MonoBehaviour
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
            if (button != null)
                button.onClick.AddListener(OnClick);

            if (switchManager != null)
            {
                switchManager.onPlayback += OnSwitchToPlayback;
                switchManager.onTrim += OnSwitchToTrim;
            }

        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClick);

            if (switchManager != null)
            {
                switchManager.onPlayback -= OnSwitchToPlayback;
                switchManager.onTrim -= OnSwitchToTrim;
            }
        }

        private void OnClick()
        {
            if (switchManager != null)
                switchManager.Switch(true);
        }

        private void OnSwitchToPlayback()
        {
            if (button != null)
                button.interactable = false;
        }

        private void OnSwitchToTrim()
        {
            if (button != null)
                button.interactable = true;
        }


    }
}


