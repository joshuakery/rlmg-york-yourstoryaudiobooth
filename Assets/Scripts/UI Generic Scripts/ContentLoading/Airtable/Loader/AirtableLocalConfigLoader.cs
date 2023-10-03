using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace JoshKery.GenericUI.ContentLoading.Airtable
{
    public class AirtableLocalConfigLoader : ContentLoader
    {
        [Header("Default Environment Configuration")]
        [SerializeField]
        protected string DefaultApiVersion;
        [SerializeField]
        protected string DefaultAppKey;
        [SerializeField]
        protected string DefaultApiKey;

        [SerializeField]
        private ContentLoader airtableLoader;

        [System.Serializable]
        public class AirtableLocalConfigJSON
        {
            public bool doDefaultLocalLoadContent = false;
            public string ApiVersion;
            public string AppKey;
            public string ApiKey;
        }

        protected override IEnumerator LoadLocalContentSuccess(string text)
        {
            AirtableLocalConfigJSON configData = JsonConvert.DeserializeObject<AirtableLocalConfigJSON>(text);

            if (configData == null)
                yield break;

            if (!string.IsNullOrEmpty(configData.ApiVersion) &&
                !string.IsNullOrEmpty(configData.AppKey) &&
                !string.IsNullOrEmpty(configData.ApiKey)
                )
            {
                AirtableUnity.PX.Proxy.SetEnvironment(
                    configData.ApiVersion,
                    configData.AppKey,
                    configData.ApiKey
                );
            }
            else
            {
                AirtableUnity.PX.Proxy.SetEnvironment(
                    DefaultApiVersion,
                    DefaultAppKey,
                    DefaultApiKey
                );
            }

            if (airtableLoader != null)
            {
                airtableLoader.doDefaultLocalLoadContent = configData.doDefaultLocalLoadContent;
            }

            onPopulateContentFinish.Invoke();
        }

        protected override IEnumerator LoadLocalContentFinally(UnityWebRequest.Result result)
        {
            yield return null;
        }

    }
}


