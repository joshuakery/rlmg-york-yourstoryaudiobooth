using UnityEngine;
using System.Collections;
using TMPro;

public class ImageSequenceLoading : MonoBehaviour
{
    private Canvas loadingDisplay;
    private TMP_Text displayText;

    private bool isWaitingToHideLoadingDisplay = false;

    private void Awake()
    {
        if (loadingDisplay == null)
            loadingDisplay = GetComponentInChildren<Canvas>();

        if (displayText == null)
            displayText = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (loadingDisplay == null)
            return;

        if (ImageSequence.sequencesAreLoading)
        {
            loadingDisplay.enabled = true;

            if (displayText != null)
                displayText.text = System.String.Format(
                    "Loading {0} image sequences... {1}/{2} frames loaded.",
                    ImageSequence.GetTotalEnabled(),
                    ImageSequence.GetTotalFramesLoaded(),
                    ImageSequence.GetTotalFramesToLoad()
                );

            StopAllCoroutines();
            isWaitingToHideLoadingDisplay = false;
        }    
        else
        {
            if (loadingDisplay.enabled && !isWaitingToHideLoadingDisplay)
            {
                StartCoroutine(HideLoadingDisplay());
            }
        }
            
    }

    private IEnumerator HideLoadingDisplay()
    {
        isWaitingToHideLoadingDisplay = true;

        yield return new WaitForSeconds(1f);

        loadingDisplay.enabled = false;

        isWaitingToHideLoadingDisplay = false;
    }
}
