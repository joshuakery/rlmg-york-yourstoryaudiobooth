using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;
using UnityEngine.Networking;

namespace JoshKery.GenericUI.ContentLoading.Airtable
{
    public abstract class AirtableErrorDisplayManager<T> : BaseDisplay
    {
        [SerializeField]
        private AirtableLoader<T> airtableLoader;

        private void OnEnable()
        {
            if (airtableLoader != null)
            {
                airtableLoader.onAirtableResponseFail += InstantiateErrorDisplay;
                airtableLoader.onAirtableResponseFinish += InstantiateSuccessDisplay;
            }
            
        }

        private void OnDisable()
        {
            if (airtableLoader != null)
            {
                airtableLoader.onAirtableResponseFail -= InstantiateErrorDisplay;
                airtableLoader.onAirtableResponseFinish -= InstantiateSuccessDisplay;
            }
        }

        protected void InstantiateSuccessDisplay(List<BaseRecord<T>> records)
        {
            if (records != null)
                InstantiateDisplay(
                    "Finished request to airtable.",
                    string.Format("{0} records found.", records.Count),
                    "",
                    AirtableErrorDisplay.LogType.success
                );
            else
                InstantiateDisplay(
                    "Finished request to airtable.",
                    "Null records were returned.",
                    "",
                    AirtableErrorDisplay.LogType.error
                );
        }

        protected void InstantiateErrorDisplay(AirtableUnity.PX.Response response, UnityWebRequest request)
        {
            string fValue = response.Err.message;
            foreach (string error in response.Err.errors)
            {
                fValue += "\n" + error;
            }

            InstantiateDisplay(response.Message, fValue, request?.url, AirtableErrorDisplay.LogType.error);
        }

        protected void InstantiateDisplay(string message, string fValue, string uri = "", AirtableErrorDisplay.LogType logType = 0)
        {
            AirtableErrorDisplay errorDisplay = InstantiateDisplay<AirtableErrorDisplay>();
            if (errorDisplay != null)
            {
                errorDisplay.SetContent(message, fValue, uri, logType);
            }
        }
    }
}

