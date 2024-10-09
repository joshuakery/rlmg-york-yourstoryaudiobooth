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
            Sequence sequence = null;

            Sequence baseOpenSequence = base._Open(SequenceType.UnSequenced, atPosition);
            if (baseOpenSequence != null)
            {
                if (sequence == null) sequence = DOTween.Sequence();

                sequence.Join(baseOpenSequence);
            }   

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

            if (material.GetFloat("_Start") != 0f)
            {
                startTween = DOTween.To(
                    () => material.GetFloat("_Start"),
                    x => material.SetFloat("_Start", x),
                    0f,
                    speed
                );
                startTween.SetEase(Ease.OutQuad);
                startTween.onComplete = () => { startTween = null; };

                if (sequence == null) sequence = DOTween.Sequence();

                sequence.Insert(openDelay, startTween);
            }

            if (material.GetFloat("_End") != 1f)
            {
                endTween = DOTween.To(
                    () => material.GetFloat("_End"),
                    x => material.SetFloat("_End", x),
                    1f,
                    speed
                );
                endTween.SetEase(Ease.OutQuad);
                endTween.onComplete = () => { endTween = null; };

                if (sequence == null) sequence = DOTween.Sequence();

                sequence.Insert(openDelay, endTween);
            }

            //Attach this to the sequence manager if desired
            if (sequence != null && sequenceManager != null)
            {
                sequenceManager.CreateSequenceIfNull();
                AttachTweenToSequence(sequenceType, sequence, sequenceManager.currentSequence, false, atPosition, null);
            }

            return sequence;
        }

        protected override Sequence _Close(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Sequence sequence = null;

            Sequence baseCloseSequence = base._Close(SequenceType.UnSequenced, atPosition);
            if (baseCloseSequence != null)
            {
                if (sequence == null) sequence = DOTween.Sequence();

                sequence.Join(baseCloseSequence);
            }
                

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
                if (material.GetFloat("_Start") != minMaxSlider.Values.minValue)
                {
                    startTween = DOTween.To(
                        () => material.GetFloat("_Start"),
                        x => material.SetFloat("_Start", x),
                        minMaxSlider.Values.minValue,
                        speed
                    );
                    startTween.SetEase(Ease.OutQuad);
                    startTween.onComplete = () => { startTween = null; };

                    if (sequence == null) sequence = DOTween.Sequence();

                    sequence.Insert(closeDelay, startTween);
                }

                if (material.GetFloat("_End") != minMaxSlider.Values.maxValue)
                {
                    endTween = DOTween.To(
                        () => material.GetFloat("_End"),
                        x => material.SetFloat("_End", x),
                        minMaxSlider.Values.maxValue,
                        speed
                    );
                    endTween.SetEase(Ease.OutQuad);
                    endTween.onComplete = () => { endTween = null; };

                    if (sequence == null) sequence = DOTween.Sequence();

                    sequence.Insert(closeDelay, endTween);
                }
            }

            //Attach this to the sequence manager if desired
            if (sequence != null && sequenceManager != null)
            {
                sequenceManager.CreateSequenceIfNull();
                AttachTweenToSequence(sequenceType, sequence, sequenceManager.currentSequence, false, atPosition, null);
            }

            return sequence;

        }
    }
}


