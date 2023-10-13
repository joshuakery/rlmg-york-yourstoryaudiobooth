using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class RecordingPage : GenericPage
    {
        private AudioRecorderProcess process;

        protected override void Awake()
        {
            base.Awake();

            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();
        }

        protected override void _Open(SequenceType sequenceType = SequenceType.UnSequenced, float atPosition = 0)
        {
            if (process != null)
                process.Init();

            base._Open(sequenceType, atPosition);
        }

        public override void Close()
        {
            if (process != null)
                process.CancelTask();

            base.Close();
        }
    }
}


