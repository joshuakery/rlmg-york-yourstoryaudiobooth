using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using rlmg.utils;

namespace JoshKery.York.AudioRecordingBooth
{
    public class InputFieldSwitchable : MonoBehaviour
    {
        [SerializeField]
        private KeyboardController keyboardController;

        [SerializeField]
        private Toggle toggle;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private Button inputClearButton;

        public delegate void OnInputTouchedEvent(GameObject input_field);
        public event OnInputTouchedEvent OnInputTouched;

        private void OnEnable()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(ToggleInputField);
            }

            if (inputClearButton != null)
            {
                inputClearButton.onClick.AddListener(OnClear);
            }
        }

        private void OnDisable()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(ToggleInputField);
            }

            if (inputClearButton != null)
            {
                inputClearButton.onClick.RemoveListener(OnClear);
            }
        }

        public virtual void InitInputField(bool doSelect = false)
        {
            if (toggle != null)
                toggle.isOn = doSelect;
        }

        public void ToggleInputField(bool value)
        {
            if (value)
                SelectInputField();
            else
                DeselectInputField();
        }

        public void SelectInputField()
        {
            if (keyboardController != null && inputField != null)
                keyboardController.OnInputSwitch(inputField);
        }

        public void DeselectInputField()
        {
            
        }

        public void OnClear()
        {
            if (inputField != null)
                inputField.text = "";

            if (toggle != null)
                toggle.isOn = true;
        }
    }
}


