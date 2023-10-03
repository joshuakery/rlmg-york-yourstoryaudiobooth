using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    public class BaseStateMachine : BaseDisplay
    {
        #region Static UnityEvents
        private static UnityEvent _onStartUpStateMachines;
        
        public static UnityEvent onStartUpStateMachines
        {
            get
            {
                if (_onStartUpStateMachines == null)
                    _onStartUpStateMachines = new UnityEvent();

                return _onStartUpStateMachines;
            }
        }
        #endregion

        [SerializeField]
        private UISequenceManager sequenceManager;

        #region State Handling Fields
        [SerializeField]
        private List<UIAnimationSequenceData> states;

        public int currentState = 0;

        private int _stateOnStart = 0;

        public int stateOnStart
        {
            get
            {
                return _stateOnStart;
            }
            set
            {
                _stateOnStart = value;
            }
        }

        public int stateOnStartToSet = 0;
        
        protected override void Awake()
        {
            stateOnStart = stateOnStartToSet;

            base.Awake();
        }
        #endregion

        #region Start Up and Monoheaviour Methods
        protected virtual void OnEnable()
        {
            if (onStartUpStateMachines != null)
                onStartUpStateMachines.AddListener(StartUp);
        }

        protected virtual void OnDisable()
        {
            if (onStartUpStateMachines != null)
                onStartUpStateMachines.RemoveListener(StartUp);
        }

        private void StartUp()
        {
            switch(stateOnStart)
            {
                case 0:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Generic State Machine Action methods
        protected virtual Sequence _StateMachineAction(
            UIAnimationSequenceData sequenceData,
            SequenceType sequenceType = SequenceType.UnSequenced,
            float atPosition = 0f
        )
        {
            if (sequenceData == null)
                return null;

            Sequence sequence = GetSequence(sequenceData, gameObject);
            switch (sequenceType)
            {
                case (SequenceType.CompleteImmediately):
                    sequence.Complete();
                    break;
                case (SequenceType.UnSequenced):
                    break;
                case (SequenceType.Join):
                    sequenceManager.JoinTween(sequence);
                    break;
                case (SequenceType.Append):
                    sequenceManager.AppendTween(sequence);
                    break;
                case (SequenceType.Insert):
                    sequenceManager.InsertTween(atPosition, sequence);
                    break;
                case (SequenceType.BackInsert):
                    sequenceManager.InsertTween(sequenceManager.currentSequence.Duration() - atPosition, sequence);
                    break;
            }
            return sequence;

        }

        public virtual Tween StateMachineAction(int index, SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0f)
        {
            if (states != null && states.Count > 0 && index >= 0 && index < states.Count)
            {
                UIAnimationSequenceData sequenceData = states[index];

                if (sequenceData != null)
                    return _StateMachineAction(sequenceData, sequenceType, atPosition);
            }
            return null;
        }

        public virtual Tween GetStateMachineAction(int index)
        {
            return StateMachineAction(index);
        }
        #endregion

        #region Helper Methods (formerly UIAnimatorModule)

        #region Create Tween Helpers
        /// <summary>
        /// Using UIAnimatorModule create tween method, gets a tween based on tweenData.animationData.animationType
        /// </summary>
        /// <param name="tweenData">Data to work from</param>
        /// <returns></returns>
        public Tween CreateTweenFromAnimationData(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.animationData == null) { return null; }

            switch (tweenData.animationData.animationType)
            {
                case (UIAnimationData.AnimationType.Fade):
                    return UIAnimatorModule.Fade(tweenData);
                case (UIAnimationData.AnimationType.Move):
                    return UIAnimatorModule.Move(tweenData);
                case (UIAnimationData.AnimationType.RelativeMove):
                    return UIAnimatorModule.RelativeMove(tweenData);
                case (UIAnimationData.AnimationType.Rotate):
                    return UIAnimatorModule.Rotate(tweenData);
                case (UIAnimationData.AnimationType.Scale):
                    return UIAnimatorModule.Scale(tweenData);
                case (UIAnimationData.AnimationType.SizeDelta):
                    return UIAnimatorModule.SizeDelta(tweenData);
                case (UIAnimationData.AnimationType.SizeDeltaY):
                    return UIAnimatorModule.SizeDeltaY(tweenData);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the chosen WindowAction, tweenData.windowAction, for the BaseWindow, tweenData.baseWindow
        /// </summary>
        /// <param name="tweenData">Data specifying windowAction and baseWindow to work with</param>
        /// <returns></returns>
        protected Tween CreateTweenFromBaseWindow(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.baseWindow == null) { return null; }

            return tweenData.baseWindow.GetWindowAction(tweenData.windowAction);
        }

        /// <summary>
        /// Analogous to CreateTweenFromBaseWindow, but gets the sequence for the BaseStateMachine's chosen state
        /// </summary>
        /// <param name="tweenData">Data specifying BaseStateMachine and StateToAnimateTo</param>
        /// <returns></returns>
        protected Tween CreateTweenFromBaseStateMachine(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.baseStateMachine == null) { return null; }

            return tweenData.baseStateMachine.GetStateMachineAction(tweenData.stateToAnimateTo);
        }

        /// <summary>
        /// Gets the desired WindowAction for all BaseWindows in childWindows
        /// And assembles them into a single parent sequence
        /// </summary>
        /// <param name="tweenData">Data specifies how child tweens will be assembled via tweenData.childSequenceType and offset parameters</param>
        /// <param name="childWindows">BaseWindows to work with</param>
        /// <returns></returns>
        protected Tween CreateTweenFromAllChildWindows(UIAnimationIndividualSequencingData tweenData, BaseWindow[] childWindows)
        {
            Sequence parentSequence = null;
            if (childWindows != null & childWindows.Length > 0)
            {
                Sequence lastChildSequence = null;
                foreach (BaseWindow childWindow in childWindows)
                {
                    if (childWindow != null)
                    {
                        if (parentSequence == null)
                            parentSequence = DOTween.Sequence();

                        Sequence childSequence = childWindow.GetWindowAction(tweenData.windowAction);

                        parentSequence = AttachTweenToSequence(tweenData.childSequenceType, childSequence, parentSequence,
                                              tweenData.doOffsetChildrenFromTheirStarts, tweenData.childOffset, lastChildSequence);

                        lastChildSequence = childSequence;
                    }


                }
            }

            parentSequence.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();

            });

            parentSequence.OnComplete(() =>
            {
                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return parentSequence;
        }
        #endregion

        #region Core Tween Creation
        /// <summary>
        /// Helper with switch for creating a tween
        /// </summary>
        /// <param name="tweenData">Data specifying how to create a tween via tweenData.sequenceSource</param>
        /// <param name="childWindows">Optional BaseWindow array to pass to CreateTweenFromAllChildWindows</param>
        /// <returns></returns>
        public Tween CreateTween(UIAnimationIndividualSequencingData tweenData, BaseWindow[] childWindows = null)
        {
            switch (tweenData.sequenceSource)
            {
                case (SequenceSource.AnimationData):
                    return CreateTweenFromAnimationData(tweenData);
                case (SequenceSource.BaseWindow):
                    return CreateTweenFromBaseWindow(tweenData);
                case (SequenceSource.AllChildWindows):
                    return CreateTweenFromAllChildWindows(tweenData, childWindows);
                case (SequenceSource.BaseStateMachine):
                    return CreateTweenFromBaseStateMachine(tweenData);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Based on animationSequenceData, generates a Sequence
        /// </summary>
        /// <param name="animationSequenceData">Holds individualSequencingData to create tweens from; data.sequenceType specifies how tweens will be attached together</param>
        /// <param name="gameObject">Default objectToAnimate for each individualSequencingData in </param>
        /// <param name="childWindows"></param>
        /// <returns></returns>
        public Sequence GetSequence(UIAnimationSequenceData animationSequenceData, GameObject gameObject, BaseWindow[] childWindows = null)
        {
            Sequence sequence = null;

            Tween lastTween = null;
            if (animationSequenceData != null)
            {
                foreach (UIAnimationIndividualSequencingData individualSequencingData in animationSequenceData.individualSequencingData)
                {
                    if (individualSequencingData.objectToAnimate == null) { individualSequencingData.objectToAnimate = gameObject; }

                    Tween tween = CreateTween(individualSequencingData, childWindows);

                    if (tween != null)
                    {
                        sequence = AttachTweenToSequence(
                            animationSequenceData.sequenceType,
                            tween,
                            sequence,
                            individualSequencingData.doOffsetFromStartOfLastTween,
                            individualSequencingData.offset,
                            lastTween
                        );

                        lastTween = tween;
                    }

                    if (sequence == null) { Debug.Log("null seq - all the tweens came out null"); }

                }
            }

            return sequence;
        }
        #endregion

        #region Sequence Assembly Helpers
        /// <summary>
        /// Following sequenceType, adds tween to sequence
        /// </summary>
        /// <param name="sequenceType">How tween will be added e.g. Join</param>
        /// <param name="tween">Tween to add</param>
        /// <param name="sequence">Sequence to add to</param>
        /// <param name="doOffsetFromStartOfLastTween">If true, will offset from the start of the last tween</param>
        /// <param name="offset">Offset duration (in seconds) for Insert and BackInsert, including offsetting from Last Tween</param>
        /// <param name="lastTween">"Last Tween" to be offset from if doOffsetFromStartOfLastTween is true</param>
        /// <returns></returns>
        public static Sequence AttachTweenToSequence(
            SequenceType sequenceType,
            Tween tween,
            Sequence sequence,
            bool doOffsetFromStartOfLastTween,
            float offset,
            Tween lastTween
        )
        {
            if (tween != null)
            {
                if (sequence == null)
                    sequence = DOTween.Sequence();

                switch (sequenceType)
                {
                    case SequenceType.CompleteImmediately:
                        tween.Complete();
                        break;
                    case SequenceType.UnSequenced:
                        break;
                    case SequenceType.Join:
                        sequence.Join(tween);
                        break;
                    case SequenceType.Append:
                        sequence.Append(tween);
                        break;
                    case SequenceType.Insert:
                        if (doOffsetFromStartOfLastTween)
                        {
                            if (lastTween != null)
                            {
                                sequence.Insert(sequence.Duration() - lastTween.Duration() + offset, tween);
                            }
                            else
                                sequence.Insert(offset, tween);
                        }
                        else
                        {
                            sequence.Insert(offset, tween);
                        }
                        break;
                    case SequenceType.BackInsert:
                        if (doOffsetFromStartOfLastTween)
                        {
                            if (lastTween != null)
                            {
                                sequence.Insert(sequence.Duration() - lastTween.Duration() - offset, tween);
                            }
                            else
                                sequence.Insert(sequence.Duration() - offset, tween);

                        }
                        else
                        {
                            sequence.Insert(sequence.Duration() - offset, tween);
                        }
                        break;

                }
            }

            return sequence;
        }
        #endregion

        #endregion

    }

}


