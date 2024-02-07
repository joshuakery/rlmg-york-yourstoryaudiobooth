using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class InputFieldClearButton : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private Image icon;

        private void OnEnable()
        {
            if (inputField != null)
                inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            if (inputField != null)
                inputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void Start()
        {
            if (icon != null && inputField != null)
            {
                if (string.IsNullOrEmpty(inputField.text))
                    icon.gameObject.SetActive(false);
                else
                    icon.gameObject.SetActive(true);
            }
        }

        private void OnValueChanged(string value)
        {
            if (icon != null)
            {
                if (string.IsNullOrEmpty(value))
                    icon.gameObject.SetActive(false);
                else
                    icon.gameObject.SetActive(true);
            }

        }
    }
}


