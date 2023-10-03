using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace rlmg.logging
{
	public enum MESSAGETYPE
	{
		ERROR,
		INFO
	}

	public enum LOGLEVEL
	{
		NONE,
		ERRORS,
		VERBOSE
	}

	public enum DESTINATIONPATH
	{
    	DESKTOP,
    	APPLICATION
	}

	public class RLMGLogger : Singleton<RLMGLogger>
	{
		[SerializeField]
		protected string logFolderName = "RLMGLogs";

		[SerializeField]
		protected string logFileName = "rlmg_log";

		[SerializeField]
		private string headerLine = "Timestamp, Message";

		[SerializeField]
		int maxDays = 30; // 0 = never delete

		[SerializeField]
		LOGLEVEL logLevel = LOGLEVEL.VERBOSE;

		[SerializeField]
		LOGDELIMITER logDelimiter = LOGDELIMITER.COMMA;

	    [SerializeField]
		DESTINATIONPATH destPath = DESTINATIONPATH.APPLICATION;

	    [SerializeField]
	    bool logFilePerSession = false;

		protected string logFolderPath;
		protected string logFilePath;
		protected string delimiter = ",";

		public enum LOGDELIMITER
		{
			COMMA,
			TAB
		}

		void Awake()
		{
			SetLogPath();

			delimiter = logDelimiter == LOGDELIMITER.COMMA ? "," : "\t";

			DateTime Now = DateTime.Now;

			#if UNITY_EDITOR
			return;  //don't save to a file if in-editor
			#endif

			// create log folder if necessary
			if(!Directory.Exists(logFolderPath))
			{
				Directory.CreateDirectory(logFolderPath);
			}

			if(maxDays > 0)
			{
				// check if need to delete old log files
				string[] filePaths = Directory.GetFiles(logFolderPath);
				foreach (string file in filePaths)
				{
					DateTime fileDate = File.GetCreationTime(file);
					int days = Now.Subtract(fileDate).Days;
					// get rid if older than a month (30 days)
					if (days > maxDays)
					{
						File.Delete(file);
					}
				}
			}

			logFileName = logFileName + "_" + Now.Month + "-" + Now.Day + "-" + Now.Year;

			if(logFilePerSession)
			{
				int sessionIndex = 1;

				// find index of latest session and add to file path name
				string[] filePaths = Directory.GetFiles(logFolderPath);
				foreach (string file in filePaths)
				{
					if(file.Contains(logFileName))
					{
						// increment index
						sessionIndex++;
					}
				}

				logFileName += "-" + sessionIndex.ToString("D4");
			}

			logFileName += ".csv";

			logFilePath = Path.Combine(logFolderPath, logFileName);

			if (!File.Exists(logFilePath))
			{
				// create if a new log file
				string header = headerLine + Environment.NewLine;
				File.WriteAllText(logFilePath, header);
			}
		}

		virtual protected void SetLogPath()
		{
			string path = "";
			if (destPath == DESTINATIONPATH.DESKTOP)
			{
				path = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
			}
			else if (destPath == DESTINATIONPATH.APPLICATION)
			{
				path = Environment.CurrentDirectory;
			}

			logFolderPath = Path.Combine(path, logFolderName);
		}

		public void Log(string message)
		{
			string line = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + delimiter + message;

			WriteLine(line);
		}

		public void Log(string message, MESSAGETYPE type)
		{
			if(type == MESSAGETYPE.ERROR || logLevel == LOGLEVEL.VERBOSE)
			{
				Log(message);
			}

			// also spit out to console
			if(type == MESSAGETYPE.ERROR)
			{
				Debug.LogError(message);
			}
			else
			{
				Debug.Log(message);
			}
		}

		protected void WriteLine(string line)
		{
			#if UNITY_EDITOR
			return;  //don't save to a file if in-editor
			#endif

			string output = line + Environment.NewLine;

			File.AppendAllText(logFilePath, output);
		}
	}
}