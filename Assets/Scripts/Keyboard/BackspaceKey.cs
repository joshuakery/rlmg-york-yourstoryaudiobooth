using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace rlmg.utils
{
    public class BackspaceKey : Key
    {

        protected override void Awake()
        {
            _name = transform.name;
        }

        public override void OnPointerDown(PointerEventData eventdata)
        {
            bg.GetComponent<Image>().color = color_symbol;
            key_symbol.GetComponent<Image>().color = color_bg;
            
        }

        public override void OnPointerUp(PointerEventData eventdata)
        {
            bg.GetComponent<Image>().color = color_bg;
            key_symbol.GetComponent<Image>().color = color_symbol;
            KeyboardKeyDown(key_ids[0]);
        }
    }
}

