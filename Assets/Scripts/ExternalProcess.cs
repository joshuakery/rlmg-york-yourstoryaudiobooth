using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using UnityEngine;


//https://www.learncsharptutorial.com/threadpooling-csharp-example.php
//https://jonskeet.uk/csharp/threads/threadpool.html

//Sending writeline commands for standardinput
//https://stackoverflow.com/questions/70059130/continuously-reading-output-stdout-from-process-in-c-accessing-multiple-time

//----> https://stackoverflow.com/questions/58363535/c-sharp-execute-threading-threadpool-queueuserworkitem-callback-in-main-thread-a

namespace JoshKery.York.AudioRecordingBooth
{
	public class ExternalProcess : MonoBehaviour
	{
		/// <summary>
		/// Timeout for ExternalProcessAction Task.
		/// Milliseconds.
		/// </summary>
		private const int DEFAULTTIMEOUT = 60000;


		/// <summary>
		/// Default executable file to use in the process if none is specified
		/// </summary>
		public string defaultProcessFileName;

		public delegate void OnFailEvent(Exception e);

		/// <summary>
		/// Event invoked when an exception is thrown during the Main Task.
		/// </summary>
		public OnFailEvent onFail;

		public delegate void OnSuccessEvent();

		/// <summary>
		/// Event invoked when the Main Task completes successfully.
		/// </summary>
		public OnSuccessEvent onSuccess;

		public delegate void OnFirstProcessStartedEvent();

		/// <summary>
		/// 
		/// </summary>
		public OnFirstProcessStartedEvent onFirstProcessStarted;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exitCodes">Exit codes from processes</param>
		public delegate void OnAllProcessSuccessEvent(int[] exitCodes);

		/// <summary>
		/// 
		/// </summary>
		public OnAllProcessSuccessEvent onAllProcessSuccess;

		public delegate void InitEvent();
		public InitEvent onInit;

		/// <summary>
		/// When signalled, process will continue, issue its exit command, and WaitForExit.
		/// </summary>
		protected static AutoResetEvent standardInputEvent = new AutoResetEvent(false);

		protected CancellationTokenSource currentTokenSource = null;

		/// <summary>
		/// The Task currently underway.
		/// </summary>
		public Task currentTask;

		private readonly object currentProcessLock = new object();
		protected int currentProcess = -1;

		public bool isRunning
		{
			get
			{
				return currentTask != null && !currentTask.IsCompleted;
			}
		}

		void OnDestroy()
		{
			// TODO Clean things up?
		}

        private void OnApplicationQuit()
        {
			if (isRunning)
				UnityEngine.Debug.Log("Application quit while recording underway. Attempting to cancel...");

			CancelTask();
        }

        public void OnCancelTask()
		{
			UnityEngine.Debug.Log("User issued command to cancel external process...");
			CancelTask();
		}

		public void CancelTask()
        {
			if (currentTokenSource != null)
			{
				currentTokenSource.Cancel();
				standardInputEvent.Set();
			}
		}

		/// <summary>
		/// Runs Main external process Task.
		/// Sets up cancellation token.
		/// Sets up SynchronizationContext and callback wrappers.
		/// Sets up exitEvent.
		/// </summary>
		/// <param name="settings">Settings which will be passed to ExternalProcessActionparam>
		public void Run(Settings settings, int timeout = DEFAULTTIMEOUT)
		{
			if (settings == null) return;
			if (string.IsNullOrEmpty(settings.process)) settings.process = defaultProcessFileName;
			if (!settings.isValid) return;

			// Setup SynchronizationContext for use in calling callbacks on main Unity thread
			var syncContext = SynchronizationContext.Current;

			// invoked when Main Task raises an exception
			settings.onFailWrapper = new Action<Exception>(e => { syncContext.Post(_ => OnFail(e), null); });

			// invoked when Main Task completes successfully
			settings.onSuccessWrapper = new Action(() => syncContext.Post(_ => OnSuccess(), null));

			settings.onFirstProcessStartedWrapper = new Action(() => syncContext.Post(_ => OnFirstProcessStarted(), null));

			// invoked when ExternalProcessTask completes successfully
			settings.onAllProcessFinishedWrapper = new Action<int[]>(
					exitCodes => syncContext.Post(_ => OnAllProcessSuccess(exitCodes), null)
				);


			// Assign autoResetEvent to wait on if there are any exit commands for a command
			settings.standardInputEvent = standardInputEvent;

			// Setup cancellation token
			currentTokenSource = new CancellationTokenSource();
			settings.ctoken = currentTokenSource.Token;

			currentTask = Task.Run( () => Main(settings, timeout), settings.ctoken );
		}

