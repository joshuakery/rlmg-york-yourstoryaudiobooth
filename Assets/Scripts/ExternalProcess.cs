using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
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
		protected static AutoResetEvent exitEvent = new AutoResetEvent(false);

		void OnDestroy()
		{
			// TODO Clean things up?
			//like clear Process?
		}

		/// <summary>
		/// Setup SynchronizationContext and callback wrappers, exitEvent and queue thread
		/// </summary>
		/// <param name="settings">Settings which will be passed as an object to WaitCallback(threadFunction)</param>
		public void Run(Settings settings)
		{
			if (settings == null) return;
			if (string.IsNullOrEmpty(settings.process)) settings.process = defaultProcessFileName;
			if (!settings.isValid) return;

			//Setup SynchronizationContext for use in calling callbacks on main Unity thread
			var syncContext = SynchronizationContext.Current;


			if (settings.startCallback != null)
				settings.startCallbackWrapper = new StartCallbackWrapper(() => syncContext.Post(_ => settings.startCallback(), null));


			settings.allFinishedCallbackWrapper = new AllFinishedCallbackWrapper(exitCodes => syncContext.Post(_ => settings.allFinishedCallback(exitCodes), null));

			//Assign autoResetEvent to wait on if there are any exit commands for a command
			settings.exitEvent = exitEvent;

			ThreadPool.QueueUserWorkItem(new WaitCallback(run), settings);
		}

		/// <summary>
		/// Thread function to run external process.
		/// </summary>
		/// <param name="obj">Settings used to start up process, including sync'd callbacks and exitEvent</param>
		void run(object obj)
		{
			var settings = obj as Settings;
			if (!settings.isValid) return;

			ProcessStartInfo startInfo = new ProcessStartInfo(settings.process);

			// UseShellExecute must be false and RedirectStandardInput must be true to pass commands to the active Process
			startInfo.UseShellExecute = string.IsNullOrEmpty(startInfo.FileName);
			startInfo.RedirectStandardInput = true; //if UseShellExecute is true, then StandardInput won't work

			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.CreateNoWindow = true;

			Process process = new Process();

			// These exitCodes will be passed to the main Unity thread via settings.callback
			int[] exitCodes = new int[settings.commands.Length];

			// Run each command
			for (int i = 0; i < settings.commands.Length; i++)
			{
				startInfo.Arguments = settings.commands[i];

				try
				{
					using (process = new Process())
					{
						process = new Process();

						process.StartInfo = startInfo;
						process.EnableRaisingEvents = true;
						process.Start();

						if (settings.startCallbackWrapper != null)
							settings.startCallbackWrapper();

						if (settings.ShouldWaitForExitCommand(i))
						{
							//StandardInput is used to pass commands to the Process, as you would be able to in a command line window
							StreamWriter streamWriter = process.StandardInput;

							//The thread should wait until the autoResetEvent is signalled
							settings.exitEvent.WaitOne();

							//Then the exitCommand can be passed to this Process
							streamWriter.WriteLine(settings.exitCommands[i]);

							//Cleanup the StandardInput
							streamWriter.Close();
						}

						//Wait to save file?
						process.WaitForExit();
						exitCodes[i] = process.ExitCode;
					}

				}
				catch (Exception e)
				{
					//do nothing
				}

			}

			if (settings.allFinishedCallback != null)
				settings.allFinishedCallbackWrapper(exitCodes);

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
			public string[] exitCommands;

			/// <summary>
			/// Callback invoked at the start of each process.
			/// </summary>
			public Action startCallback;

			/// <summary>
			/// Callback invoked at the end of all the processes.
			/// The int[] parameter is for passing back the processes' exit codes to the main Unity thread.
			/// </summary>
			public Action<int[]> allFinishedCallback;

			/// <summary>
			/// Helper delegate used for the callback after each process is started.
			/// </summary>
			public StartCallbackWrapper startCallbackWrapper;

			/// <summary>
			/// Helper delegate used for the callback when all processes are finished.
			/// </summary>
			public AllFinishedCallbackWrapper allFinishedCallbackWrapper;

			/// <summary>
			/// Synchronization event that the thread will wait on until signalling from the main thread for it to
			/// pass the exitcommand to the active Process.
			/// </summary>
			public AutoResetEvent exitEvent;

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
			/// <returns></returns>
			public bool ShouldWaitForExitCommand(int commandIndex)
			{
				return exitCommands != null &&
					   exitCommands.Length > commandIndex &&
					   !string.IsNullOrEmpty(exitCommands[commandIndex]);
			}

			public Settings(
				string process,
				string command,
				string exitCommand,
				Action startCallback,
				Action<int[]> allFinishedCallback
			)
			{
				Set(process, new string[] { command }, new string[] { exitCommand }, startCallback, allFinishedCallback);
			}
			public Settings(
				string process,
				string[] commands,
				string[] exitCommands,
				Action startCallback,
				Action<int[]> allFinishedCallback
			)
			{
				Set(process, commands, exitCommands, startCallback, allFinishedCallback);
			}

			public void Set(
				string process,
				string[] commands,
				string[] exitCommands,
				Action startCallback,
				Action<int[]> allFinishedCallback
			)
			{
				this.process = process;

				this.commands = commands;
				this.exitCommands = exitCommands;

				this.startCallback = startCallback;
				this.allFinishedCallback = allFinishedCallback;
			}
		}

	}

}

