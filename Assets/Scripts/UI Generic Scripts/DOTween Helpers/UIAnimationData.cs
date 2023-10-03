using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    [CreateAssetMenu(fileName = "UIAnimation", menuName = "UI Animation")]
    public class UIAnimationData : ScriptableObject
    {
        public enum AnimationType
        {
            Fade,
            Move,
            Rotate,
            Scale,
            RelativeMove,
            SizeDelta,
            Color,
            VertexColor,
            TextPerCharFade,
            SizeDeltaY
        }

        public AnimationType animationType;

        public static AnimationType[] sequenceAnimationTypes = {
            AnimationType.TextPerCharFade
        };

        public float duration;
        public float delay;

        public Vector3 to;

        public bool useFrom;

        public bool trueFrom = false;
        public bool setToFromImmediately = false;
        public Vector3 from;

        public enum EasingOption
        {
            None,
            Ease,
            AnimationCurve,
            CubicBezierCurve,
            CustomEase
        }

        public EasingOption easingOption = EasingOption.None;

        public Ease ease = Ease.OutQuad;
        public AnimationCurve animationCurve;

        public Vector2 p1;
        public Vector2 p2;

        public enum CustomEase
        {
            Linear,
            EaseIn,
            CubicBezier,
            Overshoot
        }

        public CustomEase customEase;

        public float easeOvershootOrAmplitude = 1.70158f;
        public float easePeriod = 0f;

        public int loops;
        public LoopType loopType = LoopType.Restart;

        public float perCharOffset = 0.1f;
        public float perCharDuration = 1f;
        public bool doPerCharOffsetBasedOnTotalDuration = false;
    }
}


