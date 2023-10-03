using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.IO;
using rlmg.logging;

public class ImageSequence : MonoBehaviour
{
    /// <summary>
    /// From where should the images be loaded
    /// </summary>
    public enum LoadingMethod
    {
        None = 0,
        AssetBundles = 1,
        Resources = 2,
        StreamingAssets = 3
    }

    /// <summary>
    /// The default to which priorityLoadingMethod is to on Awake
    /// </summary>
    public static LoadingMethod defaultLoadingMethod = LoadingMethod.None;

    /// <summary>
    /// Static delegate for all instances to subscribe to
    /// For setting the loading method across all instances
    /// </summary>
    /// <param name="loadingMethod"></param>
    public delegate void SetLoadingMethod(LoadingMethod loadingMethod);
    public static SetLoadingMethod SetAllLoadingMethod;

    /// <summary>
    /// True if any instance is executing its loading method this frame
    /// or if any instance is executing a loading coroutine this frame
    /// </summary>
    public static bool sequencesAreLoading = false;

    /// <summary>
    /// Helper class to track the frames-loading progress of all instances
    /// </summary>
    public class ImageSequenceProgress
    {
        public bool isEnabled = false;
        public int totalFramesLoaded = 0;
        public int totalFramesToLoad = 0;

        public ImageSequenceProgress(bool e, int tL, int tTL)
        {
            isEnabled = e;
            totalFramesLoaded = tL;
            totalFramesToLoad = tTL;
        }
    }

    /// <summary>
    /// Tracks all the frames-loading progress of all instances of class
    /// </summary>
    public static Dictionary<int, ImageSequenceProgress> classProgress;

    /// <summary>
    /// Returns total number of enabled instances as tracked in static dictionary
    /// tracking all class instances' progress
    /// </summary>
    /// <returns></returns>
    public static int GetTotalEnabled()
    {
        return classProgress.Count(p => p.Value != null && p.Value.isEnabled);
    }


    /// <summary>
    /// Returns total number of frames loaded across all instances tracked
    /// in static dictionary of all class instances' progress
    /// </summary>
    /// <returns></returns>
    public static int GetTotalFramesLoaded()
    {
        return classProgress.Sum(p => p.Value != null ? p.Value.totalFramesLoaded : 0);
    }

    /// <summary>
    /// Returns total number of frames loaded and expected to load across all
    /// instances tracked in static dictionary of all class instances' progress
    /// </summary>
    /// <returns></returns>
    public static int GetTotalFramesToLoad()
    {
        return classProgress.Sum(p => p.Value != null ? p.Value.totalFramesToLoad : 0);
    }

    /// <summary>
    /// True if the TryLoadingFromStreamingAssetsCo coroutine was stopped
    /// due to e.g. disabling the component, before it was completed
    /// </summary>
    private bool shouldBeLoading = false;

    /// <summary>
    /// If true, LoadImageSequence will be called in Start function
    /// </summary>
    public bool doLoadOnStart = false;

    /// <summary>
    /// Sets the loadingMethod by which the images should be loaded into frames array
    /// </summary>
    [SerializeField]
    private LoadingMethod priorityLoadingMethod = LoadingMethod.StreamingAssets;

    /// <summary>
    /// Array containing loaded frames
    /// </summary>
    public Sprite[] frames;

    /// <summary>
    /// Path to folder where images are stored
    /// For AssetBundles, StreamingAssets directory path will be prepended
    /// For Resources, Resources directory path will be prepended
    /// For StreamingAssets, StreamingAssets directory path will be prepended
    /// </summary>
    public string framesFolder;

    /// <summary>
    /// If true, Update will update the currFrame count and shown image
    /// </summary>
    public bool videoMoving = true;

    /// <summary>
    /// time within the duration of the image sequence
    /// without looping, will hover around the start or end of the duration
    /// once the video plays through and is still playing
    /// </summary>
    private float currTimecode = 0f;

    /// <summary>
    /// Rate at which display will update
    /// Given by 1 sec / frameRate
    /// So 30fps will yield 0.0333f
    /// </summary>
    public float updateRate = 0.02f;

    /// <summary>
    /// should the video once it reaches its start or end
    /// </summary>
    public bool doLoop = false;
    public bool doReverse = false;

