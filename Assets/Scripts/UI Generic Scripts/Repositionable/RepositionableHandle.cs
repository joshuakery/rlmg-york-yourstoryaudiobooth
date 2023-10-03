using UnityEngine;
using UnityEngine.EventSystems;

namespace JoshKery.GenericUI.Repositionable
{
    public class RepositionableHandle : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public delegate void DragDelegate(PointerEventData eventData, RepositionableHandle handle);
        public DragDelegate onDragDelegate;

        public void OnDrag(PointerEventData eventData)
        {
            if (onDragDelegate != null)
                onDragDelegate.Invoke(eventData, this);
        }

        public delegate void EndDragDelegate(PointerEventData eventData, RepositionableHandle handle);
        public EndDragDelegate onEndDragDelegate;

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDragDelegate != null)
                onEndDragDelegate.Invoke(eventData, this);
        }
    }
}

