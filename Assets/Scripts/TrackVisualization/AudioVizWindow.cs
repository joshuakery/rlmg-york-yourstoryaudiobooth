using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI.Extensions;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class AudioVizWindow : BaseWindow
    {
        [SerializeField]
        private Material material;

        [SerializeField]
        private MinMaxSlider minMaxSlider;

        private Tween startTween;
        private Tween endTween;

        [SerializeField]
        private float speed = 1;

        [SerializeField]
        private float openDelay = 0f;

        [SerializeField]
        private float closeDelay = 0f;

        protected override Sequence _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Sequence sequence = DOTween.Sequence();

            Sequence baseOpenSequence = base._Open(SequenceType.UnSequenced, atPosition);
            if (baseOpenSequence != null)
                sequence.Join(baseOpenSequence);

            if (startTween != null && !startTween.IsComplete())
            {
                startTween.Kill();
                startTween = null;
            }


            if (endTween != null && !endTween.IsComplete())
            {
                endTween.Kill();
                endTween = null;
            }

            startTween = DOTween.To(
                () => material.GetFloat("_Start"),
                x => material.SetFloat("_Start", x),
                0f,
                speed
            );
            startTween.SetEase(Ease.OutQuad);
            startTween.onComplete = () => { startTween = null; };
            sequence.Insert(openDelay, startTween);

            endTween = DOTween.To(
                () => material.GetFloat("_End"),
                x => material.SetFloat("_End", x),
                1f,
                speed
            );
            endTween.SetEase(Ease.OutQuad);
            endTween.onComplete = () => { endTween = null; };
            sequence.Insert(openDelay, endTween);

            //Attach this to the sequence manager if desired
            if (sequenceManager != null)
            {
                sequenceManager.CreateSequenceIfNull();
                AttachTweenToSequence(sequenceType, sequence, sequenceManager.currentSequence, false, atPosition, null);
            }

            return sequence;
        }

        protected override Sequence _Close(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Sequence sequence = DOTween.Sequence();

            Sequence baseCloseSequence = base._Close(SequenceType.UnSequenced, atPosition);
            if (baseCloseSequence != null)
                sequence.Join(baseCloseSequence);

            if (startTween != null && !startTween.IsComplete())
            {
                startTween.Kill();
                startTween = null;
            }

            if (endTween != null && !endTween.IsComplete())
            {
                endTween.Kill();
                endTween = null;
            }

            if (minMaxSlider != null)
            {
                startTween = DOTween.To(
                    () => material.GetFloat("_Start"),
                    x => material.SetFloat("_Start", x),
                    minMaxSlider.Values.minValue,
                    speed
                );
                startTween.SetEase(Ease.OutQuad);
                startTween.onComplete = () => { startTween = null; };
                sequence.Insert(closeDelay, startTween);

                endTween = DOTween.To(
                    () => material.GetFloat("_End"),
                    x => material.SetFloat("_End", x),
                    minMaxSlider.Values.maxValue,
                    speed
                );
                endTween.SetEase(Ease.OutQuad);
                endTween.onComplete = () => { endTween = null; };
                sequence.Insert(closeDelay, endTween);
            }

            //Attach this to the sequence manager if desired
            if (sequenceManager != null)
            {
                sequenceManager.CreateSequenceIfNull();
                AttachTweenToSequence(sequenceType, sequence, sequenceManager.currentSequence, false, atPosition, null);
            }


            return sequence;

        }
    }
}


