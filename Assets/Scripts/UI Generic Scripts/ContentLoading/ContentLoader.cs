using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rlmg.logging;
using UnityEngine.Serialization;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace JoshKery.GenericUI.ContentLoading
{
	public abstract class ContentLoader : MonoBehaviour
	{
		/// <summary>
        /// If true, LoadContent is called on Awake
        /// </summary>
		public bool loadOnAwake = true;

		/// <summary>
        /// If false, LoadContentCoroutine completes without calling the priority or default local loading methods.
        /// </summary>
		public bool doLoadContent = true;

		/// <summary>
        /// If Editor and true, LoadContentCoroutine completes
        /// </summary>
		public bool canUseInEditor = true;

		/// <summary>
        /// If true, LoadContentCoroutine loads local content only
        /// </summary>
		public bool doDefaultLocalLoadContent = false;

        /// <summary>
        /// Filename where local content file (e.g. json) will be written, and read from by default
        /// </summary>
        public string ContentFilename = "config.json";

		/// <summary>
		/// Is set to true after loading the content the first time.
		/// </summary>
		public bool HasLoadedContent { get; private set; } = false;

		/// <summary>
		/// Callback event at start of LoadContentCoroutine
		/// </summary>
		public UnityEvent onLoadContentCoroutineStart;

		private UnityEvent _onLoadContentCoroutineFinish;
		/// <summary>
		/// Callback event at end of LoadContentCouroutine
		/// </summary>
		public UnityEvent onLoadContentCoroutineFinish
        {
			get
            {
				if (_onLoadContentCoroutineFinish == null)
					_onLoadContentCoroutineFinish = new UnityEvent();

				return _onLoadContentCoroutineFinish;
            }
        }

		/// <summary>
        /// Callback event when caching and setting up content is complete
        /// </summary>
		public UnityEvent onPopulateContentFinish;

		/// <summary>
		/// Location where local content file (e.g. json) and mediaCache folder will be saved.
		/// </summary>
		public enum CONTENT_LOCATION
		{
			StreamingAssets,
			Desktop,
			Application
		}

		/// <summary>
		/// Determines where local content file (e.g. json) and mediaCache folder will be saved.
		/// </summary>
		public CONTENT_LOCATION contentLocation = CONTENT_LOCATION.Application;

		/// <summary>
		/// Path to directory where local content file (e.g. json) and mediaCache folder will be saved.
		/// Based on contentLocation. Either StreamingAssets, Desktop, or Application folders.
		/// </summary>
		protected string LocalContentDirectory
		{
			get
			{
				string path = "";

				if (contentLocation == CONTENT_LOCATION.StreamingAssets)
				{
					path = Application.streamingAssetsPath;
				}
				if (contentLocation == CONTENT_LOCATION.Desktop)
				{
					path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				}
				else if (contentLocation == CONTENT_LOCATION.Application)
				{
					path = Path.Combine(Application.dataPath, "..");
				}

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				return path;
			}
		}

		/// <summary>
		/// Path to file where local content (e.g. json) will be saved.
		/// </summary>
		protected string LocalContentPath
        {
			get
            {
				return Path.Combine(LocalContentDirectory, ContentFilename);
			}
        }

		/// <summary>
		/// Name of media directory where media may be saved to.
		/// </summary>
		[SerializeField]
		protected string LocalMediaCacheDirectoryName = "mediaCache";

		/// <summary>
		/// Path to directory where media may be saved to.
		/// </summary>
		private string LocalMediaCacheDirectory
        {
			get
            {
				string directoryName = string.IsNullOrEmpty(LocalMediaCacheDirectoryName) ? "mediaCache" : LocalMediaCacheDirectoryName;

				string path = Path.Combine(LocalContentDirectory, directoryName);

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				return path;
            }
        }

		

        #region Refresh Content Fields
        public enum RefreshType
		{
			NoRefresh = 0,
			HardRefresh = 1,
			SoftRefresh = 2
		}

		public enum RefreshLimit
		{
			AllContent = 0,
			TextOnly = 1,
			ImagesOnly = 2,
			VideoOnly = 3
		}

		public RefreshType refreshType;
		public RefreshLimit refreshLimit;
		#endregion

		protected virtual void Awake()
		{
#if UNITY_EDITOR
			contentLocation = CONTENT_LOCATION.StreamingAssets;
#endif
			if (loadOnAwake)
				LoadContent();
		}

        #region Load Content Primary Methods
        /// <summary>
		/// Main method for loading content. Starts main LoadContentCoroutine.
		/// </summary>
		public virtual void LoadContent()
		{
			StopAllCoroutines();

			StartCoroutine(LoadContentCoroutine());
		}

		
		/// <summary>
		/// Main coroutine for loading content.
		/// Either loads via the priority method or the default, local method.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator LoadContentCoroutine()
		{
			HasLoadedContent = false;

			if (onLoadContentCoroutineStart != null)
				onLoadContentCoroutineStart.Invoke();

			if (doLoadContent && (!Application.isEditor || canUseInEditor))
			{
				if (doDefaultLocalLoadContent)
					yield return StartCoroutine(LoadLocalContent());
				else
					yield return StartCoroutine(PriorityLoadContent());
			}

			HasLoadedContent = true;

			if (onLoadContentCoroutineFinish != null)
				onLoadContentCoroutineFinish.Invoke();
		}
		#endregion

		#region Load Target Content
		/// <summary>
		/// The priority method for loading content that will be called first.
		/// Subclasses might implement their own success and failure callbacks,
		/// and use the LoadLocalContent as a fallback in the failure callback.
		/// 
		/// Override to use a custom loading method (e.g. via graphql)
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator PriorityLoadContent()
        {
			yield return StartCoroutine(LoadLocalContent());
        }
		#endregion

		#region Load Local Content
		/// <summary>
		/// Load content via UnityWebRequest from LocalContentPath
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator LoadLocalContent()
        {
			using (UnityWebRequest webRequest = UnityWebRequest.Get(LocalContentPath))
			{
				yield return webRequest.SendWebRequest();

				switch (webRequest.result)
				{
					case UnityWebRequest.Result.ConnectionError:
					case UnityWebRequest.Result.DataProcessingError:
					case UnityWebRequest.Result.ProtocolError:
						yield return StartCoroutine( LoadLocalContentFailure(webRequest.error, webRequest.url) );
						break;
					case UnityWebRequest.Result.Success:
						yield return StartCoroutine( LoadLocalContentSuccess(webRequest.downloadHandler.text) );
						break;
				}

				yield return StartCoroutine( LoadLocalContentFinally(webRequest.result) );
			}
		}

		/// <summary>
		/// Callback for load local success
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		protected abstract IEnumerator LoadLocalContentSuccess(string text);

		/// <summary>
		/// Callback for load local error
		/// </summary>
		/// <param name="error"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		protected virtual IEnumerator LoadLocalContentFailure(string error, string url)
        {
			RLMGLogger.Instance.Log("Load Local Content Error: " + error + " at " + url);
			yield return null;
        }

		/// <summary>
		/// Callback for load local finally
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		protected abstract IEnumerator LoadLocalContentFinally(UnityWebRequest.Result result);
        #endregion

        #region Save Content File to Disk
		/// <summary>
		/// Write text to disk at LocalContentPath
		/// </summary>
		/// <param name="text"></param>
		public virtual void SaveContentFileToDisk(string text)
		{
			File.WriteAllText(LocalContentPath, text);
			RLMGLogger.Instance.Log(string.Format("Saved data to local directory: {0}", LocalContentPath), MESSAGETYPE.INFO);
		}
		#endregion

		#region Save Media to Disk
		/// <summary>
		/// Format path for a given media file in the LocalMediaCacheDirectory
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public string GetLocalMediaPath(string filename)
		{
			string path = Path.Combine(LocalMediaCacheDirectory, filename);
			return path;
		}

		/// <summary>
		/// Download the given file and save to disk
		/// </summary>
		/// <param name="onlinePath">URL to download from</param>
		/// <param name="localPath">Local path to save to</param>
		/// <returns></returns>
		public virtual IEnumerator SaveMediaToDisk(string onlinePath, string localPath, bool isTexture=true)
        {
			UnityWebRequest webRequest = null;
			if (isTexture)
				webRequest = UnityWebRequestTexture.GetTexture(onlinePath);
			else
				webRequest = UnityWebRequest.Get(onlinePath);

			yield return webRequest.SendWebRequest();

			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
				case UnityWebRequest.Result.ProtocolError:
					yield return StartCoroutine( SaveMediaToDiskFailure(webRequest) );
					break;
				case UnityWebRequest.Result.Success:
					File.WriteAllBytes(localPath, webRequest.downloadHandler.data);
					yield return StartCoroutine(SaveMediaToDiskSuccess(webRequest.result));
					break;
				default:
					yield return StartCoroutine(SaveMediaToDiskFailure(webRequest));
					break;
			}
		}

		/// <summary>
		/// Callback after successfully downloading and saving media to disk
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		protected virtual IEnumerator SaveMediaToDiskSuccess(UnityWebRequest.Result result)
		{
			yield return null;
		}

		/// <summary>
		/// Callback after failed media download
		/// </summary>
		/// <param name="request">The failed request</param>
		/// <returns></returns>
		protected virtual IEnumerator SaveMediaToDiskFailure(UnityWebRequest request)
		{
			RLMGLogger.Instance.Log("Save Media to Disk Failure: " + request.error, MESSAGETYPE.ERROR);
			RLMGLogger.Instance.Log("Save Media to Disk Failure: Download Handler Error: " + request.downloadHandler.error, MESSAGETYPE.ERROR);
			yield return null;
		}
        #endregion

    }
}


