using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace rlmg.utils
{
    public class Key : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public delegate void OnKeyPressedEvent(string key_value);
        public event OnKeyPressedEvent OnKeyPressed;

        public string _name { get; set; }
        public string _rowno { get; set; }

        
        public GameObject bg, key_symbol;

        public List<string> key_ids;
        //public List<KeyCode> keycode_relations;

        //public List<KeyCodeHandler> key_values;

        
        public Color color_bg, color_symbol;
        

        protected virtual void Awake()
        {
            _name = transform.name;
            _rowno = transform.parent.name;

            if (key_symbol != null)
            {
                if (key_symbol.TryGetComponent(out TextMeshProUGUI tmp_text))
                {
                    tmp_text.text = key_ids[1];
                }
            }
        }

        public virtual void InitKey() 
        {
            ShowSmallShiftOff();
        }

        public void ShowSmallShiftOff()
        {
            key_symbol.GetComponent<TextMeshProUGUI>().text = key_ids[1];
        }

        public void ShowCapsShiftOn()
        {
            key_symbol.GetComponent<TextMeshProUGUI>().text = key_ids[0];
        }

        public void ShowOther()
        {
            key_symbol.GetComponent<TextMeshProUGUI>().text = key_ids[2];
        }

        public virtual void OnPointerDown(PointerEventData e)
        {
            bg.GetComponent<Image>().color = color_symbol;
            key_symbol.GetComponent<TextMeshProUGUI>().color = color_bg;
        }

        public virtual void OnPointerUp(PointerEventData e)
        {
            bg.GetComponent<Image>().color = color_bg;

            key_symbol.GetComponent<TextMeshProUGUI>().color = color_symbol;

            KeyboardKeyDown(key_symbol.GetComponent<TextMeshProUGUI>().text);
        }

        public void KeyboardKeyDown(string _text)
        {
            //Debug.Log("Text coming in is:  " + _text);
            OnKeyPressed(_text);
        }
    }

}
