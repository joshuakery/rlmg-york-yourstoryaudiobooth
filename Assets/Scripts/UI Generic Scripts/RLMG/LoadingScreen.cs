using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup loadingScreen;
    public float fadeDur = 0.5f;

    private void Awake()
    //private void OnEnable()
    {
        loadingScreen.alpha = 1f;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        if (loadingScreen == null)
            yield break;

        float t = 0f;

        while (t < fadeDur)
        {
            float progress = t / fadeDur;

            loadingScreen.alpha = 1f - progress;

            t += Time.deltaTime;

            yield return null;
        }

        loadingScreen.alpha = 0f;

        loadingScreen.gameObject.SetActive(false);
    }
}
