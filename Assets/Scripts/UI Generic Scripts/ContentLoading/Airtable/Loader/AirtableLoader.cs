using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AirtableUnity.PX.Model;
using Newtonsoft.Json;
using rlmg.logging;

namespace JoshKery.GenericUI.ContentLoading.Airtable
{
    public abstract class AirtableLoader<T> : ContentLoader
    {
        public delegate void OnAirtableResponseSuccess(List<BaseRecord<T>> records);
        public OnAirtableResponseSuccess onAirtableResponseSuccess;

        public delegate void OnAirtableResponseFail(AirtableUnity.PX.Response response, UnityWebRequest request);
        public OnAirtableResponseFail onAirtableResponseFail;

        public delegate void OnAirtableResponseFinish(List<BaseRecord<T>> records);
        public OnAirtableResponseFinish onAirtableResponseFinish;

        public string DefaultTableName;
        public string DefaultFilterByFormula;

        [SerializeField]
        private AirtableLocalConfigLoader airtableLocalConfigLoader;

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();

            if (airtableLocalConfigLoader == null)
                airtableLocalConfigLoader = FindObjectOfType<AirtableLocalConfigLoader>();
        }

        protected virtual void OnEnable()
        {
            if (airtableLocalConfigLoader != null)
                airtableLocalConfigLoader.onPopulateContentFinish.AddListener(LoadContent);
        }

        protected virtual void OnDisable()
        {
            if (airtableLocalConfigLoader != null)
                airtableLocalConfigLoader.onPopulateContentFinish.RemoveListener(LoadContent);
        }
        #endregion

        protected override IEnumerator PriorityLoadContent()
        {
            yield return LoadAirtableContent();
        }

        #region Load Airtable Content Helpers
        protected virtual IEnumerator LoadAirtableContent()
        {
            var recordsToReturn = new List<BaseRecord<T>>();
            string curOffset = "";

            do
            {
                using (UnityWebRequest request = AirtableUnity.PX.Proxy.ListRecords(DefaultTableName, curOffset, DefaultFilterByFormula))
                {
                    yield return request.SendWebRequest();
                    AirtableUnity.PX.Response response = AirtableUnity.PX.Proxy.GetResponse(request);

                    if (response.Success)
                    {
                        try
                        {
                            var recordsFound = response?.GetAirtableData<T>()?.records;

                            if (recordsFound?.Count > 0)
                                recordsToReturn.AddRange(recordsFound);

                            curOffset = response?.GetAirtableData<T>()?.offset;
                        }
                        catch (Exception e)
                        {
                            RLMGLogger.Instance.Log(response.Message, MESSAGETYPE.ERROR);
                            RLMGLogger.Instance.Log(e.ToString(), MESSAGETYPE.ERROR);
                        }

                        yield return StartCoroutine(AirtableResponseSuccess(recordsToReturn));
                    }
                    else
                    {
                        RLMGLogger.Instance.Log(response.Message, MESSAGETYPE.ERROR);
                        yield return StartCoroutine(AirtableResponseFail(response, request));
                    }
                }
            } while (!string.IsNullOrEmpty(curOffset));

            yield return StartCoroutine(AirtableResponseFinish(recordsToReturn));
        }

        protected virtual IEnumerator AirtableResponseSuccess(List<BaseRecord<T>> records)
        {
            if (onAirtableResponseSuccess != null)
                onAirtableResponseSuccess.Invoke(records);

            yield return null;

            string text = JsonConvert.SerializeObject(records, Formatting.Indented);
            SaveContentFileToDisk(text);
        }

        protected virtual IEnumerator AirtableResponseFail(AirtableUnity.PX.Response response, UnityWebRequest request)
        {
            if (onAirtableResponseFail != null)
                onAirtableResponseFail.Invoke(response, request);

            yield return null;

            StartCoroutine(LoadLocalContent());
        }

        protected virtual IEnumerator AirtableResponseFinish(List<BaseRecord<T>> records)
        {
            if (onAirtableResponseFinish != null)
                onAirtableResponseFinish.Invoke(records);

            yield return null;

            if (onPopulateContentFinish != null)
                onPopulateContentFinish.Invoke();
        }

        
        #endregion

        #region Updating Airtable Fields
        public IEnumerator UpdateRecordCo(
            string tableName,
            string recordId,
            string newData,
            Action<BaseRecord<T>> callback,
            bool useHardUpdate = false
        )
        {
            yield return StartCoroutine(AirtableUnity.PX.Proxy.UpdateRecordCo<T>(tableName, recordId, newData,
                (baseRecordUpdated) =>
                {
                    callback?.Invoke(baseRecordUpdated);
                }, useHardUpdate));
        }
        #endregion


    }
}


