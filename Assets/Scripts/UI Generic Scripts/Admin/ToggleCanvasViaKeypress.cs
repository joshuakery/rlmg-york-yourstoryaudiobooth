using UnityEngine;

namespace JoshKery.GenericUI.Admin
{
	public class ToggleCanvasViaKeypress : MonoBehaviour
	{
		public KeyCode key;

		public Canvas canvas;

		public bool enabledOnStart;

		private void Start()
		{
			if (canvas != null)
				canvas.enabled = enabledOnStart;
		}

		void Update()
		{
			if (Input.GetKeyDown(key))
			{
				if (canvas != null)
					canvas.enabled = !canvas.enabled;
			}
		}
	}
}

