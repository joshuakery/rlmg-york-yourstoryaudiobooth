using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class SampleStoryDisplay : BaseWindow
    {
        public delegate void PlayEvent(int id);
        public static PlayEvent onPlay;

        public delegate void StopEvent();
        public static StopEvent onStop;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite playingSprite;

        [SerializeField]
        private Sprite stoppedSprite;

        [SerializeField]
        private PageManager pageManager;

        [SerializeField]
        private int id;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (button != null)
                button.onClick.AddListener(OnClick);

            onPlay += OnOtherDisplayPlay;

            if (pageManager != null)
                pageManager.onNewPage += OnNewPage;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (button != null)
                button.onClick.RemoveListener(OnClick);

            onPlay -= OnOtherDisplayPlay;

            if (pageManager != null)
                pageManager.onNewPage -= OnNewPage;
        }

        private void Update()
        {
            if (audioSource != null && image != null)
            {
                if (!audioSource.isPlaying && image.sprite == playingSprite)
                    StopAudio();
            }
        }

        private void OnClick()
        {
            if (audioSource == null) { return; }

            if (audioSource.isPlaying)
                StopAudio();
            else
                PlayAudio();
        }

        private void PlayAudio()
        {
            if (audioSource == null) { return; }

            audioSource.Play();

            ToggleGraphics(true);

            onPlay?.Invoke(id);
        }

        private void StopAudio()
        {
            if (audioSource == null) { return; }

            audioSource.Pause();

            audioSource.time = 0;

            ToggleGraphics(false);

            onStop?.Invoke();
        }

        private void ToggleGraphics(bool toPlaying)
        {
            if (image != null)
            {
                if (toPlaying)
                    image.sprite = playingSprite;
                else
                    image.sprite = stoppedSprite;
            }
        }

        private void OnOtherDisplayPlay(int otherID)
        {
            if (id != otherID)
                StopAudio();
        }

        private void OnNewPage(PageManager.Page page)
        {
            StopAudio();
        }
    }
}


