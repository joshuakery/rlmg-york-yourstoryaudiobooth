using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoshKery.GenericUI;
using AirtableUnity.PX.Model;
using Newtonsoft.Json;
using rlmg.logging;

namespace JoshKery.GenericUI.Repositionable
{
    public abstract class AdjustPositions<R> : MonoBehaviour where R : Repositionable
    {
        public bool canAdjust = true;

        [SerializeField]
        private GameObject adjustPositionsMenu;

        [SerializeField]
        protected GameObject repositionablesContainer;


        [SerializeField]
        private bool doDebug = false;

        protected virtual void Start()
        {
            canAdjust = false;
            ShowHide();
        }

        void Update()
        {
            if (doDebug)
                HandleDebugInput();
        }

        public void HandleDebugInput()
        {
            if (Input.GetKeyDown(KeyCode.P) &&
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            )
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            canAdjust = !canAdjust;
            ShowHide();
        }

        public void Toggle(bool value)
        {
            bool doRefresh = value != canAdjust;
            
            canAdjust = value;

            if (doRefresh)
                ShowHide();
        }

        protected virtual void ShowHide()
        {
            if (adjustPositionsMenu != null)
                adjustPositionsMenu.SetActive(canAdjust);

            if (repositionablesContainer != null)
            {
                R[] repositionables = repositionablesContainer.GetComponentsInChildren<R>();

                foreach (R repositionable in repositionables)
                {
                    repositionable.ShowHandle(canAdjust);
                }
            }
        }

        #region Button Methods
        public virtual void OnSave()
        {
            SavePositions();
        }

        public virtual void OnReset()
        {
            ResetPositions();
        }

        protected abstract void SavePositions();

        protected abstract void ResetPositions();
        #endregion


    }

}

