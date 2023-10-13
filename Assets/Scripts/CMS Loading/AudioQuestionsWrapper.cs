using Newtonsoft.Json;
using System.Collections.Generic;

namespace JoshKery.York.AudioRecordingBooth
{
    [System.Serializable]
    public class AudioQuestionsWrapper
    {
        [JsonProperty("audio_questions")]
        public List<AudioQuestion> AudioQuestions;
    }

    [System.Serializable]
    public class AudioQuestion
    {
        [JsonProperty("question_id")]
        public int id;

        [JsonProperty("question_text")]
        public string text;
    }
}


