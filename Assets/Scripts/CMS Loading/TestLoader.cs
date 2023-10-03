using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GraphQlClient.Core;
using JoshKery.GenericUI.ContentLoading;
using UnityEngine.Networking;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TestLoader : ContentLoader
    {
        /// <summary>
        /// URL to which the query will be posted.
        /// </summary>
        public string graphQLURL;

        /// <summary>
        /// Any authentication token needed for the query.
        /// </summary>
        public string authToken;

        /// <summary>
        /// The operation name to post with the query.
        /// </summary>
        public string operationName;

        /// <summary>
        /// The query. See example below.
        /// </summary>
        [Multiline]
        public string query;

        /*
        query IntroMedia {
        introMedia {
            media {
              directus_files_id {
                filename_disk
              }
            }
          }
        }
        */
        

        public delegate IEnumerator PopulateContentEvent(string json);

        /// <summary>
        /// Callback event called after successful loading.
        /// Other MonoBehaviours might subscribe to this.
        /// </summary>
        public PopulateContentEvent onPopulateContent;

        #region Graphql Loading
        /// <summary>
        /// Override the target content loader with loading from graphql
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator PriorityLoadContent()
        {
            yield return LoadGraphContent().AsIEnumerator();
        }

        /// <summary>
        /// Post the graphql query and handle response
        /// </summary>
        /// <returns></returns>
        private async Task LoadGraphContent()
        {
            //just showing that variables might be passed this way
            //object variables = new { erasByIdId = erasByIdIdVariable };
            object variables = new { };

            UnityWebRequest request = await HttpHandler.PostAsync(graphQLURL, query, variables, operationName, authToken);

            if (request.result != UnityWebRequest.Result.Success)
            {
                await GraphResponseFail(request);
            }
            else
            {
                await GraphResponseSuccess(request.downloadHandler.text);
            }
        }

        /// <summary>
        /// Callback for graphql request success
        /// </summary>
        /// <param name="text">The response text</param>
        /// <returns></returns>
        private async Task GraphResponseSuccess(string text)
        {
            await OnAnySuccess(text);
        }

        /// <summary>
        /// Callback for graphql request error
        /// </summary>
        /// <param name="request">The failed request</param>
        /// <returns></returns>
        private async Task GraphResponseFail(UnityWebRequest request)
        {
            RLMGLogger.Instance.Log(
                string.Format("Graph response error! Error message: {0}", request.error),
                MESSAGETYPE.ERROR
            );

            // TODO UI display of error handling and option to try again

            RLMGLogger.Instance.Log(
                "Falling back to locally saved content...",
                MESSAGETYPE.INFO
            );

            await LoadLocalContent();
        }
        #endregion

        #region Local loading implementation
        /// <summary>
        /// Callback for loading locally success
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override IEnumerator LoadLocalContentSuccess(string text)
        {
            yield return StartCoroutine(OnAnySuccess(text));
        }

        /// <summary>
        /// Callback for loading locally finally
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override IEnumerator LoadLocalContentFinally(UnityWebRequest.Result result)
        {
            // do nothing
            yield return null;
        }
        #endregion

        private IEnumerator OnAnySuccess(string text)
        {
            int n = 50;
            RLMGLogger.Instance.Log(
                string.Format("Content loaded successfully! First {0} chars of response: {1}", n, text.Substring(0, n)),
                MESSAGETYPE.INFO
            );

            SaveContentFileToDisk(text);

            if (onPopulateContent != null)
                yield return StartCoroutine(onPopulateContent(text));

            onPopulateContentFinish.Invoke();
        }

    }
}


