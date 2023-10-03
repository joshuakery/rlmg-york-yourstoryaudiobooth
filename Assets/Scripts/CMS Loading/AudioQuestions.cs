using Newtonsoft.Json;
using System.Collections.Generic;

namespace JoshKery.York.AudioRecordingBooth
{
    [System.Serializable]
    public class AudioQuestions
    {
        [JsonProperty("audio_questions")]
        public List<AudioQuestion> audioQuestions;
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


