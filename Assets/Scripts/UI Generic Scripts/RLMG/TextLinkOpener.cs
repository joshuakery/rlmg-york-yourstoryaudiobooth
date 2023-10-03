/*
 * Inspired by the following two links, but with some custom stuff:
 * http://www.feelouttheform.net/unity3d-links-textmeshpro/
 * https://deltadreamgames.com/unity-tmp-hyperlinks/
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class LinkOpenedEvent : UnityEvent<string>
{
}

[RequireComponent(typeof(TMP_Text))]
public class TextLinkOpener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Camera eventCamera;  //If you are not in a Canvas using Screen Overlay, put your camera instead of null

    //public Color defaultColor = Color.magenta;
    private List<Color32[]> defaultColors;

    public Color downColor = Color.red;

    private int currentDownLinkIndex = -1;

    public LinkOpenedEvent OnLinkOpened = new LinkOpenedEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, eventCamera);  

        if (linkIndex != -1) // was a link clicked?
        {
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            //Application.OpenURL(linkInfo.GetLinkID());

            Debug.Log("clicked link with id: " + linkInfo.GetLinkID());

            OnLinkOpened.Invoke(linkInfo.GetLinkID());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, eventCamera);

        if (linkIndex != -1) // was a link clicked?
        {
            //SetLinkToColor(linkIndex, downColor);
            defaultColors = SetLinkToColor(linkIndex, downColor);

            currentDownLinkIndex = linkIndex;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        //int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, eventCamera);

        //if (linkIndex != -1) // was a link clicked?
        //{
        //    //SetLinkToColor(linkIndex, defaultColor);
        //    ResetLinkColor(linkIndex, defaultColors);
        //}

        if (currentDownLinkIndex != -1) // was a link clicked?
        {
            ResetLinkColor(currentDownLinkIndex, defaultColors);
        }
    }

    //public void SetLinkToColor(string linkID, Color32 color)
    //{
    //    TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

    //    for (int i = 0; i < pTextMeshPro.textInfo.linkInfo.Length; i++)
    //    {
    //        if (pTextMeshPro.textInfo.linkInfo[i].GetLinkID() == linkID)
    //        {
    //            SetLinkToColor(i, color);

    //            return;
    //        }
    //    }
    //}

    public List<Color32[]> SetLinkToColor(int linkIndex, Color32 color)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

        List<Color32[]> oldVertColors = new List<Color32[]>(); // store the old character colors

        for (int i = 0; i < linkInfo.linkTextLength; i++) // for each character in the link string
        {
            int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
            var charInfo = pTextMeshPro.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
            int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

            Color32[] vertexColors = pTextMeshPro.textInfo.meshInfo[meshIndex].colors32; // the colors for this character

            //oldVertColors.Add(vertexColors.ToArray());
            oldVertColors.Add((Color32[])vertexColors.Clone());

            if (charInfo.isVisible)
            {
                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }
        }

        // Update Geometry
        pTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        return oldVertColors;
    }

    private void ResetLinkColor(int linkIndex, List<Color32[]> colors)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

        for (int i = 0; i < linkInfo.linkTextLength; i++) // for each character in the link string
        { 
            int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
            var charInfo = pTextMeshPro.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.

            pTextMeshPro.textInfo.meshInfo[meshIndex].colors32 = colors[meshIndex];
        }

        // Update Geometry
        pTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}