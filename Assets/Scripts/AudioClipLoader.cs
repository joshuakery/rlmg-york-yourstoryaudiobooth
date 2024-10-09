using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;
using rlmg.logging;

namespace JoshKery.York.AudioRecordingBooth
{
    public class AudioClipLoader : MonoBehaviour
    {
        public bool doLoadOnStart = true;

        public string clipFileName = "out.mp3";

        public AudioClip CurrentClip;

        [SerializeField]
        private AudioSource audioSource;

        #region Public UnityEvents
        private UnityEvent _ClipLoaded;
        public UnityEvent ClipLoaded
        {
            get
            {
                if (_ClipLoaded == null)
                    _ClipLoaded = new UnityEvent();

                return _ClipLoaded;
            }
        }
        #endregion

        
        

        private async void Start()
        {
            if (doLoadOnStart)
                await Reload();
        }

        public async Task Reload(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = Path.Combine(Application.streamingAssetsPath, clipFileName);

            CurrentClip = await LoadClip(path);

            if (audioSource != null)
            {
                audioSource.clip = CurrentClip;

                //Toggling Play here because this loads the audio or something
                //I was having problems where I couldn't change the AudioSource.time before a call to Play
                //in the case of e.g. seeking before the first playback
                audioSource.Play();
                audioSource.Pause();
            }

            ClipLoaded.Invoke();
        }

        private async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;

            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                await webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        RLMGLogger.Instance.Log("Error loading audio clip at: " + path + "\n" + webRequest.error, MESSAGETYPE.ERROR);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        RLMGLogger.Instance.Log("HTTP Error loading audio clip at : " + path + "\n" + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        clip = DownloadHandlerAudioClip.GetContent(webRequest);
                        clip.name = "Placeholder";
                        break;
                }
            }

            return clip;
        }

        public IEnumerator ReloadCo(string fileName = null, string filePath = null)
        {
            if (!string.IsNullOrEmpty(fileName))
                clipFileName = fileName;

            yield return Reload(filePath);
        }
    }
}


