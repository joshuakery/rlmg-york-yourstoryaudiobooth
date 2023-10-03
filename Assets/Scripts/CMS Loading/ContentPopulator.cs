using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace JoshKery.York.AudioRecordingBooth
{
    public class ContentPopulator : MonoBehaviour
    {
        [SerializeField]
        private AudioQuestionsLoader contentLoader;

        public AudioQuestions audioQuestions;

        private void OnEnable()
        {
            if (contentLoader != null)
            {
                contentLoader.onPopulateContent += PopulateContent;
                contentLoader.onPopulateContentFinish.AddListener(OnPopulateContentFinish);
            }
                
        }

        private void OnDisable()
        {
            if (contentLoader != null)
            {
                contentLoader.onPopulateContent -= PopulateContent;
                contentLoader.onPopulateContentFinish.RemoveListener(OnPopulateContentFinish);
            }
        }

        private IEnumerator PopulateContent(string text)
        {
            yield return null;

            audioQuestions = JsonConvert.DeserializeObject<AudioQuestions>(text);

            // todo invoke set content methods
        }

        private void OnPopulateContentFinish()
        {
            // todo put animations in start states
        }
    }
}


