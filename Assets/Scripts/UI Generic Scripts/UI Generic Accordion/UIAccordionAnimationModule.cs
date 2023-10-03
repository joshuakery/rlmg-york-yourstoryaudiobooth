using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.GenericUI.Accordion
{
    public static class UIAccordionAnimatorModule
    {
 /*       private static Tween SizeDeltaY(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();
            AccordionElement accordionElement = tweenData.objectToAnimate.GetComponent<AccordionElement>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            Vector2 to = new Vector2(rt.rect.width, accordionElement.fullHeight); //DYNAMIC TO VALUE based on accordionElement.fullHeight
            Vector2 from = new Vector2(rt.rect.width, animationData.from.y);

            if (!animationData.useFrom)
                _tween = rt.DOSizeDelta(to, animationData.duration);
            else
            {
                if (animationData.trueFrom)
                {
                    _tween = rt.DOSizeDelta(from, animationData.duration).From();

                    if (animationData.setToFromImmediately)
                        rt.sizeDelta = from;
                }
                else
                {
                    Vector2 aux = rt.sizeDelta;

                    rt.sizeDelta = to; //set scale to .to as the reference for .From()
                    _tween = rt.DOSizeDelta(from, animationData.duration).From(); //sets scale to .from

                    rt.sizeDelta = aux; //set it back to what it was
                }
            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            UIAnimatorModule.SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.useFrom && !animationData.trueFrom)
                    rt.sizeDelta = to;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return _tween;
        }

        public static Tween CreateTweenFromAnimationData(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.animationData == null) { return null; }

            switch (tweenData.animationData.animationType)
            {
                case (UIAnimationData.AnimationType.SizeDeltaY):
                    return SizeDeltaY(tweenData);
                default:
                    return UIAnimatorModule.CreateTweenFromAnimationData(tweenData);
            }
        }
        public static Tween CreateTween(UIAnimationIndividualSequencingData tweenData, BaseWindow[] childWindows = null)
        {
            switch (tweenData.sequenceSource)
            {
                case (SequenceSource.AnimationData):
                    return CreateTweenFromAnimationData(tweenData);
                default:
                    return UIAnimatorModule.CreateTween(tweenData, childWindows);
            }
        }

        public static Sequence GetSequence(UIAnimationSequenceData animationSequenceData, GameObject gameObject, BaseWindow[] childWindows = null)
        {
            Sequence sequence = null;

            Tween lastTween = null;
            if (animationSequenceData != null)
            {
                foreach (UIAnimationIndividualSequencingData individualSequencingData in animationSequenceData.individualSequencingData)
                {
                    if (individualSequencingData.objectToAnimate == null) { individualSequencingData.objectToAnimate = gameObject; }

                    Tween tween = CreateTween(individualSequencingData, childWindows);

                    sequence = UIAnimatorModule.AttachTweenToSequence(animationSequenceData.sequenceType, tween, sequence,
                                    individualSequencingData.doOffsetFromStartOfLastTween, individualSequencingData.offset, lastTween);
                    lastTween = tween;
                }
            }

            return sequence;
        }*/
        
    }
}