    /// <summary>
    /// Helper int for onFirstFrameReached and onLastFrameReached UnityEvents
    /// Tracks what the past frame was, to compare with the current frame
    /// </summary>
    private int pastFrame = -1;

    /// <summary>
    /// Called once if the first frame is reached after playing in reverse from a later frame
    /// </summary>
    public UnityEvent onFirstFrameReached;

    /// <summary>
    /// Called once if the last frame is reached after playing from an earlier frame
    /// </summary>
    public UnityEvent onLastFrameReached;

    /// <summary>
    /// Called if any of the TryLoading methods fails
    /// </summary>
    public UnityEvent onLoadFailed;

    /// <summary>
    /// current frame, set in Update loop to be currTimecode / Abs(updateRate)
    /// so that, at a currTimecode of 1 sec, for an updateRate of 0.02
    /// currFrame denotes the 50th frame
    /// </summary>
    private int currFrame = 0;
    public Int32 CurrentFrameNum
    {
        get
        {
            return currFrame;
        }
        set
        {
            SetFrame(value);
        }
    }
    public float TotalFrameNum { get { return frames == null ? 0 : frames.Length; } }

    private bool didSetPercentThisFrame = false;

    /// <summary>
    /// Target RawImage display for instance
    /// </summary>
    public RawImage rawImage;

    /// <summary>
    /// Target Image display for instance
    /// </summary>
    public Image uiImage;

    //	public MeshRenderer movieMesh;
    //private Material movieMaterial;
    //	private Texture movieTexture;

    /// <summary>
    /// Target SpriteRenderer display for instance
    /// </summary>
    public SpriteRenderer spriteRenderer;

    private float targetPercent = 0f;

    /// <summary>
    /// Calculated duration of image sequence played at current updateRate
    /// </summary>
    public float duration
    {
        get
        {
            if (frames != null)
                return updateRate * frames.Count();
            else
                return 0f;
        }
    }

    private void Awake()
    {
        priorityLoadingMethod = defaultLoadingMethod;
    }

    void Start()
    {
        if (doLoadOnStart)
        { 
            StartCoroutine(LoadImageSequenceOnDelay());
        }
    }

    /// <summary>
    /// Subscribes instance to static class delegate setLoadingMethod
    /// In addition, if the TryLoadingFromStreamingAssetsCo coroutine was stopped
    /// due to disabling the component, re-start the loading process
    /// </summary>
    private void OnEnable()
    {
        SetAllLoadingMethod += SetMyLoadingMethod;

        if (shouldBeLoading)
            LoadImageSequence();

        UpdateClassProgress();
    }

    /// <summary>
    /// Unsubscribes instance to static class delegate setLoadingMethod
    /// </summary>
    private void OnDisable()
    {
        SetAllLoadingMethod -= SetMyLoadingMethod;

        UpdateClassProgress();
    }

    private void OnDestroy()
    {
        RemoveFromClassProgress();
    }

    /// <summary>
    /// Removes instance from static dictionary tracking the progress of each instance
    /// </summary>
    private void RemoveFromClassProgress()
    {
        int id = gameObject.GetInstanceID();
        if (classProgress != null && classProgress.ContainsKey(id))
        {
            classProgress.Remove(id);
        }
    }


    /// <summary>
    /// Updates the static dictionary tracking the progress of each instance
    /// Inefficient, but for small numbers ok
    /// </summary>
    private void UpdateClassProgress()
    {
        if (classProgress == null)
            classProgress = new Dictionary<int, ImageSequenceProgress>();

        int id = gameObject.GetInstanceID();

        if (!classProgress.ContainsKey(id))
            classProgress[id] = new ImageSequenceProgress(
                this.enabled,
                frames.Count(f => f != null),
                frames.Length
            );
        else
        {
            ImageSequenceProgress myProgress = classProgress[id];
            myProgress.isEnabled = this.enabled;
            myProgress.totalFramesLoaded = frames.Count(f => f != null);
            myProgress.totalFramesToLoad = frames.Length;
        }     
    }

    /// <summary>
    /// Sets instance's priority loading method
    /// </summary>
    private void SetMyLoadingMethod(LoadingMethod method)
    {
        priorityLoadingMethod = method;
    }

    IEnumerator LoadImageSequenceOnDelay()
    {
        yield return new WaitForSeconds(0.1f);

        LoadImageSequence();
    }