		private void Main(Settings settings, int timeout = DEFAULTTIMEOUT)
        {
			if (settings == null) return;
			if (string.IsNullOrEmpty(settings.process)) settings.process = defaultProcessFileName;
			if (!settings.isValid) return;

			Task mainTask = Task.Run(() => ExternalProcessAction(settings), settings.ctoken);

			try
			{
				if (!mainTask.Wait(timeout, settings.ctoken))
				{
					//Signal to cancel
					currentTokenSource.Cancel();
					settings.standardInputEvent.Set();

					//Now that we've cancelled, wait for the task to elegantly complete.
					mainTask.Wait();

					throw new TimeoutException();
				}
				else
				{
					settings.onSuccessWrapper();
				}
			}
			catch (OperationCanceledException e)
            {
				UnityEngine.Debug.LogError(e.ToString());
				settings.onFailWrapper(e);
			}
			catch (AggregateException ae)
			{
				ae.Handle((x) =>
				{
					if (x is TaskCanceledException)
                    {
						UnityEngine.Debug.LogError(x.ToString());
						settings.onFailWrapper(x);
						return true;
					}
					else
                    {
						UnityEngine.Debug.LogError("Unknown exception in Main Task: " + x.ToString());
						settings.onFailWrapper(x);
						return false;
					}
				});
			}
			catch (TimeoutException e)
			{
				UnityEngine.Debug.LogError(e.ToString());
				settings.onFailWrapper(e);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e.ToString());
				settings.onFailWrapper(e);
			}
			finally
			{
				currentTokenSource.Dispose();

				// Set to null so that we don't try to Cancel OnApplicationQuit when there's nothing to cancel
				currentTokenSource = null;
			}
		}

		/// <summary>
		/// Thread function to run external process.
		/// </summary>
		/// <param name="obj">Gets cast to Settings used to start up process, including sync'd callbacks and exitEvent</param>
		void ExternalProcessAction(Settings settings)
		{
			if (!settings.isValid) return;

			if (settings.ctoken.IsCancellationRequested)
			{
				UnityEngine.Debug.Log("Task was cancelled before it got started.");
				settings.ctoken.ThrowIfCancellationRequested();
			}

			lock(currentProcessLock)
            {
				currentProcess = -1;
            }

			ProcessStartInfo startInfo = new ProcessStartInfo(settings.process);

			// UseShellExecute must be false and RedirectStandardInput must be true to pass standardInput to the active Process
			startInfo.UseShellExecute = string.IsNullOrEmpty(startInfo.FileName);
			startInfo.RedirectStandardInput = true; //if UseShellExecute is true, then StandardInput won't work

			// Do not create external process window
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.CreateNoWindow = true;

			Process process;

			// These exitCodes will be passed to the main Unity thread via settings.onProcessFinishedWrapper
			int[] exitCodes = new int[settings.commands.Length];

			// Run each command
			for (int commandIndex = 0; commandIndex < settings.commands.Length; commandIndex++)
			{
				startInfo.Arguments = settings.commands[commandIndex];

				try
				{
					using (process = new Process())
					{
						process.StartInfo = startInfo;

						//Do give us our exit codes
						process.EnableRaisingEvents = true;

						process.Start();

						// Synchronously signal main Unity thread that we are starting the processes
						if (commandIndex == 0)
							settings.onFirstProcessStartedWrapper();

						lock (currentProcessLock)
                        {
							currentProcess = commandIndex;
                        }

						if (settings.ShouldWaitForNextStandardInput(commandIndex, 0))
						{
							//StandardInput is used to pass commands to the Process, as you would be able to in a command line window
							StreamWriter streamWriter = process.StandardInput;

							for (int inputIndex = 0; inputIndex < settings.standardInput[commandIndex].Length; inputIndex++)
							{
								//The thread should wait until the autoResetEvent is signalled
								settings.standardInputEvent.WaitOne();

								if (settings.ctoken.IsCancellationRequested)
                                {
									process.Kill();
									settings.ctoken.ThrowIfCancellationRequested();
                                }

								//Then the exitCommand can be passed to this Process
								streamWriter.WriteLine(settings.standardInput[commandIndex][inputIndex]);
							}

							//Cleanup the StandardInput
							streamWriter.Close();
						}

						//Wait for any final process work e.g. for file to save
						process.WaitForExit();
						exitCodes[commandIndex] = process.ExitCode;
					}

				}
				catch (OperationCanceledException e)
                {
					//Throw back up to Main Task
					throw (e);
                }
				catch (Exception e)
				{
					UnityEngine.Debug.Log("Unknown exception in ExternalProcessAction Task: " + e.ToString());
					//Throw back up to Main Task
					throw (e);
				}
				finally
                {
					lock(currentProcessLock)
                    {
						currentProcess = -1;
                    }
                }

			}//end for loop

			// Synchronously signal the main Unity thread that we are finished successfully with the processes
			settings.onAllProcessFinishedWrapper(exitCodes);
		}

