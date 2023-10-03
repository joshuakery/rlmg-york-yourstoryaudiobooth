using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.GenericUI
{
    public class RenderTextureFollowSize : MonoBehaviour
    {
        [SerializeField]
        private RectTransform leader;

        private Vector2 lastLeaderSizeDelta;

        [SerializeField]
        private Camera targetCamera;

        [SerializeField]
        private RawImage ri;

        void Start()
        {
            if (leader != null)
            {
                UpdateRenderTexture();
            }

        }

        void Update()
        {
            if (targetCamera != null)
            {
                if ((int)lastLeaderSizeDelta.x != (int)leader.rect.width ||
                    (int)lastLeaderSizeDelta.y != (int)leader.rect.height
                    )
                {
                    UpdateRenderTexture();
                }
            }

        }

        private void UpdateRenderTexture()
        {
            if (targetCamera.targetTexture != null)
                targetCamera.targetTexture.Release();

            Vector2 newSize = new Vector2((int)leader.rect.width, (int)leader.rect.height);

            targetCamera.targetTexture = new RenderTexture((int)newSize.x, (int)newSize.y, 24);

            if (ri != null)
                ri.texture = targetCamera.targetTexture;

            lastLeaderSizeDelta = newSize;
        }


    }
}
