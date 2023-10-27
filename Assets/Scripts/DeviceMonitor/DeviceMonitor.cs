using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using JoshKery.GenericUI.DOTweenHelpers;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    /// <summary>
    /// Routinely scans for the available devices on the machine
    /// If the target microphone is not available, shows itself as an error message
    /// </summary>
    public class DeviceMonitor : BaseWindow
    {
        public string MicrophoneName = "Microphone Array (AMD Audio Device)";

        public int Timeout = 2000;

        private DateTime lastScanTime;

        private void Start()
        {
            lastScanTime = DateTime.Now;
        }

        private void Update()
        {
            TimeSpan duration = DateTime.Now - lastScanTime;

            if (duration.TotalMilliseconds > Timeout)
            {
                ScanForMicrophone();

                lastScanTime = DateTime.Now;
            }
        }

        private void ScanForMicrophone()
        {
            if (!Microphone.devices.Contains(MicrophoneName))
            {
                if (!isOpen)
                {
                    Open(SequenceType.UnSequenced);

                    RLMGLogger.Instance.Log(
                        string.Format(
                            "Requested microphone not found: {0}\nAvailable audio input devices:\n{1}",
                            MicrophoneName,
                            string.Join("\n - ", Microphone.devices)
                        ),
                        MESSAGETYPE.ERROR
                    );
                }
                    
            }
            else
            {
                if (isOpen)
                {
                    Close(SequenceType.UnSequenced);

                    RLMGLogger.Instance.Log(
                        string.Format(
                            "Requested microphone successively detected: {0}",
                            MicrophoneName
                        ),
                        MESSAGETYPE.INFO
                    );
                }
                    
            }
        }
    }
}


