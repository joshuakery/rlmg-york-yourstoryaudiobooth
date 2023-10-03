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
		/// Default executable file to use in the process if none is specified
		/// </summary>
		public string defaultProcessFileName;

		/// <summary>
		/// Helper wrapper around settings.startCallback
		/// </summary>
		public delegate void StartCallbackWrapper();

		/// <summary>
		/// Helper wrapper around settings.allFinishedCallback
		/// </summary>
		/// <param name="exitCodes">Codes, one for each process, corresponding to settings.commands</param>
		public delegate void AllFinishedCallbackWrapper(int[] exitCodes);

		/// <summary>
		/// When signalled, process will continue, issue its exit command, and WaitForExit.
		/// </summary>
		protected static AutoResetEvent standardInputEvent = new AutoResetEvent(false);

		private CancellationTokenSource currentTokenSource = new CancellationTokenSource();

		void OnDestroy()
		{
			// TODO Clean things up?
			//like clear Process?
		}

		public void OnCancelTask()
		{
			if (currentTokenSource != null)
            {
				UnityEngine.Debug.Log("cancel");
				currentTokenSource.Cancel();
				
				standardInputEvent.Set();
				currentTokenSource.Dispose();
			}
				
		}

		/// <summary>
		/// Setup SynchronizationContext and callback wrappers, exitEvent and queue thread
		/// </summary>
		/// <param name="settings">Settings which will be passed as an object to WaitCallback(threadFunction)</param>
		public Task Run(Settings settings)
		{
			if (settings == null) return null;
			if (string.IsNullOrEmpty(settings.process)) settings.process = defaultProcessFileName;
			if (!settings.isValid) return null;

			//Setup SynchronizationContext for use in calling callbacks on main Unity thread
			var syncContext = SynchronizationContext.Current;

			if (settings.allFinishedCallback != null)
				settings.allFinishedCallbackWrapper = new AllFinishedCallbackWrapper(exitCodes => syncContext.Post(_ => settings.allFinishedCallback(exitCodes), null));

			//Assign autoResetEvent to wait on if there are any exit commands for a command
			settings.standardInputEvent = standardInputEvent;

			currentTokenSource = new CancellationTokenSource();
			settings.token = currentTokenSource.Token;

			Task externalProcessTask = Task.Run( () => ExternalProcessTaskAction(settings), settings.token );

			return externalProcessTask;
		}

		/// <summary>
		/// Thread function to run external process.
		/// </summary>
		/// <param name="obj">Gets cast to Settings used to start up process, including sync'd callbacks and exitEvent</param>
		void ExternalProcessTaskAction(Settings settings)
		{
			if (!settings.isValid) return;

			if (settings.token.IsCancellationRequested)
			{
				UnityEngine.Debug.Log("Task was cancelled before it got started.");
				settings.token.ThrowIfCancellationRequested();
			}

			ProcessStartInfo startInfo = new ProcessStartInfo(settings.process);

			// UseShellExecute must be false and RedirectStandardInput must be true to pass standardInput to the active Process
			startInfo.UseShellExecute = string.IsNullOrEmpty(startInfo.FileName);
			startInfo.RedirectStandardInput = true; //if UseShellExecute is true, then StandardInput won't work

			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.CreateNoWindow = true;

			Process process;

			// These exitCodes will be passed to the main Unity thread via settings.callback
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

						if (settings.ShouldWaitForNextStandardInput(commandIndex, 0))
						{
							//StandardInput is used to pass commands to the Process, as you would be able to in a command line window
							StreamWriter streamWriter = process.StandardInput;

							for (int inputIndex = 0; inputIndex < settings.standardInput[commandIndex].Length; inputIndex++)
							{
								//The thread should wait until the autoResetEvent is signalled
								settings.standardInputEvent.WaitOne();

								if (settings.token.IsCancellationRequested)
                                {
									process.Kill();
									settings.token.ThrowIfCancellationRequested();
									return;
                                }

								//Then the exitCommand can be passed to this Process
								streamWriter.WriteLine(settings.standardInput[commandIndex][inputIndex]);
							}

							//Cleanup the StandardInput
							streamWriter.Close();
						}

						//Wait for file to save
						process.WaitForExit();
						exitCodes[commandIndex] = process.ExitCode;
					}

				}
				catch (Exception e)
				{
					//do nothing
				}
				finally
                {
					
				}

			}//end for loop

			//Not sure why this check needs to be performed.
			//I am expecting the return above to short-circuit(?) the rest of the Task
			//and therefore for this code to never be reached
			//and yet it is...
			if (!settings.token.IsCancellationRequested)
            {
				//Finally invoke the sync'd callback to return the exit codes
				if (settings.allFinishedCallbackWrapper != null)
					settings.allFinishedCallbackWrapper.Invoke(exitCodes);
			}


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

			public Action<int[]> allFinishedCallback;

			public AllFinishedCallbackWrapper allFinishedCallbackWrapper;

			public CancellationToken token;

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
				string command,
				Action<int[]> allFinishedCallback = null
			)
			{
				Set(process, new string[] { command }, allFinishedCallback);
			}

			public Settings(
				string process,
				string command,
				string standardInput = null,
				Action<int[]> allFinishedCallback = null
			)
			{
				Set(process, new string[] { command }, new string[][] { new string[] { standardInput } }, allFinishedCallback);
			}
			public Settings(
				string process,
				string[] commands,
				string[][] standardInput = null,
				Action<int[]> allFinishedCallback = null
			)
			{
				Set(process, commands, standardInput, allFinishedCallback);
			}

			public void Set(
				string process,
				string[] commands,
				Action<int[]> allFinishedCallback = null
			)
			{
				this.process = process;
				this.commands = commands;
				this.allFinishedCallback = allFinishedCallback;
			}

			public void Set(
				string process,
				string[] commands,
				string[][] standardInput = null,
				Action<int[]> allFinishedCallback = null
			)
			{
				this.process = process;

				this.commands = commands;
				this.standardInput = standardInput;

				this.allFinishedCallback = allFinishedCallback;
			}
		}

	}

}

