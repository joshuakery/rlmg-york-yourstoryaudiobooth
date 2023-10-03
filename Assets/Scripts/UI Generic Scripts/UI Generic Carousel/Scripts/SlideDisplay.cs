using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.GenericUI.Carousel
{
    public class SlideDisplay : BaseWindow
    {
        [SerializeField]
        private UIAnimationSequenceData outForNextSequence;

        [SerializeField]
        private UIAnimationSequenceData inAsNextSequence;

        [SerializeField]
        private UIAnimationSequenceData outForPrevSequence;

        [SerializeField]
        private UIAnimationSequenceData inAsPrevSequence;

        public virtual void SlideOutForNext()
        {
            _WindowAction(outForNextSequence, SequenceType.Join);
        }
        public virtual void SlideInAsNext()
        {
            _WindowAction(inAsNextSequence, SequenceType.Join);
        }

        public virtual void SlideOutForPrev()
        {
            _WindowAction(outForPrevSequence, SequenceType.Join);
        }

        public virtual void SlideInAsPrev()
        {
            _WindowAction(inAsPrevSequence, SequenceType.Join);
        }
    }
}


