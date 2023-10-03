using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class NameValidator : MonoBehaviour
    {
        private int MAX_FILENAME_LENGTH = 256;

        public enum ValidationResponse
        {
            Unknown = -2,
            Timeout = -1,
            Success = 0,
            Empty = 1,
            InvalidFormat = 2,
            TooLong = 3
        }

        /// <summary>
        /// Checks if filename is valid length
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ValidationResponse Validate(string filename)
        {
            if (filename == null)
                return ValidationResponse.Unknown;

            if (filename.Length > MAX_FILENAME_LENGTH)
                return ValidationResponse.TooLong;

            return ValidationResponse.Success;
        }
    }
}


