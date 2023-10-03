using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerManager : MonoBehaviour
{
	public VideoPlayer videoPlayer;

	public delegate void OnVideoComplete();
	public OnVideoComplete onVideoComplete;

    public bool clearRenderTextureOnLoad = true;
    public RawImage viewportImage;
    public float fadeUpDuration = 0.25f;
    public Color fadeStartColor = Color.black;
    public Color fadeEndColor = Color.white;

    public bool generateRenderTexture;
    public Vector2 generatedTextureSize;

    private float defaultVolume;

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = (VideoPlayer)FindObjectOfType(typeof(VideoPlayer));

        if (generateRenderTexture && videoPlayer != null)
        {
            videoPlayer.targetTexture = new RenderTexture((int)generatedTextureSize.x, (int)generatedTextureSize.y, 24);
        }

        //AudioSource videoAudioSource = videoPlayer.GetComponent<AudioSource>();
        //if (videoAudioSource != null)
        //{
        //    defaultVolume = videoAudioSource.volume;
        //}
    }

    private void OnEnable()
    {
        ClearRenderTexture();
    }

    public void Play()
    {
        if (videoPlayer != null)
        {
            //AudioSource videoAudioSource = videoPlayer.GetComponent<AudioSource>();
            //if (videoAudioSource != null)
            //{
            //    videoAudioSource.volume = defaultVolume;
            //}

            videoPlayer.Play();
        }
    }

    public void LoadAndPlayVideo(string videoPath)
	{
        LoadVideoByPath(videoPath);

		StopCoroutine(PlayVideoWhenLoaded());
		StartCoroutine(PlayVideoWhenLoaded());
	}

    public void LoadAndPlayVideo(VideoClip videoClip)
    {
        LoadVideoClip(videoClip);

        StopCoroutine(PlayVideoWhenLoaded());
        StartCoroutine(PlayVideoWhenLoaded());
    }

    public void LoadVideoAndPauseOnFirstFrame(string videoPath)
    {
        LoadVideoByPath(videoPath);

        StopCoroutine(PlayVideoWhenLoaded());
        StartCoroutine(PlayVideoWhenLoaded(true));
    }

    public void LoadVideoAndPauseOnFirstFrame(VideoClip videoClip)
    {
        LoadVideoClip(videoClip);

        StopCoroutine(PlayVideoWhenLoaded());
        StartCoroutine(PlayVideoWhenLoaded(true));
    }

    private void LoadVideoByPath(string videoPath)
    {
        if (videoPlayer != null)
        {
            videoPlayer.source = VideoSource.Url;

            videoPlayer.url = videoPath;
        }

        LoadVideo();
    }

    private void LoadVideoClip(VideoClip videoClip)
    {
        if (videoPlayer != null)
        {
            videoPlayer.source = VideoSource.VideoClip;

            videoPlayer.clip = videoClip;
        }

        LoadVideo();
    }

    private void LoadVideo()
	{
		if (videoPlayer != null)
		{
            if (videoPlayer.isPrepared)
            {
                videoPlayer.Stop();
            }

            if (!videoPlayer.isPrepared)
            {
                videoPlayer.Prepare();

                videoPlayer.loopPointReached += VideoCompleted;

                if (clearRenderTextureOnLoad)
                {
                    ClearRenderTexture();
                }

                //experimenting with blacking out the feed until the video is loaded and playing
                if (viewportImage == null)
                    viewportImage = GetComponentInChildren<RawImage>();
                if (viewportImage != null)
                {
                    viewportImage.color = fadeStartColor;
                }
            }
        }
	}

	IEnumerator PlayVideoWhenLoaded(bool pauseOnFirstFrame = false)
    {
        if (videoPlayer != null)
        {
            AudioSource videoAudioSource = videoPlayer.GetComponent<AudioSource>();
            if (videoAudioSource != null)
            {
                defaultVolume = videoAudioSource.volume;

                videoAudioSource.volume = 0f;
            }

            while (!videoPlayer.isPrepared)
                yield return null;

            videoPlayer.Play();

            videoPlayer.loopPointReached += VideoCompleted;

            if (pauseOnFirstFrame)
            {
                yield return new WaitForSeconds(0.1f);
                //yield return null;

                videoPlayer.Pause();

                yield return new WaitForSeconds(0.1f);
                //yield return null;
            }

            Debug.Log("resetting video player audio volume. defaultVolume = " + defaultVolume);

            if (videoAudioSource != null)
                videoAudioSource.volume = defaultVolume;

            //yield return new WaitForSeconds(1f);

            //experimenting with blacking out the feed until the video is loaded and playing
            if (viewportImage == null)
                viewportImage = GetComponentInChildren<RawImage>();

            if (viewportImage != null && viewportImage.color != fadeEndColor)
            {
                Color startColor = viewportImage.color;

                float time = 0f;

                while (time < fadeUpDuration)
                {
                    float progress = time / fadeUpDuration;

                    //Debug.Log("video fade-up progress = " + progress);

                    if (viewportImage != null)
                    {
                        viewportImage.color = Color.Lerp(startColor, fadeEndColor, progress);
                    }

                    time += Time.deltaTime;

                    yield return null;
                }

                if (viewportImage != null)
                {
                    viewportImage.color = fadeEndColor;
                }
            }
        }
	}

	private void VideoCompleted(VideoPlayer vp)
	{
		if (onVideoComplete != null)
			onVideoComplete();
	}

    private void ClearRenderTexture()
    {
        //ClearRenderTexture(Color.clear);
        ClearRenderTexture(fadeStartColor);
    }

    private void ClearRenderTexture(Color color)
    {
        //attempting to clear the render texture to avoid any vestiges from the previously played video
        //https://forum.unity.com/threads/how-to-clear-a-render-texture-to-transparent-color-all-bytes-at-0.147431/
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = videoPlayer.targetTexture;
        GL.Clear(true, true, color);
        RenderTexture.active = rt;
    }
}
