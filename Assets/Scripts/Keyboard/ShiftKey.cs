using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace rlmg.utils
{
    public class ShiftKey : Key
    {
        [HideInInspector]
        public bool isCaps = false;

        public GameObject key_symbol_filled;

        private bool _interactable = true;
        public bool interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;

                if (value)
                    StyleInteractable();
                else
                    StyleNonInteractable();
            }
        }


        protected override void Awake()
        {
            _name = transform.name;
            ShiftKeyToggle(false);
        }

        public override void InitKey()
        {
            ShiftKeyToggle(false);
        }

        public override void OnPointerDown(PointerEventData eventdata)
        {
            if (interactable)
            {
                //the background responds to the down state
                bg.GetComponent<Image>().color = color_bg;

                ShiftKeyToggleGraphics(!isCaps);
            }
        }

        public override void OnPointerUp(PointerEventData eventdata)
        {
            if (interactable)
            {
                //the background reverts...
                bg.GetComponent<Image>().color = color_symbol;

                if (!isCaps)
                {
                    ShiftKeyToggle(true, true);

                }
                else
                {
                    ShiftKeyToggle(false, true);
                }
            }
        }

        private void StyleInteractable()
        {
            bg.GetComponent<Image>().color = color_symbol;
            ShiftKeyToggleGraphics(isCaps);
        }

        private void StyleNonInteractable()
        {
            bg.GetComponent<Image>().color = Color.gray;
            ShiftKeyToggleGraphics(false);
        }

        public void ShiftKeyToggle(bool _toggle, bool doKeyboardKeyDown = false)
        {
            isCaps = _toggle;
            ShiftKeyToggleGraphics(_toggle);

            if (doKeyboardKeyDown)
                KeyboardKeyDown(key_ids[0]);
        }

        private void ShiftKeyToggleGraphics(bool _toggle)
        {
            //if true Caps is on
            if(_toggle)
            {
                
                key_symbol.GetComponent<Image>().enabled = false;
                key_symbol_filled.GetComponent<Image>().enabled = true;
            } else
            {
                
                key_symbol_filled.GetComponent<Image>().enabled = false;
                key_symbol.GetComponent<Image>().enabled = true;
            }
        }
    }
}

