using TMPro;
using UnityEditor;

namespace UnityEngine.UI.TableUI
{
    [CustomEditor(typeof(InputProperties))]
    public class InputProperties_Editor : Cell_Editor<TMP_InputField, InputProperties>
    {
        SerializedProperty inputEvent;
        InputProperties ip;

        private new void OnEnable()
        {
            base.OnEnable();
            if (ip == null)
                ip = target as InputProperties;
            if (ip == null)
                return;

            if (inputEvent == null)
                try {
                    inputEvent = serializedObject.FindProperty("onInputValueChange");
                } catch {
                    ;
                }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            serializedObject.Update();
            DrawBackgroundImage();
            DrawColors();
            DrawMainRect("Main Rect", "L", "R", "T", "B");
            DrawMainTextProperties();
            if (!cell.isSubset)
                EditorGUILayout.PropertyField(inputEvent);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }
    }
}