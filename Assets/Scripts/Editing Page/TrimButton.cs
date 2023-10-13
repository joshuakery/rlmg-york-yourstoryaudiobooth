using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TrimButton : MonoBehaviour
    {
        [SerializeField]
        private PlaybackTrimSwitchManager switchManager;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite onSprite;

        [SerializeField]
        private Sprite offSprite;

        private void OnEnable()
        {
            if (button != null)
                button.onClick.AddListener(OnClick);

            if (switchManager != null)
            {
                switchManager.onTrim += OnSwitchToTrim;
                switchManager.onPlayback += OnSwitchToPlayback;
            }
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClick);

            if (switchManager != null)
            {
                switchManager.onTrim -= OnSwitchToTrim;
                switchManager.onPlayback -= OnSwitchToPlayback;
            }
        }

        private void OnClick()
        {
            if (switchManager != null)
                switchManager.OnSwitch();
        }

        private void OnSwitchToTrim()
        {
            if (image != null)
                image.sprite = onSprite;
        }

        private void OnSwitchToPlayback()
        {
            if (image != null)
                image.sprite = offSprite;
        }
    }
}


