/*
	This class was created just to make it easier to attach sliders to float or int 
	variables and adjust them at runtime, as touch controls only work in a build.
	-Jon Yuhas
 */

using UnityEngine;
using UnityEngine.UI;

public class VariableTestSlider : MonoBehaviour
{
	public Slider slider;
	public Text labelText;
	public string labelName;
	public Text valueText;

	public MonoBehaviour variableClass;
	public string variableName;

	public enum VarType { Float, Int };
	public VarType varType = VarType.Float;

	public float rangeMin;
	public float rangeMax;

	public bool updateConstantly = false;

	void Start()
	{
		if (slider != null)
		{
			//var range = typeof(MoonshotDataHandler).GetField(nameof(MoonshotDataHandler.rangeAttributeTest)).GetCustomAttribute<RangeAttribute>();
			//var range = variableClass.GetType().GetField(variableName).GetCustomAttribute<RangeAttribute>();
			//var range = variableClass.GetType().GetField(variableName).GetCustomAttribute(typeof(RangeAttribute), false);

			// if (range != null)
			// {
			// 	slider.minValue = range.min;
			// 	slider.maxValue = range.max;
			// }
			// else
			// {
				slider.minValue = rangeMin;
				slider.maxValue = rangeMax;
			// }
			
			
			
			

			if (variableClass != null)
			{
				if (varType == VarType.Float)
					slider.value = (float)variableClass.GetType().GetField(variableName).GetValue(variableClass);
				else if (varType == VarType.Int)
					slider.value = (int)variableClass.GetType().GetField(variableName).GetValue(variableClass);
			}



			



			if (varType == VarType.Int)
				slider.wholeNumbers = true;

			slider.onValueChanged.AddListener(delegate { ValueChange(); });

			if (valueText != null)
				valueText.text = varType == VarType.Float ? slider.value.ToString("F3") : slider.value.ToString("F0");
		}

		if (labelText != null && !string.IsNullOrEmpty(labelName))
			labelText.text = labelName;
	}

	void Update()
	{
		if (updateConstantly)
		{
			if (slider != null && variableClass != null)
			{
				if (varType == VarType.Float)
					slider.value = (float)variableClass.GetType().GetField(variableName).GetValue(variableClass);
				else if (varType == VarType.Int)
					slider.value = (int)variableClass.GetType().GetField(variableName).GetValue(variableClass);

				if (valueText != null)
					valueText.text = varType == VarType.Float ? slider.value.ToString("F3") : slider.value.ToString("F0");
			}
		}
	}

	public void ValueChange()
	{
		if (slider != null && variableClass != null)
		{
			if (varType == VarType.Float)
				variableClass.GetType().GetField(variableName).SetValue(variableClass, slider.value);
			else if (varType == VarType.Int)
				variableClass.GetType().GetField(variableName).SetValue(variableClass, (int)slider.value);

			if (valueText != null)
				valueText.text = varType == VarType.Float ? slider.value.ToString("F3") : slider.value.ToString("F0");
		}
	}
}
