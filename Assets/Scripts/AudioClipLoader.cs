using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace JoshKery.York.AudioRecordingBooth
{
    public class AudioClipLoader : MonoBehaviour
    {
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
            var path = Path.Combine(Application.streamingAssetsPath, "out.mp3");

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
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        clip = DownloadHandlerAudioClip.GetContent(webRequest);
                        clip.name = "Placeholder";
                        break;
                }
            }

            return clip;
        }
    }
}


