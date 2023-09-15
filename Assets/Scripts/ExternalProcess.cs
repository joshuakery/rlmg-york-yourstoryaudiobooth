using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System;
using UnityEngine;

public class ExternalProcess : MonoBehaviour
{
	public string processFileName;




	void OnDestroy()
	{
		// TODO Clean things up?
		//like clear Process?
	}



	public void Run(Settings settings)
	{
		if (settings == null) return;
		if (string.IsNullOrEmpty(settings.process)) settings.process = processFileName;
		if (!settings.isValid) return;
		ThreadPool.QueueUserWorkItem(new WaitCallback(run), settings);
	}
	void run(object obj)
	{
		var settings = obj as Settings;
		if (!settings.isValid) return;

		ProcessStartInfo startInfo = new ProcessStartInfo(settings.process);
		startInfo.UseShellExecute = string.IsNullOrEmpty(startInfo.FileName);
		startInfo.WindowStyle = ProcessWindowStyle.Hidden;
		startInfo.CreateNoWindow = true;

		Process process;

		int[] exitCodes = new int[settings.commands.Length];

		//https://www.learncsharptutorial.com/threadpooling-csharp-example.php
		//https://jonskeet.uk/csharp/threads/threadpool.html

		//----> https://stackoverflow.com/questions/58363535/c-sharp-execute-threading-threadpool-queueuserworkitem-callback-in-main-thread-a
		//SyncContext
		// run each command
		for (int i=0; i<settings.commands.Length; i++)
		{
			startInfo.Arguments = settings.commands[i];

			using (process = new Process())
            {
				process.StartInfo = startInfo;
				process.EnableRaisingEvents = true;
				process.Start();

				process.WaitForExit();
				exitCodes[i] = process.ExitCode;
			}
			//what happens to the process object here? can I use the exit codes still?
			//where is the callback?
		}

		//////////////if (settings.callback != null) UnityMainThreadDispatcher.Instance().Enqueue (onComplete (settings.callback));
	}

	/*public void RunMulti (MultiProcessSettings settings)
	{
		if (settings == null) return;
		if (!settings.isValid) return;
		ThreadPool.QueueUserWorkItem (new WaitCallback (runMulti), settings);
	}
	void runMulti (object obj)
	{
		var multiProcessSettings = obj as MultiProcessSettings;
		if (!multiProcessSettings.isValid) return;

		var startInfo = new ProcessStartInfo ();
		startInfo.UseShellExecute = false;
		startInfo.CreateNoWindow = true;
		Process process;

		// run commands for each process
		foreach (var processSettings in multiProcessSettings.settings)
		{
			startInfo.FileName = processSettings.process;
			startInfo.UseShellExecute = string.IsNullOrEmpty (startInfo.FileName);

			foreach (string command in processSettings.commands)
			{
				startInfo.Arguments = command;
				process = Process.Start (startInfo);

				process.WaitForExit ();
				process.Close ();
			}
		}

		if (multiProcessSettings.callback != null) UnityMainThreadDispatcher.Enqueue (onComplete (multiProcessSettings.callback));
	}*/


	IEnumerator onComplete(Action callback = null)
	{
		if (callback != null) callback();
		yield return null;
	}




	public class Settings
	{
		public string process;
		public string[] commands;
		public Action callback;

		public bool isValid { get { return (!string.IsNullOrEmpty(process) && commands != null && commands.Length > 0 && !string.IsNullOrEmpty(commands[0])); } }


		public Settings(string process, string command, Action callback)
		{
			Set(process, new string[] { command }, callback);
		}
		public Settings(string process, string[] commands, Action callback)
		{
			Set(process, commands, callback);
		}

		public void Set(string process, string[] commands, Action callback)
		{
			this.process = process;
			this.commands = commands;
			this.callback = callback;
		}
	}


	/*public class MultiProcessSettings
	{
		public ProcessSettings [] settings;
		public Action callback;

		public bool isValid 
		{ 
			get 
			{
				if (settings != null && settings.Length > 0)
				{
					foreach (var setting in settings)
					{
						if (setting.isValid) return true;
					}
				}
				return false;
			} 
		}

		public MultiProcessSettings (ProcessSettings settings, Action callback)
		{
			Set (new ProcessSettings [] { settings }, callback);
		}
		public MultiProcessSettings (ProcessSettings [] settings, Action callback)
		{
			Set (settings, callback); 
		}

		public void Set (ProcessSettings [] settings, Action callback)
		{
			this.settings = settings;
			this.callback = callback;
		}
	}


	public class ProcessSettings
	{
		public string process;
		public string [] commands;

		public bool isValid { get { return (!string.IsNullOrEmpty (process) && commands != null && commands.Length > 0 && !string.IsNullOrEmpty (commands [0])); } }


		public ProcessSettings (string process, string command)
		{
			Set (process, new string[] { command });
		}
		public ProcessSettings (string process, string [] commands)
		{
			Set (process, commands); 
		}

		public void Set (string process, string [] commands)
		{
			this.process = process;
			this.commands = commands;
		}
	}*/
}
