using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PageManagerHelper : MonoBehaviour
    {
        private PageManager pageManager;

        [SerializeField]
        private UISequenceManager pageSequenceManager;

        private void Awake()
        {
            pageManager = FindObjectOfType<PageManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.PromptSelection);
            }
            
                
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.Recording);
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.Editing);
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.AgeSelection);
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.EmailEntry);
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.NameEntry);
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.ThankYou);
            }
               
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.Error);
            }
                
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                pageSequenceManager.CompleteCurrentSequence();
                pageManager.OpenPage(PageManager.Page.None);
            }
                
        }
    }
}


