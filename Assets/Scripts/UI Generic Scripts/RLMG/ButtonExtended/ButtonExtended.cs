using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonExtended : Button
{
	public TransitionGraphic[] graphics;

	[System.Serializable]
	public class TransitionGraphic
	{
		public Graphic graphic;

		//public ColorBlock colorBlock;  //not sure I wanted to expose the colorMultiplier or fadeDuration
		public Color normalColor;
		public Color highlightedColor;
		public Color pressedColor;
		public Color disabledColor;
	}

	public AudioSource soundOnDown;
	public AudioSource soundOnUp;

	protected override void Awake()
	{
		base.Awake();

		onClick.AddListener(() => Clicked());
	}

	private void Clicked()
	{
		if (soundOnUp != null)
			soundOnUp.Play();
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		if (base.gameObject.activeInHierarchy && this.transition == Selectable.Transition.ColorTint)
		{
			ColorTween(state);
		}

		if (state == Selectable.SelectionState.Pressed && soundOnDown != null)
			soundOnDown.Play();

		base.DoStateTransition(state, instant);
	}

	private void ColorTween(SelectionState state)
	{
		if (graphics == null)
			return;

		foreach (TransitionGraphic tg in graphics)
		{
			if (tg != null && tg.graphic != null)
			{
				Color color;
				switch (state)
				{
					case Selectable.SelectionState.Normal:
						color = tg.normalColor;
						break;
					case Selectable.SelectionState.Highlighted:
						color = tg.highlightedColor;
						break;
					case Selectable.SelectionState.Pressed:
						color = tg.pressedColor;
						break;
					case Selectable.SelectionState.Disabled:
						color = tg.disabledColor;
						break;
					default:
						color = Color.black;
						break;
				}

				tg.graphic.CrossFadeColor(color, this.colors.fadeDuration, true, true);
			}
		}
	}
}