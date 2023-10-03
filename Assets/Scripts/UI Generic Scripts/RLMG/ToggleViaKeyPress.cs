using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleViaKeyPress : MonoBehaviour
{
	public KeyCode key;

	public GameObject targetObj;
	public MonoBehaviour targetComponent;

	void Update()
	{
		if (Input.GetKeyDown(key))
		{
			if (targetObj != null)
				targetObj.SetActive(!targetObj.activeSelf);

			if (targetComponent != null)
				targetComponent.enabled = !targetComponent.enabled;
		}
	}
}
