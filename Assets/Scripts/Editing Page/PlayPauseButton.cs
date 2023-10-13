using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PlayPauseButton : MonoBehaviour
    {
        [SerializeField]
        private PlaybackUIManager playbackManager;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite playSprite;

        [SerializeField]
        private Sprite pauseSprite;

        private void OnEnable()
        {
            if (playbackManager != null)
            {
                if (button != null)
                    button.onClick.AddListener(playbackManager.OnPlayPauseAudio);

                playbackManager.onPlay += OnPlay;
                playbackManager.onPause += OnPause;
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


    }
}


