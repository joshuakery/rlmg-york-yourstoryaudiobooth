using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    [System.Serializable]
    public enum SequenceType
    {
        CompleteImmediately = -2,
        UnSequenced = -1,
        Join = 0,
        Append = 1,
        Insert = 2,
        BackInsert = 3
    }

    [SerializeField]
    public enum SequenceSource
    {
        AnimationData = 0,
        BaseWindow = 1,
        AllChildWindows = 2,
        BaseStateMachine = 3
    }

    [System.Serializable]
    public class UIAnimationIndividualSequencingData
    {
        public SequenceSource sequenceSource;

        //AnimationData Source
        [Header("If Animation Data Source")]
        public UIAnimationData animationData;
        public GameObject objectToAnimate;

        public UnityEvent onStartEvent;
        public UnityEvent onCompleteEvent;

        public bool doOffsetFromStartOfLastTween = false;
        public float offset = 0f;

        //BaseWindow or AllChildWindows Source
        [Header("If Base Window Source")]
        public BaseWindow baseWindow;
        public BaseWindow.WindowAction windowAction;

        //AllChildWindows Source
        [Header("If All Child Windows Source")]
        public SequenceType childSequenceType;
        public bool doOffsetChildrenFromTheirStarts = false;
        public float childOffset = 0f;
        

        [Header("If Base State Machine Source")]
        public BaseStateMachine baseStateMachine;
        public int stateToAnimateTo = 0;

    }

    [System.Serializable]
    public class UIAnimationSequenceData
    {
        public SequenceType sequenceType = SequenceType.Join;

        public List<UIAnimationIndividualSequencingData> individualSequencingData;

        public UnityEvent onStartEvent;
        public UnityEvent onCompleteEvent;

        public UIAnimationSequenceData(
            SequenceType s,
            List<UIAnimationIndividualSequencingData> iSD,
            UnityEvent oS = null,
            UnityEvent oC = null
        )
        {
            sequenceType = s;
            individualSequencingData = iSD;
            onStartEvent = oS;
            onCompleteEvent = oC;
        }
    }
}

