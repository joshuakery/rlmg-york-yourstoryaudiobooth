using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.GenericUI
{
    [RequireComponent(typeof(RectTransform))]
    public class RTFollowSize : MonoBehaviour
    {
        public RectTransform leader;

        public Vector2 lastLeaderSizeDelta;

        private RectTransform rt;

        void Start()
        {
            rt = GetComponent<RectTransform>();

            if (leader != null)
            {
                rt.sizeDelta = leader.sizeDelta;

                lastLeaderSizeDelta = leader.sizeDelta;
            }

        }

        void Update()
        {
            if (rt != null && leader != null)
            {
                if (lastLeaderSizeDelta != rt.sizeDelta)
                {
                    rt.sizeDelta = leader.sizeDelta;

                    lastLeaderSizeDelta = leader.sizeDelta;
                }
            }

        }


    }
}
