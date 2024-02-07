using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    public class Emailer : MonoBehaviour
    {
        /// <summary>
        /// Settings for sending email.
        /// </summary>
        public class EmailSettings
        {
            /// <summary>
            /// Visitor-input email address
            /// </summary>
            public string address;

            public string firstName;
            public string lastName;

            /// <summary>
            /// Path to saved recording of visitor
            /// </summary>
            public string attachmentPath;

            public bool doSubscribe;

            /// <summary>
            /// Visitor response to 13 or older question. Will be inverse of MainSubmissionHandler.doSaveData
            /// </summary>
            public bool isMinor;

            /// <summary>
            /// Visitor-chosen question prompt
            /// </summary>
            public string prompt;
            
            public EmailSettings(string a, string f, string l, string p, bool sub, bool isM, string pr)
            {
                address = a;
                firstName = f;
                lastName = l;
                attachmentPath = p;
                doSubscribe = sub;
                isMinor = isM;
                prompt = pr;
            }
        }

        /// <summary>
        /// Time (in milliseconds) after which email Task will timeout. Passed to task.Wait()
        /// </summary>
        public int EMAILTIMEOUT = 3000;

        /// <summary>
        /// Aux value used to update emailTimeoutWait without recreating every Send
        /// </summary>
        private int lastTimeout = 0;

        /// <summary>
        /// Wait before timeout cancels SendEmail POST request
        /// </summary>
        private WaitForSeconds emailTimeoutWait;

        /// <summary>
        /// SendEmail POST request endpoint
        /// </summary>
        public string EMAILAPIENDPOINT = "";

        /// <summary>
        /// SendEmail POST request bearer token
        /// </summary>
        public string TOKEN = "";
        
        public delegate void OnFailEvent(string message);

        /// <summary>
        /// Event invoked when an exception is thrown during the email task.
        /// </summary>
        public OnFailEvent onFail;

        public delegate void OnSuccessEvent();

        /// <summary>
        /// Event invoked when the email task completes successfully.
        /// </summary>
        public OnSuccessEvent onSuccess;

        /// <summary>
        /// Is SendEmail coroutine underway? For UI elements watching Emailer
        /// </summary>
        public bool isSending = false;

        private void Awake()
        {
            emailTimeoutWait = new WaitForSeconds(EMAILTIMEOUT / 1000f);
            lastTimeout = EMAILTIMEOUT;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                OnCancelSendEmail();
        }

        /// <summary>
        /// Signals core email Action to cancel gracefully.
        /// </summary>
        public void OnCancelSendEmail()
        {
            RLMGLogger.Instance.Log("Cancelling send email...", MESSAGETYPE.INFO);
            StopSendEmail();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopSendEmail()
        {
            StopAllCoroutines();
            isSending = false;
        }

        /// <summary>
        /// Starts SendEmail coroutine.
        /// </summary>
        /// <param name="settings"></param>
        public void StartSendEmail(EmailSettings settings)
        {
            StopSendEmail();
            StartCoroutine(Timeout());
            StartCoroutine(SendEmail(settings));
        }

        /// <summary>
        /// Creates POST Multipart Form and sends UnityWebRequest
        /// </summary>
        /// <param name="settings">Used to create form</param>
        /// <returns></returns>
        private IEnumerator SendEmail(EmailSettings settings)
        {
            isSending = true;

            var form = new List<IMultipartFormSection>
                        {
                            new MultipartFormFileSection( "audio",      File.ReadAllBytes( settings.attachmentPath), Path.GetFileName(settings.attachmentPath), "audio/mpeg" ),
                            new MultipartFormDataSection( "email",      Encoding.ASCII.GetBytes( settings.address ) ),
                            new MultipartFormDataSection( "question",   Encoding.ASCII.GetBytes( settings.prompt ) ),
                            new MultipartFormDataSection( "isMinor",    Encoding.ASCII.GetBytes( settings.isMinor ? "true" : "false" ) )
                        };

            UnityWebRequest request = UnityWebRequest.Post(EMAILAPIENDPOINT, form);

            request.SetRequestHeader("Authorization", "Bearer " + TOKEN);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                RLMGLogger.Instance.Log(request.error.ToString() + "\n" + request.downloadHandler.text, MESSAGETYPE.ERROR);
                onFail?.Invoke(request.error.ToString() + "\n" + request.downloadHandler.text);
            }
            else
            {
                RLMGLogger.Instance.Log(String.Format("Email completed. Response: {0}", request.downloadHandler.text), MESSAGETYPE.INFO);
                onSuccess?.Invoke();
            }

            isSending = false;

            // Stop the timeout coroutine
            StopAllCoroutines();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator Timeout()
        {
            if (lastTimeout != EMAILTIMEOUT)
            {
                emailTimeoutWait = new WaitForSeconds(EMAILTIMEOUT / 1000f);
                lastTimeout = EMAILTIMEOUT;
            }

            yield return emailTimeoutWait;

            RLMGLogger.Instance.Log("Email timeout. Cancelling send email...", MESSAGETYPE.ERROR);
            StopSendEmail();
        }
    }
}


