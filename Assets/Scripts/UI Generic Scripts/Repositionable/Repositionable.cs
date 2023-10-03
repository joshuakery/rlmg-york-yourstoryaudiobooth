using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.

namespace JoshKery.GenericUI.Repositionable
{
    public class Repositionable : MonoBehaviour
    {
        [SerializeField]
        private GameObject handlePrefab;

        private RepositionableHandle handle;

        private RectTransform rt;

        protected virtual void Awake()
        {
            CreateHandle();

            rt = GetComponent<RectTransform>();
        }

        private void CreateHandle()
        {
            GameObject handleGameObject = Instantiate(handlePrefab, transform);
            handle = handleGameObject.GetComponentInChildren<RepositionableHandle>();

            if (handle != null)
            {
                handle.onDragDelegate += MoveWithMouse;
            }
        }

        private void MoveWithMouse(PointerEventData eventData, RepositionableHandle handle)
        {
            if (rt != null)
                rt.anchoredPosition = rt.anchoredPosition + eventData.delta;
        }

        public void ShowHandle(bool doShow)
        {
            if (handle != null)
                handle.gameObject.SetActive(doShow);
        }

        public Vector2 GetAnchoredPosition()
        {
            if (rt != null)
                return rt.anchoredPosition;
            else
                return new Vector2(0, 0);
        }

        public void ResetPosition(Vector2 reset)
        {
            if (rt != null)
                rt.anchoredPosition = reset;
        }

    }
}