		/// <summary>
		/// For overriding
		/// </summary>
		/// <param name="onFailWrapper"></param>
		/// <param name="e"></param>
		protected virtual void OnFail(Exception e)
        {
			onFail?.Invoke(e);
        }

		/// <summary>
		/// For overriding
		/// </summary>
		/// <param name="onSuccessWrapper"></param>
		protected virtual void OnSuccess()
        {
			onSuccess?.Invoke();
        }

		/// <summary>
		/// For overriding
		/// </summary>
		/// <param name="onFirstProcessStartedWrapper"></param>
		protected virtual void OnFirstProcessStarted()
        {
			onFirstProcessStarted?.Invoke();
        }

		/// <summary>
		/// For overriding
		/// </summary>
		/// <param name="onAllProcessSuccessWrapper"></param>
		/// <param name="exitCodes"></param>
		protected virtual void OnAllProcessSuccess(int[] exitCodes)
        {
			onAllProcessSuccess?.Invoke(exitCodes);
        }

		public virtual void Init()
		{
			//If there's a recording to cancel, wait to invoke the callback
			if (currentTokenSource != null)
			{
				onFail += InvokeInitOnFail;
				CancelTask();
			}
			//Otherwise invoke it right away
			else
				onInit?.Invoke();
		}

		private void InvokeInitOnFail(Exception e)
		{
			onFail -= InvokeInitOnFail;

			onInit?.Invoke();
		}

		/// <summary>
		/// Object containing a process's StartInfo parameters, as well as callbacks for the containing thread
		/// </summary>
		public class Settings
		{
			/// <summary>
			/// The executable file used to run the process.
			/// </summary>
			public string process;

			/// <summary>
			/// Commands that would be sent line by line to the process, as they would be in a separate command line window.
			/// This class starts up a new process for each command.
			/// </summary>
			public string[] commands;

			/// <summary>
			/// Commands that would be sent line by line to the process, as they would be in a separate command line window.
			/// This class uses these commands, if there are any, to pass via StandardInput to the active process.
			/// </summary>
			public string[][] standardInput;

			/// <summary>
			/// Synchronization event that the thread will wait on until signalling from the main thread for it to
			/// pass the next standard input string to the active Process.
			/// </summary>
			public AutoResetEvent standardInputEvent;

			public Action<Exception> onFailWrapper;
			public Action onSuccessWrapper;
			public Action onFirstProcessStartedWrapper;
			public Action<int[]> onAllProcessFinishedWrapper;

			/// <summary>
			/// Cancellation token for ExternalProcessAction Task.
			/// </summary>
			public CancellationToken ctoken;

			public int timeout = 60000;

			/// <summary>
			/// Checks that there is a process file to run and commands to issue to it.
			/// </summary>
			public bool isValid
			{
				get
				{
					return (
						!string.IsNullOrEmpty(process) &&
						commands != null &&
						commands.Length > 0 &&
						!string.IsNullOrEmpty(commands[0])
					);
				}
			}

			/// <summary>
			/// Checks if there are exit commands to issue to the command corresponding to the given index
			/// </summary>
			/// <param name="commandIndex">Index of given command in settings.commands</param>
			/// <param name="standardInputIndex">Index of standard input for the given command</param>
			/// <returns></returns>
			public bool ShouldWaitForNextStandardInput(int commandIndex, int standardInputIndex)
			{
				return commands != null &&
					   commandIndex < commands.Length &&
					   standardInput != null &&
					   commandIndex < standardInput.Length &&
					   standardInput[commandIndex] != null &&
					   standardInputIndex < standardInput[commandIndex].Length &&
					   !string.IsNullOrEmpty(standardInput[commandIndex][standardInputIndex]);
			}

			public Settings(
				string process,
				string command
			)
			{
				Set(process, new string[] { command });
			}

			public Settings(
				string process,
				string command,
				string standardInput = null
			)
			{
				Set(process, new string[] { command }, new string[][] { new string[] { standardInput } });
			}
			public Settings(
				string process,
				string[] commands,
				string[][] standardInput = null
			)
			{
				Set(process, commands, standardInput);
			}

			public void Set(
				string process,
				string[] commands
			)
			{
				this.process = process;
				this.commands = commands;
			}

			public void Set(
				string process,
				string[] commands,
				string[][] standardInput = null
			)
			{
				this.process = process;
				this.commands = commands;
				this.standardInput = standardInput;
			}
		}

	}

}

