using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.UI.TableUI
{
    [CustomEditor(typeof(DropdownProperties))]
    public class DropdownProperties_Editor : Cell_Editor<TMP_Dropdown,DropdownProperties>
    {
        static readonly GUIContent leftLabel = new GUIContent("Left");

      

        SerializedProperty optionsprop,dropdownEvent;
        DropdownProperties dp;
        Rect rect;
        private new void OnEnable()
        {
            base.OnEnable();

            if (dp == null)
                dp = target as DropdownProperties;

            if (optionsprop == null){
                try{
                optionsprop = serializedObject.FindProperty("options");
                }catch{;}

            }
            if (dropdownEvent == null)
                try{
                    dropdownEvent = serializedObject.FindProperty("onDropdownChange");
                }catch{;}
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.BeginVertical(boxStyle);
            serializedObject.Update();
            DrawColors();
            
            dp.ItemHeight = EditorGUILayout.FloatField("Item Height", dp.ItemHeight);
            //DrawMainRect();
            DrawMainRect("Main Rect", "L", "R", "T", "B");
            DrawMainTextProperties();
            DrawSecondTextProperties();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(optionsprop);
            if (EditorGUI.EndChangeCheck())
            {
                dp.ApplyOptions();
            }
            if (!cell.isSubset)
                EditorGUILayout.PropertyField(dropdownEvent);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }
    }    
}
