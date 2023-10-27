using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace rlmg.utils
{
    public class AlphaNumbericToggleKey : Key
    {
        //[HideInInspector]
        public bool isAlpha = true;

        protected override void Awake()
        {
            _name = transform.name;
            AlphaNumKeyToggle(true);
        }

        public override void InitKey()
        {
            AlphaNumKeyToggle(true);
        }

        public override void OnPointerDown(PointerEventData eventdata)
        {
            bg.GetComponent<Image>().color = color_symbol;
            key_symbol.GetComponent<TMP_Text>().color = color_bg;
        }

        public override void OnPointerUp(PointerEventData eventdata)
        {
            bg.GetComponent<Image>().color = color_bg;
            key_symbol.GetComponent<TMP_Text>().color = color_symbol;

            if (!isAlpha)
            {
                AlphaNumKeyToggle(true, true);
            }
            else
            {
                AlphaNumKeyToggle(false, true);
            }
        }

        public void AlphaNumKeyToggle(bool _toggle, bool doKeyboardKeyDown = false)
        {
            isAlpha = _toggle;
            AlphaNumKeyToggleGraphics(_toggle);

            if (doKeyboardKeyDown)
                KeyboardKeyDown(_name);
        }

        private void AlphaNumKeyToggleGraphics(bool _toggle)
        {
            if (_toggle)
            {
                key_symbol.GetComponent<TextMeshProUGUI>().text = "123";
            }
            else
            {
                key_symbol.GetComponent<TextMeshProUGUI>().text = "ABC";
            }
        }
    }
}

