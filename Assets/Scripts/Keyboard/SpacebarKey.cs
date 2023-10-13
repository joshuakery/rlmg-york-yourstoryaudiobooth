using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace rlmg.utils
{
    public class SpacebarKey : Key
    {

/*        protected override void Awake()
        {
            return;
        }*/

        public override void OnPointerDown(PointerEventData eventdata)
        {
            bg.GetComponent<Image>().color = color_symbol;

        }

        public override void OnPointerUp(PointerEventData eventdata)
        {
            bg.GetComponent<Image>().color = color_bg;
            KeyboardKeyDown(key_ids[0]);
        }
    }
}

