using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace JoshKery.GenericUI.ContentLoading
{
    public class LocalConfigLoader : ContentLoader
    {
        [System.Serializable]
        public class ConfigJSON
        {

        }

        protected override IEnumerator LoadLocalContentSuccess(string text)
        {
            ConfigJSON configData = JsonConvert.DeserializeObject<ConfigJSON>(text);

            if (configData == null)
                yield break;

            Debug.Log(text);
        }

        protected override IEnumerator LoadLocalContentFinally(UnityWebRequest.Result result)
        {
            yield break;
        }

    }
}




