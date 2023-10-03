using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EasingCurve;
using TMPro;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    public static class UIAnimatorModule
    {
        #region Easing
        public static void SetEase(Tween tween, UIAnimationData animationData)
        {
            if (tween == null || animationData == null) { return; }

            switch (animationData.easingOption)
            {
                case UIAnimationData.EasingOption.Ease:
                    tween.SetEase(animationData.ease);
                    tween.easePeriod = animationData.easePeriod;
                    tween.easeOvershootOrAmplitude = animationData.easeOvershootOrAmplitude;
                    break;

                case UIAnimationData.EasingOption.AnimationCurve:
                    tween.SetEase(animationData.animationCurve);
                    break;

                case UIAnimationData.EasingOption.CubicBezierCurve:
                    Vector2[] controlPointStrips = new Vector2[] {
                    new Vector2(0.0f, 0.0f),
                    new Vector2(animationData.p1.x, animationData.p1.y),
                    new Vector2(animationData.p2.x, animationData.p2.y),
                    new Vector2(1.0f, 1.0f)
                };
                    tween.SetEase(EasingAnimationCurve.BezierToAnimationCurve(controlPointStrips));
                    break;

                case UIAnimationData.EasingOption.CustomEase:
                    tween.SetEase(GetCustomEase(animationData.customEase));
                    tween.easePeriod = animationData.easePeriod;
                    tween.easeOvershootOrAmplitude = animationData.easeOvershootOrAmplitude;
                    break;

                default:
                    break;
            }
        }

        private static EaseFunction GetCustomEase(UIAnimationData.CustomEase customEase)
        {
            switch (customEase)
            {
                case UIAnimationData.CustomEase.Linear:
                    return new EaseFunction(UICustomEase.Linear);

                case UIAnimationData.CustomEase.EaseIn:
                    return new EaseFunction(UICustomEase.EaseIn);

                case UIAnimationData.CustomEase.CubicBezier:
                    return new EaseFunction(UICustomEase.CubicBezier);

                case UIAnimationData.CustomEase.Overshoot:
                    return new EaseFunction(UICustomEase.Overshoot);

                default:
                    return new EaseFunction(UICustomEase.EaseIn);
            }
        }
        #endregion

        #region Tween Creators
        public static Tween Fade(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.objectToAnimate.GetComponent<CanvasGroup>() == null)
                tweenData.objectToAnimate.AddComponent<CanvasGroup>();

            if (tweenData.objectToAnimate.GetComponent<Canvas>() == null)
                tweenData.objectToAnimate.AddComponent<Canvas>();

            CanvasGroup canvasGroup = tweenData.objectToAnimate.GetComponent<CanvasGroup>();
            Canvas canvas = tweenData.objectToAnimate.GetComponent<Canvas>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            if (!animationData.useFrom)
                _tween = canvasGroup.DOFade(animationData.to.x, animationData.duration);
            else
            {
                float aux = canvasGroup.alpha;

                canvasGroup.alpha = animationData.to.x; //set alpha to .to.x as the reference for .From()
                _tween = canvasGroup.DOFade(animationData.from.x, animationData.duration).From(); //sets alpha to .from.x

                canvasGroup.alpha = aux; //set it back to what it was
            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {

                if (!canvas.enabled && animationData.to.x != 0)
                    canvas.enabled = true;

                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.to.x == 0)
                    canvas.enabled = false;
                else
                    canvas.enabled = true;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();

            });

            return _tween;
        }

        public static Tween Move(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            if (!animationData.useFrom)
                _tween = rt.DOAnchorPos(animationData.to, animationData.duration);
            else
            {
                Vector2 aux = rt.anchoredPosition;

                rt.anchoredPosition = animationData.to; //set pos to .to as the reference for .From()
                _tween = rt.DOAnchorPos(animationData.from, animationData.duration).From(); //sets pos to .from

                rt.anchoredPosition = aux; //set it back to what it was
            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {

                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.useFrom)
                    rt.anchoredPosition = animationData.to;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return _tween;
        }

        public static Tween RelativeMove(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            Vector2 to = rt.anchoredPosition + (Vector2)animationData.to;
            Vector2 from = rt.anchoredPosition + (Vector2)animationData.from;

            if (!animationData.useFrom)
            {
                _tween = rt.DOAnchorPos(to, animationData.duration);
            }
            else
            {
                if (animationData.trueFrom)
                {
                    _tween = rt.DOAnchorPos(from, animationData.duration).From();
                }
                else
                {
                    Vector2 aux = rt.anchoredPosition;

                    rt.anchoredPosition = to; //set pos to 'to' as the reference for .From()
                    _tween = rt.DOAnchorPos(from, animationData.duration).From(); //sets pos to 'from'

                    rt.anchoredPosition = aux; //set it back to what it was
                }


            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {

                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.useFrom)
                    rt.anchoredPosition = to;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return _tween;
        }

        public static Tween Rotate(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            if (!animationData.useFrom)
                _tween = rt.DORotate(animationData.to, animationData.duration);
            else
            {
                Vector2 aux = rt.localEulerAngles;

                rt.localEulerAngles = animationData.to; //set rot to .to as the reference for .From()
                _tween = rt.DORotate(animationData.from, animationData.duration).From(); //sets rot to .from

                rt.localEulerAngles = aux; //set it back to what it was
            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.useFrom)
                    rt.localEulerAngles = animationData.to;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return _tween;
        }

        public static Tween Scale(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            if (!animationData.useFrom)
                _tween = rt.DOScale(animationData.to, animationData.duration);
            else
            {
                Vector2 aux = rt.localScale;

                rt.localScale = animationData.to; //set scale to .to as the reference for .From()
                _tween = rt.DOScale(animationData.from, animationData.duration).From(); //sets scale to .from

                rt.localScale = aux; //set it back to what it was
            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.useFrom)
                    rt.localScale = animationData.to;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return _tween;
        }

        public static Tween SizeDelta(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            if (!animationData.useFrom)
                _tween = rt.DOSizeDelta(animationData.to, animationData.duration);
            else
            {
                if (animationData.trueFrom)
                {
                    _tween = rt.DOSizeDelta(animationData.from, animationData.duration).From();
                }
                else
                {
                    Vector2 aux = rt.sizeDelta;

                    rt.sizeDelta = animationData.to; //set scale to .to as the reference for .From()
                    _tween = rt.DOSizeDelta(animationData.from, animationData.duration).From(); //sets scale to .from

                    rt.sizeDelta = aux; //set it back to what it was
                }
            }

            if (animationData.delay > 0)
                _tween.SetDelay(animationData.delay);

            SetEase(_tween, animationData);

            _tween.SetLoops(animationData.loops, animationData.loopType);

            _tween.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            _tween.OnComplete(() =>
            {
                if (animationData.useFrom && !animationData.trueFrom)
                    rt.sizeDelta = animationData.to;

                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return _tween;
        }

        public static Tween SizeDeltaY(UIAnimationIndividualSequencingData tweenData)
        {
            RectTransform rt = tweenData.objectToAnimate.GetComponent<RectTransform>();

            UIAnimationData animationData = tweenData.animationData;

            Tween _tween;

            Vector2 to = new Vector2(rt.rect.width, animationData.to.y);
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

            SetEase(_tween, animationData);

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
#endregion Tween Creators
    }
}

