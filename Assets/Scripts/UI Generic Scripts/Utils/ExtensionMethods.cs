using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static void ResetScale(this Transform trans)
    {
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static Transform[] GetChildrenWithTag(this Transform parent, string tag)
    {
        if (parent == null) { throw new System.ArgumentNullException();  }
        if (System.String.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

        List<Transform> tagged = new List<Transform>();
        for (int i=0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag) { tagged.Add(child);  }
        }

        return tagged.ToArray();
    }

    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
        if (list.Count == 0) { return null; }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].CompareTag(tag) == false)
            {
                list.RemoveAt(i);
            }
        }
        return list.ToArray();
    }

    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject parent, string tag, int depth, bool forceActive = false) where T : Component
    {
        //depth of 0 gets all children

        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
        if (list.Count == 0) { return null; }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].CompareTag(tag) == false)
            {
                list.RemoveAt(i);
            }
        }
        return list.ToArray();
    }

    public static void SizeToWidth(this RectTransform rt, Texture2D tex)
    {
        if (tex != null)
        {
            float nw = rt.rect.width;
            float nh = nw * ((float)tex.height / (float)tex.width);
            rt.sizeDelta = new Vector2(nw, nh);
        }
    }

    public static IEnumerator SizeToWidthCo(this RectTransform rt, Texture2D tex)
    {
        yield return null;
        SizeToWidth(rt, tex);
    }

    public static void SizeToWidth(this LayoutElement le, Texture2D tex)
    {
        if (tex != null)
        {
            float nw = le.preferredWidth;
            float nh = nw * ((float)tex.height / (float)tex.width);
            le.preferredHeight = nh;

            le.minHeight = nh;
            le.minWidth = nw;
        }
    }

    public static IEnumerator SizeToWidthCo(this LayoutElement le, Texture2D tex)
    {
        yield return null;
        SizeToWidth(le, tex);
    }

    public static void ResetAllAnimatorTriggers(this Animator animator)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="exceptions">Trigger Names</param>
    public static void ResetAllAnimatorTriggers(this Animator animator, List<string> exceptions)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger && !exceptions.Contains(trigger.name))
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

}
