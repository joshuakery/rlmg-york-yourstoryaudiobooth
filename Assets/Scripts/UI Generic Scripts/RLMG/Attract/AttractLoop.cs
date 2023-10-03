using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.Video;

public class AttractLoop : MonoBehaviour
{
	public AttractScreen attractScreen;

	public VideoPlayerManager videoPlayerManager;
	public CanvasGroup attractUICanvasGroup;
	public CanvasGroup blackCanvasGroup;

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    public VideoClip attractVideoClip;
    public string attractVideoPath;

	public UnityEvent onBlackFadeInStarted;
	public UnityEvent onBlackFadeInComplete;
	public UnityEvent onBlackFadeOutStarted;
	public UnityEvent onBlackFadeOutComplete;

	public UnityEvent onFadeInStarted;
	public UnityEvent onFadeInComplete;
	public UnityEvent onFadeOutStarted;
	public UnityEvent onFadeOutComplete;

	void Awake()
	{
		if (attractScreen == null)
			attractScreen = GetComponent<AttractScreen>();

		if (attractScreen != null)
		{
			attractScreen.onOpen += OnOpen;
			attractScreen.onClose += OnClose;
		}

		if (videoPlayerManager == null)
			videoPlayerManager = GetComponentInChildren<VideoPlayerManager>();

		if (attractUICanvasGroup != null)
		{
			attractUICanvasGroup.alpha = 1f;
			attractUICanvasGroup.gameObject.SetActive(true);
		}

		if (blackCanvasGroup != null)
        {
			blackCanvasGroup.alpha = 0f;
			blackCanvasGroup.gameObject.SetActive(false);
        }
	}

	//void OnEnable()
	void OnOpen(AttractScreen attract)
	{
		StopAllCoroutines();

		StartCoroutine(StartAttractLoop());
	}

	private IEnumerator StartAttractLoop()
	{
		if (attractUICanvasGroup != null)
			attractUICanvasGroup.gameObject.SetActive(true);

		blackCanvasGroup.gameObject.SetActive(false);

		if (attractScreen.doFadeToBlackFirst && !videoPlayerManager.videoPlayer.isPlaying)
        {
			onBlackFadeInStarted.Invoke();
			blackCanvasGroup.gameObject.SetActive(true);
			yield return StartCoroutine(FadeIn(blackCanvasGroup));
			onBlackFadeInComplete.Invoke();
        }
		
		if (!videoPlayerManager.videoPlayer.isPlaying)
		{
			if (attractVideoClip != null)
            {
                videoPlayerManager.LoadAndPlayVideo(attractVideoClip);
            }
            else if (!string.IsNullOrEmpty(attractVideoPath))
            {
                videoPlayerManager.LoadAndPlayVideo(attractVideoPath);
            }
            else
            {
                //Debug.LogError("No attract video clip or url found.");
            }


			while (!videoPlayerManager.videoPlayer.isPrepared)
			{
				yield return null;
			}

            //Debug.Log("Video finished preparing! Continue on.");
        }
        else
		{
            //Debug.Log("Video is already playing. Continue on.");
        }

        if (attractUICanvasGroup != null && attractUICanvasGroup.alpha < 1f)
        {
			onFadeInStarted.Invoke();
			yield return StartCoroutine(FadeInUI());
			onFadeInComplete.Invoke();
		}
			

		if (attractScreen.doFadeToBlackFirst && blackCanvasGroup.gameObject.activeInHierarchy)
		{
			onBlackFadeOutStarted.Invoke();
			yield return StartCoroutine(FadeOut(blackCanvasGroup));
			blackCanvasGroup.gameObject.SetActive(false);
			onBlackFadeOutComplete.Invoke();
		}
	}

	private IEnumerator FadeIn(CanvasGroup cg)
    {
		if (cg == null)
			yield break;

		float t = 0f;

		while (t < fadeInDuration)
		{
			cg.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);

			t += Time.deltaTime;

			yield return null;
		}

		cg.alpha = 1f;
	}

	private IEnumerator FadeOut(CanvasGroup cg)
	{
		if (cg == null)
			yield break;

		float t = 0f;

		while (t < fadeOutDuration)
		{
			cg.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);

			t += Time.deltaTime;

			yield return null;
		}

		cg.alpha = 0f;
	}

	private IEnumerator FadeInUI()
	{
		if (attractUICanvasGroup == null)
			yield break;

		//attractUICanvasGroup.gameObject.SetActive(true);

		float t = 0f;

		while (t < fadeInDuration)
		{
			attractUICanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);

			t += Time.deltaTime;

			yield return null;
		}

		attractUICanvasGroup.alpha = 1f;
	}

	private IEnumerator FadeOutUI()
	{
		if (attractUICanvasGroup == null)
			yield break;

		float t = 0f;

		while (t < fadeOutDuration)
		{
			attractUICanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);

			t += Time.deltaTime;

			yield return null;
		}

		attractUICanvasGroup.alpha = 0f;

		//attractUICanvasGroup.interactable = attractUICanvasGroup.blocksRaycasts = false;
		attractUICanvasGroup.gameObject.SetActive(false);


		videoPlayerManager.videoPlayer.Stop();
	}

	//void OnDisable()
	void OnClose(AttractScreen attract)
	{
//		if (!quitApp)
//		{
			StopAllCoroutines();

		//reset things here
		//		}

		StartCoroutine(StopAttractLoop());
	}

	private IEnumerator StopAttractLoop()
    {
		StartCoroutine(FadeOut(blackCanvasGroup));
		blackCanvasGroup.gameObject.SetActive(false);

		yield return StartCoroutine(FadeOutUI());
		
		onFadeOutComplete.Invoke();
	}

//	private bool quitApp = false;
//
//	void OnApplicationQuit()
//	{
//		quitApp = true;
//	}
}
