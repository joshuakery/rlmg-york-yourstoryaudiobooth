using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    public class UISequenceManagerHelper : MonoBehaviour
    {
        public UISequenceManager uiSequenceManager;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (uiSequenceManager != null)
                    uiSequenceManager.CompleteCurrentSequence();
            }
        }
    }
}

