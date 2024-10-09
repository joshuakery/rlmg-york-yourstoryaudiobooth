using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PlayPauseButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
    {
        [SerializeField]
        private PlaybackUIManager playbackManager;

        private PlaybackTrimSwitchManager switchManager;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite playSprite;

        [SerializeField]
        private Sprite pauseSprite;

        [SerializeField]
        private Sprite playPressedSprite;

        [SerializeField]
        private Sprite pausePressedSprite;

        private void Awake()
        {
            switchManager = FindObjectOfType<PlaybackTrimSwitchManager>();
        }

        private void OnEnable()
        {
            if (playbackManager != null)
            {
                if (button != null)
                    button.onClick.AddListener(playbackManager.OnPlayPauseAudio);

                playbackManager.onPlay += OnPlay;
                playbackManager.onPause += OnPause;
            }

            if (switchManager != null)
            {
                switchManager.onSwitchStart += OnSwitchStart;
                switchManager.onSwitchComplete += OnSwitchComplete;
            }
                
        }

        private void OnDisable()
        {
            if (playbackManager != null)
            {
                if (button != null)
                    button.onClick.RemoveListener(playbackManager.OnPlayPauseAudio);

                playbackManager.onPlay -= OnPlay;
                playbackManager.onPause -= OnPause;
            }

            if (switchManager != null)
            {
                switchManager.onSwitchStart -= OnSwitchStart;
                switchManager.onSwitchComplete -= OnSwitchComplete;
            }
        }

        private void OnPlay()
        {
            if (image != null)
                image.sprite = playSprite;
        }

        private void OnPause()
        {
            if (image != null)
                image.sprite = pauseSprite;
        }

        public void OnSwitchStart(bool isToPlayback)
        {
            if (button != null)
                button.interactable = false;
        }

        public void OnSwitchComplete(bool isPlayback)
        {
            if (button != null)
                button.interactable = isPlayback;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (image != null)
            {
                if (image.sprite == playSprite)
                    image.sprite = playPressedSprite;
                else if (image.sprite == pauseSprite)
                    image.sprite = pausePressedSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (image != null)
            {
                if (image.sprite == playPressedSprite)
                    image.sprite = playSprite;
                else if (image.sprite == pausePressedSprite)
                    image.sprite = pauseSprite;
            }
        }
    }
}


