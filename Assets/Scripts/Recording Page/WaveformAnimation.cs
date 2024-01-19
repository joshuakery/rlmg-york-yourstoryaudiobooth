using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class WaveformAnimation : BaseWindow
    {
        private AudioRecorderProcess process;

        [SerializeField]
        private RectTransform wave0rt;
        private Tween wave0tween;
        private Tween wave0scaleTween;

        [SerializeField]
        private RectTransform wave1rt;
        private Tween wave1tween;

        [SerializeField]
        private RectTransform wave2rt;
        private Tween wave2tween;

        [SerializeField]
        private float loopDuration = 10f;

        protected override void Awake()
        {
            base.Awake();

            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (process != null)
            {
                process.onFirstProcessStarted += OnRecordingStart;
                process.onFail += OnFail;
                process.onRecordingError += OnRecordingEnd;
                process.onRecordingSuccess += OnRecordingEnd;
                process.onStopRequested += OnRecordingEnd;
                process.onInit += OnRecordingEnd;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (process != null)
            {
                process.onFirstProcessStarted -= OnRecordingStart;
                process.onFail -= OnFail;
                process.onRecordingError -= OnRecordingEnd;
                process.onRecordingSuccess -= OnRecordingEnd;
                process.onStopRequested -= OnRecordingEnd;
                process.onInit -= OnRecordingEnd;
            }
        }

        private void OnRecordingStart()
        {
            _Open(SequenceType.Insert, 0.5f);
        }

        private void OnFail(System.Exception e)
        {
            OnRecordingEnd();
        }

        private void OnRecordingEnd()
        {
            if (isOpen)
                DoClose();
        }

        protected override Sequence _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Animate();

            return base._Open(sequenceType, atPosition);
        }

        protected override Sequence _Close(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Sequence tween = base._Close(sequenceType, atPosition);

            //create wrapper to add new oncomplete
            Sequence sequence = DOTween.Sequence();

            sequence.Join(tween);

            sequence.OnComplete(StopAnimating);

            return sequence;
        }

        public void Animate()
        {
            if (wave0rt != null)
            {
                wave0rt.anchoredPosition = new Vector2(-1460f, wave0rt.anchoredPosition.y);
                wave0tween = wave0rt.DOAnchorPosX(wave0rt.anchoredPosition.x - 1920f, loopDuration);
                wave0tween.SetEase(Ease.Linear);
                wave0tween.SetLoops(-1, LoopType.Restart);
            }

            if (wave1rt != null)
            {
                wave1rt.anchoredPosition = new Vector2(1900f, wave1rt.anchoredPosition.y);
                wave1tween = wave1rt.DOAnchorPosX(wave1rt.anchoredPosition.x + 1920f, loopDuration);
                wave1tween.SetEase(Ease.Linear);
                wave1tween.SetLoops(-1, LoopType.Restart);
            }

            if (wave2rt != null)
            {
                wave2tween = wave2rt.DOAnchorPosX(-1920f, loopDuration);
                wave2tween.SetEase(Ease.Linear);
                wave2tween.SetLoops(-1, LoopType.Restart);
            }

        }

        public void StopAnimating()
        {
            if (wave0tween != null)
            {
                wave0tween.Kill();
                wave0tween = null;
                
            }

            if (wave1tween != null)
            {
                wave1tween.Kill();
                wave1tween = null;
            }

            if (wave2tween != null)
            {
                wave2tween.Kill();
                wave2tween = null;
            }

        }
    }
}


