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
        private string INFILENAME = "out.mp3";

        public string OUTDIRECTORY = "recordings";

        public string LOGFILENAME = "all_user_info";

        public string LOGHEADERLINE = "FirstName, LastName, Email, FileName";

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

            logFilePath = Path.Combine(Application.streamingAssetsPath, LOGFILENAME) + extension;

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

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

        public void LogFile(string firstName, string lastName, string email, string fileName)
        {
            string line = string.Join(delimiter, firstName, lastName, email, fileName);

            WriteLine(line);
        }

        protected void WriteLine(string line)
        {
            string output = line + Environment.NewLine;

            File.AppendAllText(logFilePath, output);
        }
    }
}


