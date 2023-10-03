using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class AttractScreen : MonoBehaviour
{
	//public GameObject attractScreen;  //ideally make this a child object, as this script needs to stay enabled
 //   public GameObject hackyVideoCoverUp;
    //public GameObject[] objsToSetActiveOnAttract;
	public Button attractScreenButton;
	public bool triggerOnDown = false;

	public float timeToActivate = 60f;
	public float timeOfLastInput = 0f;

    public VideoPlayer[] videoPlayersToWaitFor;

	private bool isOpen = false;
	public bool IsOpen { get { return isOpen; } }

	//private CanvasGroup attractScreenCanvasGroup;

	//public bool doOpacityFadeTransition = true;

	//public float transitionSpeedExpand = 1f;
	//public float transitionSpeedCollapse = 1f;
	//private float opacityPercent = 1f;

	//public UnityEvent onOpenEvent;
	//public UnityEvent onCloseEvent;

	public delegate void OnOpen(AttractScreen attractScreen);
	public OnOpen onOpen;

	public delegate void OnClose(AttractScreen attractScreen);
	public OnClose onClose;

	public bool doFadeToBlackFirst = true;

	//public bool resetAppAfterOpen = true;
	//public UnityEvent onResetEvent;

	private void Start()
	{
		if (attractScreenButton == null)
			attractScreenButton = GetComponentInChildren<Button>();

		if (attractScreenButton != null)
		{
			if (triggerOnDown)
			{
				EventTrigger eventTrigger = attractScreenButton.GetComponent<EventTrigger>();
				if (eventTrigger == null)
					eventTrigger = attractScreenButton.gameObject.AddComponent<EventTrigger>();
				if (eventTrigger != null)
				{
					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerDown;
					entry.callback.AddListener((data) => { CloseAttractScreen(); });
					eventTrigger.triggers.Add(entry);
				}
			}
			else
			{
				attractScreenButton.onClick.AddListener(() => CloseAttractScreen());
			}
		}

		//if (attractScreen != null)
		//{
		//	attractScreenCanvasGroup = attractScreen.GetComponent<CanvasGroup>();
		//}

		OpenAttractScreen();
	}

	void Update ()
	{
		//if (!isOpen)
		//{
			if ( Input.anyKey )
        {
			
			timeOfLastInput = Time.time;
		}
				

            foreach(VideoPlayer videoPlayer in videoPlayersToWaitFor)
            {
                if (videoPlayer != null && videoPlayer.isPlaying)
                {
                    timeOfLastInput = Time.time;
                    break;
                }
            }

            if (Time.time >= timeOfLastInput + timeToActivate)
				OpenAttractScreen();
		//}

		//if (doOpacityFadeTransition)
		//{
		//	if (isOpen && opacityPercent < 1f)
		//	{
		//		opacityPercent += Time.deltaTime * transitionSpeedExpand;
		//	}
		//	else if (!isOpen && opacityPercent > 0f)
		//	{
		//		opacityPercent -= Time.deltaTime * transitionSpeedCollapse;
		//	}

		//	if (opacityPercent < 0f)
		//	{
		//		opacityPercent = 0f;

		//		OnCloseTransitionComplete();
		//	}
		//	else if (opacityPercent > 1f)
		//	{
		//		opacityPercent = 1f;

		//		OnOpenTransitionComplete();
		//	}

		//	if (attractScreenCanvasGroup != null)
		//		attractScreenCanvasGroup.alpha = opacityPercent;
		//}
	}

	public void OpenAttractScreen()
	{
		Debug.Log("Opening attract...");

		timeOfLastInput = Time.time;

		if (attractScreenButton != null)
			attractScreenButton.gameObject.SetActive(true);

		//if (attractScreen != null)
		//	attractScreen.SetActive(true);

        //foreach (GameObject obj in objsToSetActiveOnAttract)
        //{
        //    obj.SetActive(true);
        //}

		//if (!doOpacityFadeTransition) { OnOpenTransitionComplete(); }
		
        isOpen = true;

        //if (onOpenEvent != null)
        //    onOpenEvent.Invoke();

        if (onOpen != null)
			onOpen(this);
	}

	public void CloseAttractScreen()
	{
		Debug.Log("close attract");

		isOpen = false;

		timeOfLastInput = Time.time;

		if (attractScreenButton != null)
			attractScreenButton.gameObject.SetActive(false);

		//if (!doOpacityFadeTransition)
		//{
		//	if (attractScreen != null)
		//		attractScreen.SetActive(false);

  //          foreach (GameObject obj in objsToSetActiveOnAttract)
  //          {
  //              obj.SetActive(false);
  //          }

  //          OnCloseTransitionComplete();
  //      }

		//if (onCloseEvent != null)
		//	onCloseEvent.Invoke();

		if (onClose != null)
			onClose(this);
	}

	//private void OnOpenTransitionComplete()
	//{
	//	Debug.Log("on open transition complete");
	//	if (resetAppAfterOpen)
	//		ResetApp();

 //       if (hackyVideoCoverUp != null)
 //           hackyVideoCoverUp.SetActive(true);
 //   }

	//private void OnCloseTransitionComplete()
	//{
	//	if (doOpacityFadeTransition)
	//	{
	//		if (attractScreen != null)
	//			attractScreen.SetActive(false);

 //           foreach(GameObject obj in objsToSetActiveOnAttract)
 //           {
 //               obj.SetActive(false);
 //           }

 //           if (hackyVideoCoverUp != null)
 //               hackyVideoCoverUp.SetActive(false);
	//	}
	//}

	//void ResetApp()
	//{
	//	if (onResetEvent != null)
	//	{
	//		onResetEvent.Invoke();
	//	}
	//}
}
