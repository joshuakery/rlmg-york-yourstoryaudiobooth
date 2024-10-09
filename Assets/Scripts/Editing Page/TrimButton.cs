using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TrimButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
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


        [SerializeField]
        private Sprite onPressedSprite;

        [SerializeField]
        private Sprite offPressedSprite;

        private void OnEnable()
        {
            if (button != null)
                button.onClick.AddListener(OnClick);

            if (switchManager != null)
            {
                switchManager.onCancel += OnCancelTrim;
                switchManager.onSwitchStart += OnSwitchStart;
                switchManager.onSwitchComplete += OnSwitchComplete;
            }
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClick);

            if (switchManager != null)
            {
                switchManager.onCancel -= OnCancelTrim;
                switchManager.onSwitchComplete -= OnSwitchComplete;
            }
        }

        private void OnClick()
        {
            if (switchManager != null)
                switchManager.OnSwitch();
        }

        private void OnCancelTrim()
        {
            
        }

        private void OnSwitchStart(bool isToPlayback)
        {
            if (button != null)
                button.interactable = false;
        }

        private void OnSwitchComplete(bool isPlayback)
        {
            if (button != null)
                button.interactable = true;

            if (image != null)
                image.sprite = isPlayback ? offSprite : onSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (image != null)
            {
                if (image.sprite == onSprite)
                    image.sprite = onPressedSprite;
                else if (image.sprite == offSprite)
                    image.sprite = offPressedSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (image != null)
            {
                if (image.sprite == onPressedSprite)
                    image.sprite = onSprite;
                else if (image.sprite == offPressedSprite)
                    image.sprite = offSprite;
            }
        }
    }
}


