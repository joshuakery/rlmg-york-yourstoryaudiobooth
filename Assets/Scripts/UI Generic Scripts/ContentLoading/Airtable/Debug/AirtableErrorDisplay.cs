using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AirtableUnity.PX;
using UnityEngine.Networking;

namespace JoshKery.GenericUI.ContentLoading.Airtable
{
    public class AirtableErrorDisplay : BaseDisplay
    {
        public enum LogType
        {
            success = 0,
            error = 1,
            warn = 2
        }

        public static Color32 warnColor;
        public static Color32 errorColor = new Color32(205, 132, 132, 255);
        public static Color32 successColor = new Color32(132, 205, 145, 255);

        [Header("Display Fields")]
        [SerializeField]
        private TMP_Text message;
        [SerializeField]
        private TMP_Text requestURI;
        [SerializeField]
        private TMP_Text errors;

        [SerializeField]
        private Image background;

        protected override void Awake()
        {
            base.Awake();

            if (background == null)
                background = GetComponent<Image>();
            

            message.text = "";
            requestURI.text = "";
            errors.text = "";
        }

        //public virtual void SetContent(Response response, UnityWebRequest request = null)
        //{
        //    message.text = response.Message;

        //    if (request != null)
        //    {
        //        requestURI.text = request.url;
        //    }

        //    errors.text = response.Err.message;
        //    foreach (string error in response.Err.errors)
        //    {
        //        errors.text += "\n" + error;
        //    }
        //}

        public virtual void SetContent(
            string messageText,
            string errorsText,
            string uri,
            LogType logType
        )
        {
            message.text = messageText;
            errors.text = errorsText;
            requestURI.text = uri;

            if (background != null)
            {
                switch (logType)
                {
                    case LogType.success:
                        background.color = successColor;
                        break;
                    case LogType.error:
                        background.color = errorColor;
                        break;
                    case LogType.warn:
                        background.color = warnColor;
                        break;
                    default:
                        break;
                }
            }

        }
    }
}


