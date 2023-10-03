using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ButtonExtended))]
public class ButtonExtendedEditor : UnityEditor.UI.ButtonEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var serializedObject = new SerializedObject(target);
		serializedObject.Update();
		SerializedProperty sp = serializedObject.FindProperty("graphics");
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(sp, true);
		if (EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();


		//LevelScript myTarget = (LevelScript)target;
//		myTarget.experience = EditorGUILayout.ObjectField("Experience", myTarget.experience);
//		EditorGUILayout.LabelField("Level", myTarget.Level.ToString());

		ButtonExtended myTarget = (ButtonExtended)target;
		myTarget.soundOnDown = (AudioSource)EditorGUILayout.ObjectField("Sound On Down", myTarget.soundOnDown, typeof(AudioSource), true);
		myTarget.soundOnUp = (AudioSource)EditorGUILayout.ObjectField("Sound On Up", myTarget.soundOnUp, typeof(AudioSource), true);
	}
}