    /// <summary>
    /// Switch for TryLoading methods
    /// </summary>
    public void LoadImageSequence()
    {
        sequencesAreLoading = true;

        switch (priorityLoadingMethod)
        {
            case LoadingMethod.None:
                break;
            case LoadingMethod.AssetBundles:
                TryLoadingFromAssetBundle();
                break;
            case LoadingMethod.Resources:
                TryLoadingFromResources();
                break;
            case LoadingMethod.StreamingAssets:
                TryLoadingFromStreamingAssets();
                break;
        }

        sequencesAreLoading = false;

        //IComparer myComparer = new mySorter();
        //Array.Sort(frames, myComparer);



        //StartCoroutine(UpdateFrame());
    }

    /// <summary>
    /// Tries to load images from AssetBundle at framesFolder path
    /// where framesFolder = path to AssetBundle itself
    /// </summary>
    private void TryLoadingFromAssetBundle()
    {
        if (framesFolder == null)
        {
            RLMGLogger.Instance.Log("AssetBundle path cannot be created. Folder value is null.", MESSAGETYPE.ERROR);
            onLoadFailed.Invoke();
            return;
        }

        string path = Path.Combine(Application.streamingAssetsPath, framesFolder);
        if (!File.Exists(path))
        {
            RLMGLogger.Instance.Log(System.String.Format("AssetBundle path does not exist: {0}", path), MESSAGETYPE.ERROR);
            onLoadFailed.Invoke();
            return;
        }

        var myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
        if (myLoadedAssetBundle == null)
        {
            RLMGLogger.Instance.Log("Failed to load AssetBundle!", MESSAGETYPE.ERROR);
            onLoadFailed.Invoke();
        }
        else
        {
            frames = myLoadedAssetBundle.LoadAllAssets<Sprite>();

            if (frames.Length == 0)
            {
                RLMGLogger.Instance.Log("Loaded 0 frames from AssetBundle.", MESSAGETYPE.ERROR);
                onLoadFailed.Invoke();
            }
        }
        myLoadedAssetBundle.Unload(false);

        UpdateClassProgress();
    }

    /// <summary>
    /// Tries to load images from Resources folder at framesFolder path
    /// where framesFolder = subdirectory containing individual image files
    /// </summary>
    private void TryLoadingFromResources()
    {
        if (System.String.IsNullOrEmpty(framesFolder))
        {
            RLMGLogger.Instance.Log("Resources cannot be loaded. Folder value is null or empty.", MESSAGETYPE.ERROR);
            onLoadFailed.Invoke();
            return;
        }

        //frames = Resources.LoadAll<Texture2D>("Image Sequences/" + framesFolder);
        frames = Resources.LoadAll<Sprite>(framesFolder);

        UpdateClassProgress();
    }

    /// <summary>
    /// Via UnityWebRequest coroutines, tries to load images from StreamingAssets folder
    /// at framesFolder path
    /// where framesFolder = subdirectory containing individual image files
    /// </summary>
    private void TryLoadingFromStreamingAssets()
    {
        StartCoroutine(TryLoadingFromStreamingAssetsCo());
    }

