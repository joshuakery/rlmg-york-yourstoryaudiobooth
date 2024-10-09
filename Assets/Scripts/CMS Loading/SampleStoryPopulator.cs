using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace JoshKery.York.AudioRecordingBooth
{
    public class SampleStoryPopulator : MonoBehaviour
    {
        [SerializeField]
        private AudioQuestionsLoader contentLoader;

        public GraphQLResponseWrapper ResponseWrapper;

        [SerializeField]
        private AudioClipLoader sampleStoryLoader1;

        [SerializeField]
        private AudioClipLoader sampleStoryLoader2;

        [SerializeField]
        private string sampleStory1FilePath;

        [SerializeField]
        private string sampleStory2FilePath;

        private void OnEnable()
        {
            if (contentLoader != null)
            {
                contentLoader.onPopulateContentFinish.AddListener(OnPopulateContentFinish);
            }
        }

        private void OnDisable()
        {
            if (contentLoader != null)
            {
                contentLoader.onPopulateContentFinish.RemoveListener(OnPopulateContentFinish);
            }
        }
        public IEnumerator PopulateContent(string text)
        {
            ResponseWrapper = JsonConvert.DeserializeObject<GraphQLResponseWrapper>(text);

            // download the files
            if (ResponseWrapper?.data?.audioQuestions == null) { yield break; }

            if (ResponseWrapper.data.audioQuestions.play_story_1 != null)
            {
                string playStory1OnlinePath = contentLoader.assetsURL + "/" + ResponseWrapper.data.audioQuestions.play_story_1.filename_disk;

                sampleStory1FilePath = contentLoader.GetLocalMediaPath(ResponseWrapper.data.audioQuestions.play_story_1.filename_download);

                yield return StartCoroutine(contentLoader.SaveMediaToDisk(playStory1OnlinePath, sampleStory1FilePath, false));
            }

            if (ResponseWrapper.data.audioQuestions.play_story_2 != null)
            {
                string playStory2OnlinePath = contentLoader.assetsURL + "/" + ResponseWrapper.data.audioQuestions.play_story_2.filename_disk;

                sampleStory2FilePath = contentLoader.GetLocalMediaPath(ResponseWrapper.data.audioQuestions.play_story_2.filename_download);

                yield return StartCoroutine(contentLoader.SaveMediaToDisk(playStory2OnlinePath, sampleStory2FilePath, false));
            }

            yield return SetSampleStories();
        }

        private IEnumerator SetSampleStories()
        {
            if (sampleStoryLoader1 != null)
                yield return StartCoroutine(sampleStoryLoader1.ReloadCo(null, sampleStory1FilePath));

            if (sampleStoryLoader2 != null)
                yield return StartCoroutine(sampleStoryLoader2.ReloadCo(null, sampleStory2FilePath));
        }

        private void OnPopulateContentFinish()
        {
            // todo put animations in start states
        }
    }
}


