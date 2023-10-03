using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using rlmg.logging;

namespace JoshKery.GenericUI.ContentLoading
{
    public static class MediaLoadingUtility
    {
        public static string[] imageExtensions = {
            ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", //etc
        };

        public static string[] videoExtensions =
        {
            ".MP4", ".MOV"
        };

        public enum FileType
        {
            Unknown = 0,
            Image = 1,
            Video = 2,
            Audio = 3
        }

        public static bool IsImageFile(string path)
        {
            return -1 != Array.IndexOf(imageExtensions, Path.GetExtension(path).ToUpperInvariant());
        }

        public static bool IsVideoFile(string path)
        {
            return -1 != Array.IndexOf(videoExtensions, Path.GetExtension(path).ToUpperInvariant());
        }

        public static FileType GetFileType(string path)
        {
            if (IsImageFile(path))
                return FileType.Image;
            else if (IsVideoFile(path))
                return FileType.Video;
            else
                return FileType.Unknown;
        }

        public static char[] pathSplitCharacters = new char[] { '/', '\\' };

        public static string RemoveStartingPathSplitCharacter(string path)
        {
            foreach (char c in pathSplitCharacters)
            {
                if (path.StartsWith(c))
                    return path.Substring(1);
            }
            return path;
        }

        public static IEnumerator LoadTexture2DFromPath(string uri, Action<Texture2D> setter)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        RLMGLogger.Instance.Log(String.Format("Error: {0} at {1}", webRequest.error, uri), MESSAGETYPE.ERROR);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        RLMGLogger.Instance.Log(String.Format("HTTP Error: {0} at {1}", webRequest.error, uri), MESSAGETYPE.ERROR);
                        break;
                    case UnityWebRequest.Result.Success:
                        Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                        if (texture == null)
                        {
                            RLMGLogger.Instance.Log(String.Format("Null texture for {0}", uri), MESSAGETYPE.ERROR);
                        }
                        if (texture != null) { setter(texture); }

                        break;
                }
            }
        }
    }

}
