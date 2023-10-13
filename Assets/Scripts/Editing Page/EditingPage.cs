using JoshKery.GenericUI.DOTweenHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace JoshKery.York.AudioRecordingBooth
{
    public class EditingPage : GenericPage
    {

        [SerializeField]
        private AudioClipLoader audioClipLoader;

        [SerializeField]
        private PlaybackUIManager playbackUIManager;

        [SerializeField]
        private MinMaxSlider trimSlider;

        [SerializeField]
        private Slider volumeSlider;

        [SerializeField]
        private PlaybackTrimSwitchManager playbackTrimSwitchManager;

        private TrimErrorDisplay errorDisplay;

        protected override void Awake()
        {
            base.Awake();

            errorDisplay = FindObjectOfType<TrimErrorDisplay>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.AddListener(OnAudioClipLoaded);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.RemoveListener(OnAudioClipLoaded);
        }

        private void OnAudioClipLoaded()
        {
            if (sequenceManager != null)
                sequenceManager.CreateNewSequenceAfterCurrent();

            if (playbackUIManager != null)
            {
                playbackUIManager.Init();
            }

            if (playbackTrimSwitchManager != null)
            {
                playbackTrimSwitchManager.Switch(true);
            }

            if (trimSlider != null)
            {
                trimSlider.SetValues(0, 1);
            }

            if (volumeSlider != null)
                volumeSlider.value = 0.75f;
        }

        protected override void _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (errorDisplay != null)
                errorDisplay.Close(SequenceType.CompleteImmediately);

            StartCoroutine(InitCo());
            base._Open(sequenceType, atPosition);
        }

        private IEnumerator InitCo()
        {
            if (audioClipLoader != null)
                yield return audioClipLoader.Reload();
        }

    }
}


