using UnityEngine;
using System.Collections;

public class DisableInBuild : MonoBehaviour
{
#if !UNITY_EDITOR
	void Start ()
	{
		this.gameObject.SetActive(false);
	}
#endif
}
