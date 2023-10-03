using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    [CreateAssetMenu(fileName = "UISequenceManager", menuName = "UI Sequence Manager")]
    public class UISequenceManager : ScriptableObject
    {
        private Sequence _currentSequence;
        public Sequence currentSequence
        {
            get
            {
                //Debug.Log(_currentSequence?.id);

                return _currentSequence;
            }
            set
            {
                _currentSequence = value;
            }
        }

        private void _CompleteCurrentSequence()
        {
            if (currentSequence != null)
            {
                currentSequence.Complete();
                currentSequence = null;
            }
        }

        private void _CreateNewSequence()
        {
            _CompleteCurrentSequence();
            if (currentSequence == null)
            {
                currentSequence = DOTween.Sequence();
                currentSequence.id = System.Guid.NewGuid();
                currentSequence.OnComplete(() => { currentSequence = null; });
            }
        }

        public void CompleteCurrentSequence()
        {
            Debug.Log("completing current sequence");
            _CompleteCurrentSequence();
        }

        public void AppendInterval(float interval)
        {
            if (currentSequence == null)
            {
                _CreateNewSequence();
            }

            currentSequence.AppendInterval(interval);
        }

        public void InsertCallback(float atPosition, TweenCallback callback)
        {
            if (currentSequence == null)
            {
                _CreateNewSequence();
            }


            currentSequence.InsertCallback(atPosition, callback);
        }

        public void CreateNewSequenceAfterCurrent()
        {
            if (currentSequence == null)
            {
                _CreateNewSequence();
            }
            else
            {
                float remaining = currentSequence.Duration() - currentSequence.Elapsed();
                currentSequence = DOTween.Sequence();
                currentSequence.id = System.Guid.NewGuid();
                currentSequence.AppendInterval(remaining);
            }
        }

        public void JoinTween(Tween tween)
        {
            if (currentSequence == null)
            {
                _CreateNewSequence();
            }
            currentSequence.Join(tween);
        }

        public void AppendTween(Tween tween)
        {
            if (currentSequence == null)
            {
                _CreateNewSequence();
            }
            currentSequence.Append(tween);
        }

        public void InsertTween(float atPosition, Tween tween)
        {
            if (currentSequence == null)
            {
                _CreateNewSequence();
            }
            currentSequence.Insert(atPosition, tween);
        }

        public void _DebugLog()
        {
            Debug.Log("Calling this debug log function!");
        }

        public void _DebugLog2()
        {
            Debug.Log("Second debug log function!");
        }
    }
}


