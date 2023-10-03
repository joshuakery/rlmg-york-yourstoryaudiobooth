using System.Collections;
using System.Collections.Generic;
using rlmg.logging;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSaver : MonoBehaviour
{
    //private float timeOfLastInput = 0f;

    public bool doOnEnable = true;

    public float screenSaverFrequency = 1800f;
    private float lastScreenSaverTime = 0f;

    public Image myImage;

	public float fadeDuration = 1f;
	public float holdDuration = 1f;

	void Awake()
	{
		if (myImage == null)
			myImage = GetComponentInChildren<Image>();
	}

    void Update()
    {
        if (Input.anyKey)
            //timeOfLastInput = Time.time;
            lastScreenSaverTime = Time.time;

        if (Time.time - lastScreenSaverTime > screenSaverFrequency)
        {
            DoScreenSaver();
        }
    }

    void OnEnable()
	{
        if (doOnEnable)
            DoScreenSaver();
    }

	void OnDisable()
	{
		StopAllCoroutines();

		if (myImage != null)
			myImage.color = Color.clear;
	}

	[ContextMenu ("Test")]
	void Test()
	{
        DoScreenSaver();
    }

    void DoScreenSaver()
    {
        StopAllCoroutines();

        lastScreenSaverTime = Time.time;

        if (myImage != null)
            myImage.color = Color.clear;

        StartCoroutine(DoSequence());

        RLMGLogger.Instance.Log("Did Screen Saver");
    }

    private IEnumerator DoSequence()
	{
		if (myImage == null)
			yield break;

		float t = 0f;

		while (t < fadeDuration)
		{
			myImage.color = Color.Lerp(Color.clear, Color.black, t / fadeDuration);

			t += Time.deltaTime;

			yield return null;
		}

		yield return new WaitForSeconds(holdDuration);

		t = 0f;

		while (t < fadeDuration)
		{
			myImage.color = Color.Lerp(Color.black, Color.white, t / fadeDuration);

			t += Time.deltaTime;

			yield return null;
		}

		yield return new WaitForSeconds(holdDuration);

		t = 0f;

		while (t < fadeDuration)
		{
			myImage.color = Color.Lerp(Color.white, Color.clear, t / fadeDuration);

			t += Time.deltaTime;

			yield return null;
		}

		myImage.color = Color.clear;

		this.gameObject.SetActive(false);  //disable myself after the sequence
	}
}
