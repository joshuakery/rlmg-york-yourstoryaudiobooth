using System;
using EasingCurve;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    public static class UICustomEase
    {
        public static float Linear(float time, float duration, float amplitude, float period)
        {
            return time / duration;
        }

        public static float _EaseIn(float t)
        {
            return t * t;
        }

        public static float EaseIn(float time, float duration, float amplitude, float period)
        {
            return _EaseIn(time / duration);
        }

        //Cubic Bezier Math

        private static float _A(float a1, float a2)
        {
            return 1f - 3f * a2 + 3f * a1;
        }

        private static float _B(float a1, float a2)
        {
            return 3f * a2 - 6f * a1;
        }

        private static float _C(float a1)
        {
            return 3f * a1;
        }

        public static float _CubicBezier(float t)
        {
            //only the y values of the control points may be manipulated here
            //hard-coded here

            float p1 = 1f;
            float p3 = 0f;

            return ((_A(p1, p3) * t + _B(p1, p3)) * t + _C(p1)) * t;

        }

        public static float CubicBezier(float time, float duration, float amplitude, float period)
        {
            return _CubicBezier(time / duration);

        }

        //Overshoot

        private static float _Overshoot(float k, float amplitude, float period)
        {
            //float freq = 2f;
            //float decay = 7f;
            //float amp = 0.03f; //TODO externalize these so that we produce a dynamic function with them

            //float w = freq * MathF.PI * 2;
            //return amp * MathF.Sin(t * freq * MathF.PI * 2) / MathF.Exp(decay * t);

            //float length = end - start;
            //if (value <= 0) return start;
            //if (value >= 1) return start + length;

            //return start + length * (Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 10 - 0.75f) * Mathf.PI * 2 / 3) + 1);

            if (k == 0) return 0;
            if (k == 1) return 1;
            return MathF.Pow(2f, -10f * k) * MathF.Sin((k - amplitude) * (2f * MathF.PI) / period) + 1f; //todo this is wrong with the period and amplitude
        }

        public static float Overshoot(float time, float duration, float amplitude, float period)
        {
            return _Overshoot(time / duration, amplitude, period);
        }
    }

}