    /// <summary>
    /// Helper coroutine for loading from StreamingAssets via UnityWebRequests
    /// </summary>
    /// <returns></returns>
    private IEnumerator TryLoadingFromStreamingAssetsCo()
    {
        if (System.String.IsNullOrEmpty(framesFolder))
        {
            RLMGLogger.Instance.Log("Streaming assets cannot be loaded. Folder value is null or empty.", MESSAGETYPE.ERROR);
            onLoadFailed.Invoke();
            yield break;
        }

        //string dirPath = Path.Combine(Application.streamingAssetsPath, framesFolder);
        //if (!Directory.Exists(dirPath))
        //{
        //    RLMGLogger.Instance.Log(System.String.Format("Streaming Assets path does not exist: {0}", dirPath), MESSAGETYPE.ERROR);
        //    onLoadFailed.Invoke();
        //    yield break;
        //}

        //shouldBeLoading = true;
        //sequencesAreLoading = true;

        //DirectoryInfo dir = new DirectoryInfo(dirPath);
        //FileInfo[] info = dir.GetFiles("*.*").Where(file => AirtableLoader.IsImageFile(file.Name)).ToArray();

        //frames = new Sprite[info.Length];

        //for (int i=0; i<info.Length; i++)
        //{
        //    FileInfo file = info[i];
        //    string uri = ContentLoader.fileProtocolPrefix + Path.Combine(dirPath, file.Name);
        //    using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
        //    {
        //        sequencesAreLoading = true;

        //        yield return webRequest.SendWebRequest();

        //        switch (webRequest.result)
        //        {
        //            case UnityWebRequest.Result.ConnectionError:
        //                RLMGLogger.Instance.Log(String.Format("Error: {0}", webRequest.error), MESSAGETYPE.ERROR);
        //                break;
        //            case UnityWebRequest.Result.DataProcessingError:
        //                RLMGLogger.Instance.Log(String.Format("WebRequest Error: {0}", webRequest.error), MESSAGETYPE.ERROR);
        //                RLMGLogger.Instance.Log(String.Format("Download Handler Error: {0}", webRequest.downloadHandler.error), MESSAGETYPE.ERROR);
        //                break;
        //            case UnityWebRequest.Result.ProtocolError:
        //                RLMGLogger.Instance.Log(String.Format("HTTP Error: {0}", webRequest.error), MESSAGETYPE.ERROR);
        //                break;
        //            case UnityWebRequest.Result.Success:
        //                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
        //                if (texture == null) Debug.LogError("null texture");
        //                Sprite sprite = Sprite.Create(
        //                    texture,
        //                    new Rect(0, 0, texture.width, texture.height),
        //                    new Vector2(0.5f, 0.5f)
        //                );
        //                frames[i] = sprite;
        //                break;
        //        }
        //    }
        //    UpdateClassProgress();
        //}

        //shouldBeLoading = false;
        //sequencesAreLoading = false;

        //RLMGLogger.Instance.Log(String.Format("Loading from Streaming Assets complete for {0}.", dirPath), MESSAGETYPE.INFO);

    }

    public void SetPercent(float percent)
    {
        Percent = percent;
    }

    public float Percent
    {
        get
        {
            return targetPercent;
        }
        set
        {
            targetPercent = value;

            didSetPercentThisFrame = true;
        }
    }

    public void SetFrame(int targetFrame)
    {
        if (frames == null || frames.Length == 0)
            return;

        if (targetFrame > frames.Length - 1)
            targetFrame = frames.Length - 1;

        currFrame = targetFrame;

        if (frames[targetFrame] != null)
        {
            if (rawImage != null)
                rawImage.texture = frames[targetFrame].texture;

            if (uiImage != null)
                uiImage.sprite = frames[targetFrame];

            if (spriteRenderer != null)
                spriteRenderer.sprite = frames[targetFrame];
        }

        currTimecode = ((float)currFrame / (float)(frames.Length - 1)) * duration;

        pastFrame = -1;
    }

    public void SetLastFrame()
    {
        int targetFrame = frames.Length - 1;
        SetFrame(targetFrame);
    }

    /// <summary>
    /// Stops Update loop from updating currFrame or display
    /// </summary>
    public void Pause()
    {
        videoMoving = false;
    }

    /// <summary>
    /// Continues Update loop in updating currFrame and display
    /// </summary>
    public void Play()
    {
        videoMoving = true;
    }


    /// <summary>
    /// Changes direction of video playback
    /// </summary>
    /// <param name="value">If true, will playback in reverse.</param>
    public void SetReverse(bool value)
    {
        doReverse = value;
    }

    // IEnumerator UpdateFrame()
    // {
    //     while(true)
    //     {
    // 		if (videoMoving == true && !didSetPercentThisFrame)
    //         {
    //             // if (updateRate >= 0)
    //             //     currFrame++;
    //             // else
    //             //     currFrame--;

    //             currFrame = Mathf.FloorToInt(currTimecode / Mathf.Abs(updateRate));

    //             if (currFrame >= frames.Length - 1)
    //             {
    //                 if (doLoop)
    //                     currFrame = 0;
    //                 else
    //                     currFrame = frames.Length - 1;
    //             }
    //             else if (currFrame < 0)
    //             {
    //                 if (doLoop)
    //                     currFrame = frames.Length - 1;
    //                 else
    //                     currFrame = 0;
    //             }

    //             rawImage.texture = frames[currFrame];

    // 			targetPercent = currFrame / frames.Length;

