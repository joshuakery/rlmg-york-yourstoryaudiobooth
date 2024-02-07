using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PlaybackHeadWindow : BaseWindow
    {
        [SerializeField]
        private PlaybackUIManager playbackManager;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private TMP_Text timeDisplay;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (playbackManager != null)
            {
                playbackManager.onPlay += OnPlay;
                playbackManager.onPause += OnPause;
                playbackManager.onPointerDownEvent += OnPointerDownEvent;
                playbackManager.onPointerUpEvent += OnPointerUpEvent;
                playbackManager.onInitEvent += OnInit;
                playbackManager.onValueChanged.AddListener(OnValueChanged);
            }    
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (playbackManager != null)
            {
                playbackManager.onPlay -= OnPlay;
                playbackManager.onPause -= OnPause;
                playbackManager.onPointerDownEvent-= OnPointerDownEvent;
                playbackManager.onPointerUpEvent -= OnPointerUpEvent;
                playbackManager.onInitEvent -= OnInit;
                playbackManager.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnPlay()
        {
            sequenceManager.KillCurrentSequence();

            if (!isOpen)
                Open();
        }

        private void OnPause()
        {
            sequenceManager.KillCurrentSequence();

            if (isOpen)
                Close();
        }

        private void OnPointerDownEvent()
        {
            sequenceManager.KillCurrentSequence();

            if (!isOpen)
                Open();
        }

        private void OnPointerUpEvent()
        {
            sequenceManager.KillCurrentSequence();

            if (isOpen)
                Close();
        }

        private void OnInit()
        {
            sequenceManager.KillCurrentSequence();

            Close(SequenceType.CompleteImmediately);
        }

        private void OnValueChanged(float value)
        {
            if (audioSource?.clip != null && audioSource.clip.length > 0 && timeDisplay != null)
            {
                // Fix for bug where clip length is double
                float time = value * (audioSource.clip.length / 2f);

                TimeSpan timeSpan = TimeSpan.FromSeconds(time);

                timeDisplay.text = string.Format(
                    "<mspace=0.7em>{0}</mspace>:<mspace=0.7em>{1}</mspace>",
                    timeSpan.ToString("mm"),
                    timeSpan.ToString("ss")
                );
            }
        }


    }
}


