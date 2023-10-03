using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


namespace JoshKery.GenericUI.DOTweenHelpers
{
    public class PerCharTest : MonoBehaviour
    {
        /*private Dictionary<TMP_Text, DOTweenTMPAnimator> doTweenTMPAnimators;

        public TMP_Text primaryText;
        public TMP_Text secondaryText;
        public RectTransform alternate;

        private Sequence sequence;
        private Tween primary;
        private Tween secondary;

        public UIAnimationData primaryAnimation;
        public UIAnimationData secondaryAnimation;

        private Tween GetSequence(int a)
        {
            sequence.Complete();

            primary = TextPerCharFade(primaryText, a);
            secondary = TextPerCharFade(secondaryText, a);

            Tween altTween = alternate.DOScale(new Vector3(a, a, a), 1f);

            sequence = DOTween.Sequence();

            sequence.Join(altTween);
            sequence.Append(primary);
            sequence.Append(secondary);

            sequence = DOTween.Sequence().Append(sequence);

            return sequence;
        }

        private Tween TextPerCharFade(TMP_Text tmp_text, int a)
        {
            if (tmp_text == null) { Debug.Log("null"); return null; }

            if (doTweenTMPAnimators == null)
                doTweenTMPAnimators = new Dictionary<TMP_Text, DOTweenTMPAnimator>();

            DOTweenTMPAnimator animator;
            if (doTweenTMPAnimators != null && doTweenTMPAnimators.ContainsKey(tmp_text))
            {
                animator = doTweenTMPAnimators[tmp_text];
            }
            else
            {
                animator = new DOTweenTMPAnimator(tmp_text);
                doTweenTMPAnimators.Add(tmp_text, animator);
            }

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < animator.textInfo.characterCount; ++i)
            {
                if (!animator.textInfo.characterInfo[i].isVisible) continue;

                Tween _tween;

                _tween = animator.DOFadeChar(i, a, 1);

                sequence.Insert(i * 0.1f, _tween);
            }

            return sequence;
        }*/
    }
}

