using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class SaverLogger : MonoBehaviour
    {
        [SerializeField]
        private string INFILENAME = "out-trimmed.mp3";

        public string OUTDIRECTORY = "recordings";

        public string LOGDIRECTORY = "all_user_info";

        public string LOGFILENAMEBASE = "all_user_info";

        public string LOGHEADERLINE = "FirstName, LastName, Email, FileName, Subscribe";

        protected string logFilePath;
        protected string delimiter = ",";
        protected string extension = ".csv";

        [SerializeField]
        LOGDELIMITER logDelimiter = LOGDELIMITER.COMMA;

        public enum LOGDELIMITER
        {
            COMMA,
            TAB
        }

        private void Awake()
        {
            delimiter = logDelimiter == LOGDELIMITER.COMMA ? "," : "\t";
            extension = logDelimiter == LOGDELIMITER.COMMA ? ".csv" : ".tsv";

            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            string logDirPath = Path.Combine(Application.streamingAssetsPath, LOGDIRECTORY);
            if (!Directory.Exists(logDirPath))
                Directory.CreateDirectory(logDirPath);

            string logFileName = LOGFILENAMEBASE + "_" + DateTime.Now.ToString("yyyy-MM");
            logFilePath = Path.Combine(logDirPath, logFileName) + extension;

            if (!File.Exists(logFilePath))
            {
                string header = LOGHEADERLINE + Environment.NewLine;
                File.WriteAllText(logFilePath, header);
            }
        }



        public string SaveFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return null; }

            try
            {
                string dirPath = Path.Combine(Application.streamingAssetsPath, OUTDIRECTORY);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                string filePath = Path.Combine(dirPath, fileName);

                File.Copy(
                    Path.Combine(Application.streamingAssetsPath, INFILENAME),
                    filePath
                );

                return filePath;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LogFile(string firstName, string lastName, string email, string fileName, bool doSubscribe)
        {
            string line = string.Join(delimiter, firstName, lastName, email, fileName, doSubscribe.ToString());

            WriteLine(line);
        }

        protected void WriteLine(string line)
        {
            string output = line + Environment.NewLine;

            File.AppendAllText(logFilePath, output);
        }
    }
}


