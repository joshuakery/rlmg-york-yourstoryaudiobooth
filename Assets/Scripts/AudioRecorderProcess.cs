using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace references
{
	public class AudioRecorderProcess : ExternalProcess
	{
		private const string FFMPEG = "ffmpeg";

		private string startRecordingTemplate = "-f dshow -y -i audio=\"{0}\" {1}";

		private string stopRecordingTemplate = @"q";

		private string testTemplate = "-version";

		void Start()
		{
			processFileName = FFMPEG;
		}

    
        //2. Use Exit Codes
        //2. Modify base class to allow standardinput or to return Process so that I cans close it

		public void StartRecording(string deviceName, string fileOut, Action callback)
        {
            Run(new Settings(processFileName, string.Format(startRecordingTemplate, deviceName, fileOut), callback));

/*            string command = string.Format(startRecordingTemplate, deviceName, fileOut);
            UnityEngine.Debug.Log("This is the ffmpeg string being passed to the process: " + command);

            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "ffmpeg";
                myProcess.StartInfo.Arguments = command;
                myProcess.EnableRaisingEvents = true;
                myProcess.Start();
                myProcess.WaitForExit();
                int ExitCode = myProcess.ExitCode;
                print("Process exited : " + ExitCode);

            }
            catch (Exception e)
            {
                print("Process exception: " + e);
            }*/
        }

		public void StopRecording()
        {
			
        }

		public void OnStartRecording()
        {
			StartRecording("Microphone Array (AMD Audio Device)", "out.mp3", null);
        }

		public void TestProcess()
        {
            UnityEngine.Debug.Log("This is the ffmpeg string being passed to the process: " + testTemplate);

            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal; //Hidden
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "ffmpeg";
                myProcess.StartInfo.Arguments = testTemplate;
                myProcess.EnableRaisingEvents = true;
                myProcess.Start();
                myProcess.WaitForExit();
                int ExitCode = myProcess.ExitCode;
                print("Process exited : " + ExitCode);
            }
            catch (Exception e)
            {
                print("Process exception: " + e);
            }
        }

	}

}

