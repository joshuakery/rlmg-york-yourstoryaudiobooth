using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.GenericUI
{
    [RequireComponent(typeof(RectTransform))]
    public class RTFollow : MonoBehaviour
    {
        public RectTransform leader;

        public Vector2 lastLeaderPosition;

        private RectTransform rt;

        // Start is called before the first frame update
        void Start()
        {
            rt = GetComponent<RectTransform>();

            if (leader != null)
            {
                rt.anchoredPosition = leader.anchoredPosition;

                lastLeaderPosition = leader.anchoredPosition;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (rt != null)
            {
                if (lastLeaderPosition != rt.anchoredPosition)
                {
                    rt.anchoredPosition = leader.anchoredPosition;
                    lastLeaderPosition = rt.anchoredPosition;
                }
            }

        }

        
    }
}


