using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class Emailer : MonoBehaviour
    {
        public class EmailSettings
        {
            public string address;

            public string firstName;
            public string lastName;

            public string attachmentPath;

            public Action<string> onFailWrapper;
            public Action onSuccessWrapper;
            
            public EmailSettings(string a, string f, string l, string p)
            {
                address = a;
                firstName = f;
                lastName = l;
                attachmentPath = p;
            }
        }


        /// <summary>
        /// Time (in milliseconds) after which email Task will timeout. Passed to task.Wait()
        /// </summary>
        public int EMAILTIMEOUT = 3000;

        /// <summary>
        /// For issuing a cancel command.
        /// </summary>
        private CancellationTokenSource currentTokenSource;

        /// <summary>
        /// The Task currently underway.
        /// </summary>
        public Task currentTask;

        
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                OnCancelTask();
        }

        /// <summary>
        /// Signals core email Action to cancel gracefully.
        /// </summary>
        public void OnCancelTask()
        {
            if (currentTokenSource != null)
            {
                UnityEngine.Debug.Log("Cancelling email...");
                currentTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Runs Main email task.
        /// Sets up synchronization context for callback events
        /// </summary>
        /// <param name="settings"></param>
        public void Run(EmailSettings settings)
        {
            var syncContext = SynchronizationContext.Current;
            settings.onFailWrapper = new Action<string>(message => syncContext.Post(_ => onFail(message), null));
            settings.onSuccessWrapper = new Action(() => syncContext.Post(_ => onSuccess(), null));

            Task.Run(() => Main(settings));
        }

        /// <summary>
        /// Main email task.
        /// Sets up cancellation token.
        /// Wraps around core Action and handles exceptions and success.
        /// </summary>
        /// <param name="settings"></param>
        private void Main(EmailSettings settings)
        {
            currentTokenSource = new CancellationTokenSource();

            currentTask = Task.Run(() => SendEmail(settings, currentTokenSource.Token), currentTokenSource.Token);

            try
            {
                if (!currentTask.Wait(EMAILTIMEOUT))
                {
                    //Signal to cancel
                    currentTokenSource.Cancel();

                    //Now that we've cancelled, wait for the task to elegantly complete.
                    currentTask.Wait();

                    throw new TimeoutException();
                }
                else
                {
                    settings.onSuccessWrapper();
                }
            }
            catch (AggregateException e)
            {
                Debug.LogError(e.ToString());
                if (e.InnerException.GetType() == typeof(TaskCanceledException))
                {
                    settings.onFailWrapper("Email cancelled.");
                }
                else
                {
                    settings.onFailWrapper(e.ToString());
                }
            }
            catch (TimeoutException e)
            {
                Debug.LogError(e.ToString());
                settings.onFailWrapper("Sending the email timed out.");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                settings.onFailWrapper(e.ToString());
            }
            finally
            {
                currentTokenSource.Dispose();
            }
        }

        /// <summary>
        /// Core email Action
        /// with graceful cancellation
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="token"></param>
        private void SendEmail(EmailSettings settings, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                Debug.LogError("Email cancelled before it got started.");
                token.ThrowIfCancellationRequested();
            }

            // todo replace with actual email sending
            for (int i=0; i<10; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.LogError("Email cancelled while underway.");
                    token.ThrowIfCancellationRequested();
                }
                
                Thread.Sleep(500);
            }

            Debug.Log("Email completed.");
        }
    }
}