    //             //currTimecode += updateRate * Time.deltaTime;
    //             if (updateRate > 0)
    //                 currTimecode += Time.deltaTime;
    //             else
    //                 currTimecode -= Time.deltaTime;
    //         }

    //         //yield return new WaitForSeconds(Mathf.Abs(updateRate));  //TODO: this just doesn't work as intended with smaller values, as it seems sort of framerate bound
    //         yield return null;  //TODO: this just doesn't work as intended with smaller values, as it seems sort of framerate bound
    //     }
    // }

    protected virtual void Update()
    {
        if (frames == null || frames.Length < 1)
            return;

        if (videoMoving && !didSetPercentThisFrame)
        {
            // if (updateRate >= 0)
            //     currFrame++;
            // else
            //     currFrame--;

            currFrame = Mathf.FloorToInt(currTimecode / Mathf.Abs(updateRate));

            if (currFrame > frames.Length - 1)
            {
                //Debug.Log("reached end of image sequence.   currFrame = " + currFrame + "   doLoop = " + doLoop);

                if (doLoop)
                {
                    pastFrame = currFrame;

                    currFrame = 0;
                    currTimecode = 0f;
                    //currTimecode -= frames.Length * updateRate;
                }
                else
                {
                    if (pastFrame <= frames.Length - 1)
                        onLastFrameReached.Invoke();

                    pastFrame = currFrame;

                    currFrame = frames.Length - 1;
                    currTimecode = frames.Length * updateRate;
                }
            }
            else if (currFrame < 0)
            {
                if (doLoop)
                {
                    pastFrame = currFrame;

                    currFrame = frames.Length - 1;
                    currTimecode = frames.Length * Mathf.Abs(updateRate);
                    //currTimecode += frames.Length * updateRate;
                }
                else
                {
                    if (pastFrame >= 0)
                    {
                        onFirstFrameReached.Invoke();
                    }

                    pastFrame = currFrame;

                    currFrame = 0;
                    currTimecode = 0f;
                }
            }
            else
            {
                pastFrame = currFrame;
            }

            if (rawImage != null)
                rawImage.texture = frames[currFrame] != null ?
                    frames[currFrame].texture :
                    null;

            if (uiImage != null)
                uiImage.sprite = frames[currFrame];

            if (spriteRenderer != null)
                spriteRenderer.sprite = frames[currFrame];

            targetPercent = currFrame / frames.Length;

            if (updateRate > 0)
            {
                if (doReverse)
                    currTimecode -= Time.deltaTime;
                else
                    currTimecode += Time.deltaTime;
            }
                
            else if (updateRate < 0)
            {
                if (doReverse)
                    currTimecode += Time.deltaTime;
                else
                    currTimecode -= Time.deltaTime;
            }
                
        }
        else
        {
            float currPercent = 1f * currFrame / frames.Length;

            //print("curr: " + currPercent + ", targ: " + targetPercent);

            //float percent = Mathf.Lerp(currPercent, targetPercent, Time.deltaTime * 8f);
            float percent = targetPercent;

            int index = Mathf.FloorToInt(frames.Length * percent);

            currFrame = Mathf.Clamp(index, 0, frames.Length - 1);

            if (frames[currFrame] != null)
            {
                if (rawImage != null)
                    rawImage.texture = frames[currFrame].texture;

                if (uiImage != null)
                    uiImage.sprite = frames[currFrame];

                if (spriteRenderer != null)
                    spriteRenderer.sprite = frames[currFrame];
            }

            currTimecode = currFrame / frames.Length;

            pastFrame = currFrame;
        }
    }

    private void LateUpdate()
    {
        didSetPercentThisFrame = false;
    }
}

public class mySorter : IComparer
{
    int IComparer.Compare(object a, object b)
    {
        string nameA = ((Texture2D)a).name;
        string nameB = ((Texture2D)b).name;

        int indexA, indexB = 0;

        //int.TryParse(nameA.Substring(nameA.IndexOf(" ") + 1), out indexA);
        //int.TryParse(nameB.Substring(nameB.IndexOf(" ") + 1), out indexB);

        int.TryParse(nameA.Substring(nameA.IndexOf(" ", StringComparison.CurrentCulture) + 1), out indexA);
        int.TryParse(nameB.Substring(nameB.IndexOf(" ", StringComparison.CurrentCulture) + 1), out indexB);

        return (indexA.CompareTo(indexB));
    }
}