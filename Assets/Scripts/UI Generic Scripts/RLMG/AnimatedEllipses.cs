using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimatedEllipses : MonoBehaviour
{
	public Text text;

	private string originalText;

	void Awake()
	{
		if (text == null)
			text = GetComponent<Text>();

		if (text != null)
			originalText = text.text;
	}

	void OnEnable()
	{
		stepNum = 0;

		StartCoroutine(AnimateEllipses());
	}

	void OnDisable()
	{
		StopAllCoroutines();
	}

	private int stepNum = 0;
    public float interval = 0.5f;

	private IEnumerator AnimateEllipses()
	{
		string newText = originalText;

		if (stepNum == 0)
			newText += "";
		else if (stepNum == 1)
			newText += ".";
		else if (stepNum == 2)
			newText += "..";
		else if (stepNum == 3)
			newText += "...";

		if (text != null)
			text.text = newText;

		if (stepNum < 3)
			stepNum++;
		else
			stepNum = 0;

		yield return new WaitForSeconds(interval);

		StartCoroutine(AnimateEllipses());
	}
}
