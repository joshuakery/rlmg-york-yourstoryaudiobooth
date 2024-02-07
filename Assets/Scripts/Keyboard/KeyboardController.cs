using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

namespace rlmg.utils
{
    public class KeyCodeHandler
    {
        public string id;
        public KeyCode kc;
    }

    public class KeyboardController : MonoBehaviour
    {
        public List<GameObject> keys;

        public GameObject shift_key, alphanumeric_key, spacebar_key, backspace_key, dotcom_key;

        public GameObject currentInputField;

        [SerializeField]
        private TMP_InputField[] inputFields;

        //todo impose char limit?
        public GameObject keyboard;

        public delegate void OnInitEvent();
        public OnInitEvent onInit;

        public delegate void OnSubmitEvent(string[] input);
        public OnSubmitEvent onSubmit;

        public bool doPresentUppercaseLettersToStart = false;
        public bool doPresentLettersToStart = false;

        private void Awake()
        {
            keys = GetComponentsInChildren<Key>()
                .Select(k => k.gameObject)
                .Where(
                    g =>  g != shift_key &&
                          g != alphanumeric_key &&
                          g != spacebar_key &&
                          g != backspace_key &&
                          g != dotcom_key
                          
                )
                .ToList();

            Init();
        }

        public void Init()
        {
            ClearAll();

            onInit?.Invoke();
        }

        private void OnEnable()
        {
            EnableKeyboard();
        }

        private void OnDisable()
        {
            DisableKeyboard();
        }

        public void EnableKeyboard()
        {
            //Do not allow double subscriptions
            DisableKeyboard();

            transform.GetComponent<CanvasGroup>().interactable = true;
            transform.GetComponent<CanvasGroup>().blocksRaycasts = true;

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] != null)
                    keys[i].GetComponent<Key>().OnKeyPressed += HandleKeyPressed;
            }

            if (shift_key != null)
                shift_key.GetComponent<Key>().OnKeyPressed += HandleKeyPressed;

            if (alphanumeric_key != null)
                alphanumeric_key.GetComponent<Key>().OnKeyPressed += HandleKeyPressed;

            if (spacebar_key != null)
                spacebar_key.GetComponent<Key>().OnKeyPressed += HandleKeyPressed;

            if (backspace_key != null)
                backspace_key.GetComponent<Key>().OnKeyPressed += HandleKeyPressed;

            if (dotcom_key != null) 
                dotcom_key.GetComponent<Key>().OnKeyPressed += HandleKeyPressed;

            SetKeyboardToStartState();
        }

        public void DisableKeyboard()
        {
            transform.GetComponent<CanvasGroup>().interactable = false;
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] != null)
                    keys[i].GetComponent<Key>().OnKeyPressed -= HandleKeyPressed;
            }

            if (shift_key != null)
                shift_key.GetComponent<Key>().OnKeyPressed -= HandleKeyPressed;

            if (alphanumeric_key != null)
                alphanumeric_key.GetComponent<Key>().OnKeyPressed -= HandleKeyPressed;

            if (spacebar_key != null)
                spacebar_key.GetComponent<Key>().OnKeyPressed -= HandleKeyPressed;

            if (backspace_key != null)
                backspace_key.GetComponent<Key>().OnKeyPressed -= HandleKeyPressed;

            if (dotcom_key != null)
                dotcom_key.GetComponent<Key>().OnKeyPressed -= HandleKeyPressed;

        }

        private void SetKeyboardToStartState()
        {
            if (doPresentLettersToStart || alphanumeric_key == null)
            {
                if (doPresentUppercaseLettersToStart)
                {
                    if (shift_key != null)
                        shift_key.GetComponent<ShiftKey>().ShiftKeyToggle(true, true);
                    else if (alphanumeric_key != null)
                        alphanumeric_key.GetComponent<AlphaNumbericToggleKey>().AlphaNumKeyToggle(true, true);
                }
                else
                {
                    if (shift_key != null)
                        shift_key.GetComponent<ShiftKey>().ShiftKeyToggle(false, true);
                    else if (alphanumeric_key != null)
                        alphanumeric_key.GetComponent<AlphaNumbericToggleKey>().AlphaNumKeyToggle(true, true);
                }
            }
            else
            {
                if (alphanumeric_key != null)
                    alphanumeric_key.GetComponent<AlphaNumbericToggleKey>().AlphaNumKeyToggle(false, true);
            }
        }

        private void HandleKeyPressed(string _value)
        {
            Debug.Log("The Key pressed is: " + _value);

            if(_value == "shift")
            {
                if (shift_key == null)
                    return;
                
                if(shift_key.GetComponent<ShiftKey>().isCaps)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        keys[i].GetComponent<Key>().ShowCapsShiftOn();
                    }
                }
                else
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        keys[i].GetComponent<Key>().ShowSmallShiftOff();
                    }
                }
                
            } else if (_value == "123")
            {
                if (alphanumeric_key == null)
                    return;

                if(alphanumeric_key.GetComponent<AlphaNumbericToggleKey>().isAlpha)
                {
                    if (shift_key != null)
                    {
                        shift_key.GetComponent<ShiftKey>().interactable = true;
                        //Simulate pressing the shift key so that the graphcis are correct 
                        shift_key.GetComponent<ShiftKey>().ShiftKeyToggle(shift_key.GetComponent<ShiftKey>().isCaps, true);
                    }
                    else //we still need the keys to change
                    {
                        for (int i = 0; i < keys.Count; i++)
                        {
                            if (doPresentUppercaseLettersToStart)
                                keys[i].GetComponent<Key>().ShowCapsShiftOn();
                            else
                                keys[i].GetComponent<Key>().ShowSmallShiftOff();
                        }
                    }
                }
                else
                {
                    if (shift_key != null)
                        shift_key.GetComponent<ShiftKey>().interactable = false;

                    for (int i = 0; i < keys.Count; i++)
                    {
                        keys[i].GetComponent<Key>().ShowOther();
                    }
                }
                
            } else if(_value == "backspace")
            {
                string _st = currentInputField.GetComponent<TMP_InputField>().text;
                if(_st.Length > 0)
                {
                    _st = _st.Substring(0, _st.Length - 1);
                    currentInputField.GetComponent<TMP_InputField>().text = _st;
                }

            } else if (_value == "spacebar")
            {
                if (currentInputField != null)
                    currentInputField.GetComponent<TMP_InputField>().text += " ";

            } else if (_value == ".com")
            {
                if (currentInputField != null)
                    currentInputField.GetComponent<TMP_InputField>().text += ".com";

            }
            else
            {
                if (currentInputField != null)
                    currentInputField.GetComponent<TMP_InputField>().text += _value; 
            }
        }

        public void ClearAll()
        {
            if (currentInputField != null)
                currentInputField.GetComponent<TMP_InputField>().text = "";

            foreach (TMP_InputField inputField in inputFields)
            {
                if (inputField != null)
                    inputField.text = "";
            }
        }

        public void OnInputSwitch(TMP_InputField inputField)
        {
            if (currentInputField == null)
                return;

            currentInputField = inputField.gameObject;

            if (inputField.text.Length == 0)
            {
                SetKeyboardToStartState();
            }
        }

        public void OnSubmit()
        {
            Submit();
        }

        public void Submit()
        {
            if (inputFields != null && inputFields.Length > 0)
            {
                onSubmit(inputFields.Select(f => f != null ? f.text : "").ToArray());
            }
        }
    }
}
        