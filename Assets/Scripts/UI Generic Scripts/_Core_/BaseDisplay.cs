using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JoshKery.GenericUI
{
    public abstract class BaseDisplay : MonoBehaviour
    {
        public List<BaseDisplay> childDisplays;
        public Transform childDisplaysContainer;
        public GameObject childDisplayPrefab;

        protected virtual void Awake()
        {
            ClearAllDisplays();
        }

        public virtual void ClearAllDisplays()
        {
            if (childDisplaysContainer != null)
            {
                childDisplays.Clear();
                foreach (Transform child in childDisplaysContainer)
                {
                    GameObject.Destroy(child.gameObject);
                }
                childDisplaysContainer.DetachChildren();
            }
        }

        public virtual T InstantiateDisplay<T>() where T : BaseDisplay
        {
            GameObject displayGameObject = Instantiate(childDisplayPrefab, childDisplaysContainer);
            T baseDisplay = displayGameObject.GetComponent<T>();
            childDisplays.Add(baseDisplay);

            return baseDisplay;
        }
    }
}


