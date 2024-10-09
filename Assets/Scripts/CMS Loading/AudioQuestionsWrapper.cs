using Newtonsoft.Json;
using System.Collections.Generic;

namespace JoshKery.York.AudioRecordingBooth
{
    [System.Serializable]
    public class GraphQLResponseWrapper
    {
        [JsonProperty("data")]
        public DataWrapper data { get; set; }
    }

    [System.Serializable]
    public class DataWrapper
    {
        [JsonProperty("YourStoryAudioBooth")]
        public AudioQuestionsWrapper audioQuestions { get; set; }
    }

    [System.Serializable]
    public class AudioQuestionsWrapper
    {
        [JsonProperty("question_one")]
        public string question_one { get; set; }

        [JsonProperty("question_two")]
        public string question_two { get; set; }

        [JsonProperty("question_three")]
        public string question_three { get; set; }

        [JsonProperty("question_four")]
        public string question_four { get; set; }

        [JsonProperty("question_five")]
        public string question_five { get; set; }

        [JsonProperty("question_six")]
        public string question_six { get; set; }

        [JsonProperty("play_story_1")]
        public MediaFile play_story_1 { get; set; }

        [JsonProperty("play_story_2")]
        public MediaFile play_story_2 { get; set; }

        public string GetPrompt(int index)
        {
            switch (index)
            {
                case 0:
                    return question_one;
                case 1:
                    return question_two;
                case 2:
                    return question_three;
                case 3:
                    return question_four;
                case 4:
                    return question_five;
                case 5:
                    return question_six;
                default:
                    return null;
            }
        }

        public int GetIndex(string prompt)
        {
            if (prompt == question_one)
                return 0;
            if (prompt == question_two)
                return 1;
            if (prompt == question_three)
                return 2;
            if (prompt == question_four)
                return 3;
            if (prompt == question_five)
                return 4;
            if (prompt == question_six)
                return 5;
            return -1;
        }
    }

    [System.Serializable]
    public class MediaFile
    {
        #region Graph Properties
        [JsonProperty("filename_download")]
        public string filename_download { get; set; }

        [JsonProperty("filename_disk")]
        public string filename_disk { get; set; }
        #endregion

        #region Other Fields
        [JsonIgnore]
        public string path_download;

        [JsonIgnore]
        public string local_path;
        #endregion
    }


}


