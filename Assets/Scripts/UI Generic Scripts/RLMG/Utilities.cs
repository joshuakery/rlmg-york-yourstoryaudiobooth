using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utilities
{

	public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
	{
		T item = list[index];
		list.RemoveAt(index);
		list.Insert(0, item);
	}

	static public void ShuffleArray<T>(T[] arr)
	{
		for (int i = arr.Length - 1; i > 0; i--)
		{
			int r = Random.Range(0, i + 1);
			T tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
	}

	static public void ShuffleList<T>(List<T> arr)
	{
		for (int i = arr.Count - 1; i > 0; i--)
		{
			int r = Random.Range(0, i + 1);
			T tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
	}

	static public string UppercaseFirst(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		char[] a = s.ToCharArray();
		a[0] = char.ToUpper(a[0]);
		return new string(a);
	}

	static public Sprite TextureToMipMappedSprite(Texture2D texture)
	{
		//there's a little added complexity to generating mipmaps http://answers.unity3d.com/questions/10292/how-do-i-generate-mipmaps-at-runtime.html
		Texture2D mippedTexture = new Texture2D(texture.width, texture.height);
		mippedTexture.SetPixels(texture.GetPixels());
		mippedTexture.Apply(true);

		return Sprite.Create(mippedTexture, new Rect(0, 0, mippedTexture.width, mippedTexture.height), Vector2.zero);
	}

    static public string GetOnlyFileNameFromFullPath(string fullPath)
    {
        string[] pathSplit = fullPath.Split(new char[] { '/', '\\' });
        string fileName = pathSplit.Last();

        return fileName;
    }

    static public float LerpUnclamped(float a, float b, float t)
    {
        return t * b + (1 - t) * a;
    }

    static public Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(LerpUnclamped(a.x, b.x, t), LerpUnclamped(a.y, b.y, t), LerpUnclamped(a.z, b.z, t));
    }

    static public float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f)
            angle = 360 + angle;

        if (angle > 180f)
            return Mathf.Max(angle, 360 + from);

        return Mathf.Min(angle, to);
    }

    //https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/#post-1623103
    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

	static public int Get1DArrayIndexFrom2D(int x, int y, int xCount)
	{
		return (y * xCount) + x;
	}

	static public Vector2 Get2DArrayIndex(int arrayPos, int xCount)
	{
		int x = arrayPos % xCount;
		int y = (arrayPos - x) / xCount;
		return new Vector2(x, y);
	}

	static public int Get1DArrayIndexFrom3D(Vector3 index, int xCount, int yCount)
	{
		return Get1DArrayIndexFrom3D((int)index.x, (int)index.y, (int)index.z, xCount, yCount);
	}

	static public int Get1DArrayIndexFrom3D(int x, int y, int z, int xCount, int yCount)
	{
		return x + y * xCount + z * xCount * yCount;
	}
}
