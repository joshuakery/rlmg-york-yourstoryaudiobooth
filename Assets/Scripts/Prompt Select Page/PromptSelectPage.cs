using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoshKery.GenericUI.DOTweenHelpers;
using DG.Tweening;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PromptSelectPage : GenericPage
    {
        [SerializeField]
        private BaseWindow samplesWindow;

        [SerializeField]
        private RectTransform textureMask;

        [SerializeField]
        private RectTransform textureCircle;

        [SerializeField]
        private RectTransform smallDottedCircle;

        [SerializeField]
        private RectTransform largeDottedCircle;

        private void Start()
        {
            float duration = 20f;

            if (textureMask != null && textureCircle != null)
            {
                DOTween.To(
                    () => textureMask.localScale,
                    v =>
                    {
                        textureMask.localScale = v;
                        float inverseScale = 1f / textureMask.localScale.x;
                        textureCircle.localScale = new Vector3(inverseScale, inverseScale, inverseScale);
                    },
                    new Vector3(1, 1, 1),
                    10f
                ).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);

            }

            if (smallDottedCircle != null)
                smallDottedCircle.DORotate(new Vector3(0, 0, 90f), duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

            if (largeDottedCircle != null)
                largeDottedCircle.DORotate(new Vector3(0,0,90f), duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        protected override DG.Tweening.Sequence _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (samplesWindow != null)
                samplesWindow.Close(SequenceType.CompleteImmediately);

            return base._Open(sequenceType, atPosition);
        }
    }
}


