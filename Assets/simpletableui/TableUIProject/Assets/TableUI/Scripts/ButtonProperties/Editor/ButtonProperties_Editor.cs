using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.UI.TableUI
{
    [CustomEditor(typeof(ButtonProperties))]
    public class ButtonProperties_Editor : Cell_Editor<Button,ButtonProperties>
    {

        ButtonProperties bp;

        SerializedProperty buttonEvent;
        Editor buttonTextPropertiesEditor;
        Rect rect;
        private new void OnEnable()
        {
            base.OnEnable();
            if (bp == null)
                bp = target as ButtonProperties;
            if (bp == null)
                return;

            if (buttonEvent == null)
                buttonEvent = serializedObject.FindProperty("onButtonClick");

            if (buttonTextPropertiesEditor == null)
                buttonTextPropertiesEditor = CreateEditor(bp.mainTextProperties);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            serializedObject.Update();
            DrawBackgroundImage();
            DrawColors();
            //DrawMainRect();
            DrawMainRect("Main Rect", "L", "R", "T", "B");
            DrawMainTextProperties();
            if(!cell.isSubset)
                EditorGUILayout.PropertyField(buttonEvent);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }
    }
}