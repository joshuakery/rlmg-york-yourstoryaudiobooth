using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    public class BaseWindow : BaseDisplay
    {
        public enum WindowAction
        {
            None = 0,
            Open = 1,
            Close = 2,
            Pulse = 3
        }

        #region Static UnityEvents
        private static UnityEvent _onAwakeWindows;

        public static UnityEvent onAwakeWindows
        {
            get
            {
                if (_onAwakeWindows == null)
                    _onAwakeWindows = new UnityEvent();

                return _onAwakeWindows;
            }
        }

        private static UnityEvent _onStartUpWindows;


        public static UnityEvent onStartUpWindows
        {
            get
            {
                if (_onStartUpWindows == null)
                    _onStartUpWindows = new UnityEvent();

                return _onStartUpWindows;
            }
        }
        #endregion

        #region FIELDS
        [SerializeField]
        protected UISequenceManager sequenceManager;

        public bool isOpen = true;

        /// <summary>
        /// If true, on startup all child windows will have the same open or close state as this window
        /// </summary>
        public bool doSyncChildWindows = true;

        public BaseWindow[] exemptions;

        private bool _doOpenOnStart = true;

        public bool doOpenOnStart
        {
            get
            {
                return _doOpenOnStart;
            }
            set
            {
                _doOpenOnStart = value;

                if (doSyncChildWindows && childWindows != null && childWindows.Length > 0)
                {
                    foreach (BaseWindow childWindow in childWindows)
                    {
                        childWindow.doOpenOnStart = _doOpenOnStart;
                        childWindow.doOpenOnStartToSet = _doOpenOnStart;
                    }
                }
            }
        }

        public bool doOpenOnStartToSet = true;

        [SerializeField]
        protected UIAnimationSequenceData openSequence;

        [SerializeField]
        protected UIAnimationSequenceData closeSequence;

        /// <summary>
        /// Array of BaseWindows that are used when an animation is prepared using
        /// all childWindows option
        /// </summary>
        [SerializeField]
        private BaseWindow[] childWindows;
        #endregion

        #region Start Up & Monobehvaiour Methods
        protected override void Awake()
        {
            ResetChildWindows();

            doOpenOnStart = doOpenOnStartToSet;

            base.Awake();
        }

        /// <summary>
        /// Sets childWindows array, excluding the BaseWindow that is this component itself
        /// </summary>
        protected virtual void ResetChildWindows()
        {
            childWindows = GetComponentsInChildren<BaseWindow>();

            List<BaseWindow> aux = new List<BaseWindow>();
            foreach (BaseWindow childWindow in childWindows)
            {
                if (childWindow.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    aux.Add(childWindow);
                }
            }
            childWindows = aux.ToArray();
        }

        protected virtual void OnEnable()
        {
            if (onStartUpWindows != null)
                onStartUpWindows.AddListener(StartUp);
        }

        protected virtual void OnDisable()
        {
            if (onStartUpWindows != null)
                onStartUpWindows.RemoveListener(StartUp);
        }

        private void StartUp()
        {
            if (doOpenOnStart)
                Open(SequenceType.CompleteImmediately);
            else
                Close(SequenceType.CompleteImmediately);
        }
        #endregion

        #region Generic Window Action Methods
        protected virtual Sequence GetSequenceWithControlledChildWindows(UIAnimationSequenceData sequenceData, GameObject target, BaseWindow[] childWindows = null)
        {
            BaseWindow[] controlledChildWindows = childWindows?.Where(window => !(System.Array.IndexOf(exemptions, window) > -1)).ToArray();
            return GetSequence(sequenceData, target, controlledChildWindows);
        }
        
        protected virtual Sequence _WindowAction(
            UIAnimationSequenceData sequenceData,
            SequenceType sequenceType = SequenceType.UnSequenced,
            float atPosition = 0f
        )
        {
            if (sequenceData == null)
                return null;

            Sequence sequence = GetSequenceWithControlledChildWindows(sequenceData, gameObject, childWindows);
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

        /// <summary>
        /// Helper method for getting the open or close sequence for this window
        /// </summary>
        /// <param name="windowAction">Type of WindowAction to the sequence of</param>
        /// <returns></returns>
        public virtual Sequence GetWindowAction(WindowAction windowAction)
        {
            switch (windowAction)
            {
                case (BaseWindow.WindowAction.Open):
                    return _WindowAction(openSequence, SequenceType.UnSequenced, 0f);
                case (BaseWindow.WindowAction.Close):
                    return _WindowAction(closeSequence, SequenceType.UnSequenced, 0f);
                case (BaseWindow.WindowAction.Pulse):
                    return null;
                default:
                    return null;
            }
        }
        #endregion

        #region Helper Methods (formerly UIAnimatorModule)

        #region Create Tween Helpers
        /// <summary>
        /// Using UIAnimatorModule create tween method, gets a tween based on tweenData.animationData.animationType
        /// </summary>
        /// <param name="tweenData">Data to work from</param>
        /// <returns></returns>
        public virtual Tween CreateTweenFromAnimationData(UIAnimationIndividualSequencingData tweenData)
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
        protected virtual Tween CreateTweenFromBaseWindow(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.baseWindow == null) { return null; }

            Tween tween = tweenData.baseWindow.GetWindowAction(tweenData.windowAction);

            tween.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();
            });

            tween.OnComplete(() =>
            {
                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return tween;
        }

        /// <summary>
        /// Analogous to CreateTweenFromBaseWindow, but gets the sequence for the BaseStateMachine's chosen state
        /// </summary>
        /// <param name="tweenData">Data specifying BaseStateMachine and StateToAnimateTo</param>
        /// <returns></returns>
        protected virtual Tween CreateTweenFromBaseStateMachine(UIAnimationIndividualSequencingData tweenData)
        {
            if (tweenData.baseStateMachine == null) { return null; }

            Tween tween = tweenData.baseStateMachine.GetStateMachineAction(tweenData.stateToAnimateTo);

            tween.OnStart(() =>
            {
                if (tweenData.onStartEvent != null)
                    tweenData.onStartEvent.Invoke();
            });

            tween.OnComplete(() =>
            {
                if (tweenData.onCompleteEvent != null)
                    tweenData.onCompleteEvent.Invoke();
            });

            return tween;
        }

        /// <summary>
        /// Gets the desired WindowAction for all BaseWindows in childWindows
        /// And assembles them into a single parent sequence
        /// </summary>
        /// <param name="tweenData">Data specifies how child tweens will be assembled via tweenData.childSequenceType and offset parameters</param>
        /// <param name="childWindows">BaseWindows to work with</param>
        /// <returns></returns>
        protected virtual Tween CreateTweenFromAllChildWindows(UIAnimationIndividualSequencingData tweenData, BaseWindow[] childWindows)
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
        public virtual Tween CreateTween(UIAnimationIndividualSequencingData tweenData, BaseWindow[] childWindows = null)
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
        public virtual Sequence GetSequence(UIAnimationSequenceData animationSequenceData, GameObject gameObject, BaseWindow[] childWindows = null)
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

        #region Window Open Methods
        protected virtual void _Open(
            SequenceType sequenceType = SequenceType.UnSequenced,
            float atPosition = 0f
        )
        {
            _WindowAction(openSequence, sequenceType, atPosition);
            isOpen = true;
        }

        public virtual void Open(SequenceType sequenceType)
        {
            _Open(sequenceType);
        }

        public virtual void Open(float atPosition)
        {
            _Open(SequenceType.Insert, atPosition);
        }

        public virtual void Open()
        {
            _Open(SequenceType.Join);
        }
        #endregion

        #region Window Close Methods
        protected virtual void _Close(
            SequenceType sequenceType = SequenceType.UnSequenced,
            float atPosition = 0f
        )
        {
            _WindowAction(closeSequence, sequenceType, atPosition);
            isOpen = false;
        }

        public virtual void Close(SequenceType sequenceType)
        {
            _Close(sequenceType);
        }

        public virtual void Close(float atPosition)
        {
            _Close(SequenceType.Insert, atPosition);
        }

        public virtual void Close()
        {
            _Close(SequenceType.Join);
        }
        #endregion

        #region Window Toggle Methods
        protected virtual void _Toggle(
            SequenceType sequenceType = SequenceType.UnSequenced,
            float atPosition = 0f
        )
        {
            if (isOpen)
                _Close(sequenceType, atPosition);
            else
                _Open(sequenceType, atPosition);
        }

        public virtual void Toggle(SequenceType sequenceType = SequenceType.UnSequenced)
        {
            _Toggle(sequenceType);
        }

        public virtual void Toggle(float atPosition)
        {
            _Toggle(SequenceType.Insert, atPosition);
        }

        public virtual void Toggle()
        {
            _Toggle(SequenceType.Join);
        }
        #endregion

    }
}


