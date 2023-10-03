using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.ContentLoading;
using UnityEngine.Networking;

namespace JoshKery.York.AudioRecordingBooth
{
    [RequireComponent(typeof(EmailValidator))]
    public class ValidationRegexLoader : ContentLoader
    {
        private EmailValidator emailValidator;

        protected override void Awake()
        {
            base.Awake();

            emailValidator = GetComponent<EmailValidator>();
        }
        protected override IEnumerator LoadLocalContentFinally(UnityWebRequest.Result result)
        {
            yield return null;
            //do nothing
        }

        protected override IEnumerator LoadLocalContentSuccess(string text)
        {
            yield return null;

            /*if (emailValidator != null)
                emailValidator.MatchGenericEmailPattern = text;*/
        }
    }
}


