using TMPro.EditorUtilities;
using UnityEditor;

namespace UnityEngine.UI.TableUI
{
    public abstract class Cell_Editor<T,E> : Editor where E : Cell<T> where T : Object
    {
        protected Cell<T> cell;
        SerializedProperty colorsprop, mainrectprop, secondRectProp;
        Editor mainTextPropertiesEditor,secondTextPropertiesEditor;
        protected GUIStyle ls,boxStyle;
        bool colorsFoldout,imagesFoldout,mainTextFoldout,secondTextFoldout;

        protected void OnEnable()
        {
            if (cell == null)
                cell = target as Cell<T>;
            if (cell == null)
                return;
            if (colorsprop == null)
                colorsprop = serializedObject.FindProperty("colors");
            if (mainrectprop == null)
                mainrectprop = serializedObject.FindProperty("mainRect");
            if (secondRectProp == null)
                secondRectProp = serializedObject.FindProperty("secondRect");
            if (mainTextPropertiesEditor == null)
                mainTextPropertiesEditor = CreateEditor(cell.mainTextProperties);
            if (secondTextPropertiesEditor == null)
                secondTextPropertiesEditor = CreateEditor(cell.secondTextProperties);

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle("box");
            }

        }

        protected void DrawImages()
        {
            EditorGUI.indentLevel += 1;
            imagesFoldout = EditorGUILayout.Foldout(imagesFoldout, new GUIContent("Images"), true, TMP_UIStyleManager.boldFoldout);
            if (imagesFoldout)
            {
                DrawBackgroundImage();
                DrawSecondaryImage();
            }
            EditorGUI.indentLevel -= 1;
        }

        protected void DrawBackgroundImage()
        {
            cell.BackgroundImage = (Sprite)EditorGUILayout.ObjectField("Background Image", cell.BackgroundImage, typeof(Sprite), true);
        }

        protected void DrawSecondaryImage()
        {
            cell.SecondaryImage = (Sprite)EditorGUILayout.ObjectField("Check Image", cell.SecondaryImage, typeof(Sprite), true);
        }

        protected void DrawColors()
        {
            EditorGUI.indentLevel += 1;
            colorsFoldout = EditorGUILayout.Foldout(colorsFoldout, new GUIContent("Colors"), true, TMP_UIStyleManager.boldFoldout);
            if (colorsFoldout)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(colorsprop);
                if (EditorGUI.EndChangeCheck())
                {
                    cell.ApplyProperty("Colors", "colors");
                }
            }
            EditorGUI.indentLevel -= 1;
        }

        protected void DrawMainRect()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mainrectprop);

            if (EditorGUI.EndChangeCheck())
            {
                cell.MainRect = mainrectprop.vector4Value;
            }
        }

        protected void DrawMainRect(string mainLabel, params string[] labels)
        {
            float[] result = new float[] { mainrectprop.vector4Value.x, mainrectprop.vector4Value.y, mainrectprop.vector4Value.z, mainrectprop.vector4Value.w };
            if (labels.Length != 4)
            {
                Debug.LogError("There must be 4 labels to draw the rect");
            }

            bool twoLinesLayout = Screen.width < 375f;

            EditorGUILayout.BeginVertical(GUILayout.Height(twoLinesLayout ? 40 : 20));

            Rect rect = EditorGUILayout.GetControlRect();
            Rect mainLabelRect = rect;
            mainLabelRect.width = 80;
            /*if (twoLinesLayout)
                mainLabelRect.height += 0.5f;*/

            Rect fieldRect = rect;

            if (twoLinesLayout)
            {
                //fieldRect.height *= 0.5f;
                fieldRect.y += fieldRect.height;
                fieldRect.width = rect.width / 4;
            }
            else
            {
                fieldRect.x += mainLabelRect.width;
                fieldRect.width = (rect.width - mainLabelRect.width) / 4;
            }

            EditorGUI.LabelField(mainLabelRect, mainLabel);
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < 4; i++)
            {
                GUIContent label = new GUIContent(labels[i]);
                //Vector2 labelSize = EditorStyles.label.CalcSize(label);

                EditorGUIUtility.labelWidth = 15; //Mathf.Max(labelSize.x + 5, 0.3f * fieldRect.width);
                result[i] = EditorGUI.FloatField(fieldRect, label, result[i]);
                fieldRect.x += fieldRect.width;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Vector4 v = cell.SecondRect;
                v.x = result[0];
                v.y = result[1];
                v.z = result[2];
                v.w = result[3];
                cell.MainRect = v;
            }
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();
        }

        protected void DrawSecondRect()
        {

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(secondRectProp);

            if (EditorGUI.EndChangeCheck())
            {
                cell.SecondRect = secondRectProp.vector4Value;

            }
        }

        protected void DrawSecondRect(string mainLabel,params string[] labels)
        {
            float[] result = new float[] { secondRectProp.vector4Value.x, secondRectProp.vector4Value.y, secondRectProp.vector4Value.z, secondRectProp.vector4Value.w };
            if (labels.Length != 4)
            {
                Debug.LogError("There must be 4 labels to draw the rect");
            }

            bool twoLinesLayout = Screen.width < 375f;

            EditorGUILayout.BeginVertical(GUILayout.Height(twoLinesLayout?40:20));

            Rect rect = EditorGUILayout.GetControlRect();
            Rect mainLabelRect = rect;
            mainLabelRect.width = 80;
            /*if (twoLinesLayout)
                mainLabelRect.height += 0.5f;*/

            Rect fieldRect = rect;

            if (twoLinesLayout)
            {
                //fieldRect.height *= 0.5f;
                fieldRect.y += fieldRect.height;
                fieldRect.width = rect.width / 4;
            }
            else
            {
                fieldRect.x += mainLabelRect.width;
                fieldRect.width = (rect.width - mainLabelRect.width) / 4;
            }

            EditorGUI.LabelField(mainLabelRect, mainLabel);
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < 4; i++)
            {
                GUIContent label = new GUIContent(labels[i]);
                //Vector2 labelSize = EditorStyles.label.CalcSize(label);

                EditorGUIUtility.labelWidth = 15; //Mathf.Max(labelSize.x + 5, 0.3f * fieldRect.width);
                result[i] = EditorGUI.FloatField(fieldRect, label, result[i]);
                fieldRect.x += fieldRect.width;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Vector4 v = cell.SecondRect;
                v.x = result[0];
                v.y = result[1];
                v.z = result[2];
                v.w = result[3];
                cell.SecondRect = v;
            }
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();
        }

        protected void DrawMainTextProperties()
        {
            EditorGUI.indentLevel += 1;
            mainTextFoldout = EditorGUILayout.Foldout(mainTextFoldout, new GUIContent("Main Text Properties"), true, TMP_UIStyleManager.boldFoldout);
            if(mainTextFoldout)
                mainTextPropertiesEditor.OnInspectorGUI();
            EditorGUI.indentLevel -= 1;
        }

        protected void DrawSecondTextProperties()
        {
            EditorGUI.indentLevel += 1;
            secondTextFoldout = EditorGUILayout.Foldout(secondTextFoldout, new GUIContent("Second Text Properties"), true, TMP_UIStyleManager.boldFoldout);
            if(secondTextFoldout)
                secondTextPropertiesEditor.OnInspectorGUI();
            EditorGUI.indentLevel -= 1;
        }

        protected void DrawLabelField(string title)
        {
            if (ls == null)
            {
                ls = new GUIStyle
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 15
                };
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, ls);
        }
    }
}
