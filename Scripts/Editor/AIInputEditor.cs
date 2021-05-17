/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AiInput))]
[CanEditMultipleObjects]
public class AIInputEditor : Editor
{
    SerializedProperty actionNameProperty;
    SerializedProperty targetProperty;
    //SerializedProperty typeProperty;
    //SerializedProperty minPressTimeProperty;
    //SerializedProperty maxPressTimeProperty;

    //float minPressTime = 0;
    //float maxPressTime = 1;
    //const float PRESS_TIME_MIN = 0;
    //const float PRESS_TIME_MAX = 10;

    private void OnEnable()
    {
        actionNameProperty = serializedObject.FindProperty("actionName");
        targetProperty = serializedObject.FindProperty("target");
        //typeProperty = serializedObject.FindProperty("type");
        //minPressTimeProperty = serializedObject.FindProperty("minPressDurationSeconds");
        //maxPressTimeProperty = serializedObject.FindProperty("maxPressDurationSeconds");
    }

    //void ButtonInputLayout()
    //{
    //    EditorGUILayout.LabelField("Press time (min, max)");
    //    EditorGUILayout.BeginHorizontal();
    //    minPressTime = EditorGUILayout.FloatField(minPressTime);
    //    maxPressTime = EditorGUILayout.FloatField(maxPressTime);
    //    EditorGUILayout.EndHorizontal();
    //    EditorGUILayout.MinMaxSlider(ref minPressTime, ref maxPressTime, PRESS_TIME_MIN, PRESS_TIME_MAX);

    //    minPressTimeProperty.floatValue = minPressTime;
    //    maxPressTimeProperty.floatValue = maxPressTime;
    //}

    //void ValueInputLayout()
    //{
        
    //}

    //void Axis2DInputLayout()
    //{

    //}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(actionNameProperty);
        EditorGUILayout.LabelField("Will send message: On" + actionNameProperty.stringValue);

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(targetProperty);
        if (targetProperty.objectReferenceValue == null)
        {
            EditorGUILayout.LabelField("Will send message to self");
        }
        //EditorGUILayout.Separator();

        //EditorGUILayout.PropertyField(typeProperty);

        //EditorGUILayout.Separator();

        //if (typeProperty.enumValueIndex == (int)AIInput.InputType.Button)
        //{
        //    ButtonInputLayout();
        //}
        //else if (typeProperty.enumValueIndex == (int)AIInput.InputType.Value)
        //{
        //    ValueInputLayout();
        //}
        //else if (typeProperty.enumValueIndex == (int)AIInput.InputType.Vector2)
        //{
        //    Axis2DInputLayout();
        //}

        

        serializedObject.ApplyModifiedProperties();
    }
}
