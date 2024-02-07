using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class RecordButton : MonoBehaviour
    {
        private AudioRecorderProcess process;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite recordSprite;

        [SerializeField]
        private Sprite stopSprite;

        private void Awake()
        {
            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }

            if (process != null)
            {
                process.onStopRequested += OnStopRequested;
                process.onFail += OnFail;
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }

            if (process != null)
            {
                process.onStopRequested -= OnStopRequested;
                process.onFail -= OnFail;
            }
        }

        private void OnFail(System.Exception e)
        {
            ToggleSprite(false);
        }

        private void OnStopRequested()
        {
            ToggleSprite(false);
        }

        private void OnClick()
        {
            if (process != null)
            {
                if (!process.isRunning)
                {
                    ToggleSprite(true);

                    process.OnStartRecording();
                }
                else
                {
                    ToggleSprite(false);

                    process.OnStopRecording();
                }
            }
        }

        private void ToggleSprite(bool startingRecording)
        {
            if (image != null)
            {
                if (startingRecording == true)
                    image.sprite = stopSprite;
                else
                    image.sprite = recordSprite;
            }
        }
    }
}

