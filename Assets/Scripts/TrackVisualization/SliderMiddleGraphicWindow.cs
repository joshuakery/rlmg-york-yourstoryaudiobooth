using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;
using DG.Tweening;

namespace JoshKery.York.AudioRecordingBooth
{
    [RequireComponent(typeof(RectTransform))]
    public class SliderMiddleGraphicWindow : BaseWindow
    {
        
        private RectTransform rt;

        [SerializeField]
        private RectTransform parentRT;

        [SerializeField]
        private RectTransform audioAreaRT;

        private Tween sizeDeltaTween;

        private Tween positionTween;

        [SerializeField]
        private float duration = 0.2f;

        protected override void Awake()
        {
            base.Awake();

            rt = GetComponent<RectTransform>();
        }

        protected override Sequence _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Sequence sequence = DOTween.Sequence();

            Sequence baseOpenSequence = base._Open(SequenceType.UnSequenced, atPosition);
            if (baseOpenSequence != null)
                sequence.Join(baseOpenSequence);

            if (sizeDeltaTween != null && !sizeDeltaTween.IsComplete())
            {
                sizeDeltaTween.Kill();
                sizeDeltaTween = null;
            }

            if (positionTween != null && !positionTween.IsComplete())
            {
                positionTween.Kill();
                positionTween = null;
            }

            // return to the default position and size, stretching to its parent's bounds
            sizeDeltaTween = DOTween.To(
                    () => rt.sizeDelta.x,
                    (x) => { rt.sizeDelta = new Vector2(x, 0); },
                    0,
                    duration
                );
            sizeDeltaTween.SetEase(Ease.OutQuad);
            sizeDeltaTween.onComplete = () => { sizeDeltaTween = null; };
            sequence.Join(sizeDeltaTween);

            positionTween = DOTween.To(
                () => rt.anchoredPosition.x,
                x => { rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y); },
                0,
                duration
            );
            positionTween.onComplete = () => { positionTween = null; };
            sequence.Join(positionTween);

            if (sequenceManager != null)
            {
                //Attach this to the sequence manager if desired
                sequenceManager.CreateSequenceIfNull();
                AttachTweenToSequence(sequenceType, sequence, sequenceManager.currentSequence, false, atPosition, null);
            }


            return sequence;
        }

        protected override Sequence _Close(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            Sequence sequence = DOTween.Sequence();

            //Append the base close sequence to this override
            Sequence baseCloseSequence = base._Close(SequenceType.UnSequenced, atPosition);
            if (baseCloseSequence != null)
                sequence.Join(base._Close(SequenceType.UnSequenced, atPosition));

            if (sizeDeltaTween != null && !sizeDeltaTween.IsComplete())
            {
                sizeDeltaTween.Kill();
                sizeDeltaTween = null;
            }

            if (positionTween != null && !positionTween.IsComplete())
            {
                positionTween.Kill();
                positionTween = null;
            }

            //change to stretch to the audio area's bounds
            if (audioAreaRT != null && parentRT != null)
            {
                sizeDeltaTween = DOTween.To(
                    () => rt.sizeDelta.x,
                    (x) => { rt.sizeDelta = new Vector2(x, 0); },
                    //resize to the difference of the two RTs' widths
                    audioAreaRT.rect.width - parentRT.rect.width,
                    duration
                );
                sizeDeltaTween.SetEase(Ease.OutQuad);
                sizeDeltaTween.onComplete = () => { sizeDeltaTween = null; };
                sequence.Join(sizeDeltaTween);

                positionTween = DOTween.To(
                    () => rt.anchoredPosition.x,
                    x => { rt.anchoredPosition = new Vector2(x, 0); },
                    //move to the inverse of the parent RT's position
                    -1 * parentRT.anchoredPosition.x,
                    duration
                );
                positionTween.onComplete = () => { positionTween = null; };
                sequence.Join(positionTween);
            }


            if (sequenceManager != null)
            {
                //Attach this to the sequence manager if desired
                sequenceManager.CreateSequenceIfNull();
                AttachTweenToSequence(sequenceType, sequence, sequenceManager.currentSequence, false, atPosition, null);
            }


            return sequence;
        }
    }
}


