
using UnityEditor;


namespace UnityEngine.UI.TableUI
{
    [CustomEditor(typeof(ToggleProperties))]
    public class ToggleProperties_Editor : Cell_Editor<Toggle,ToggleProperties>
    {

        SerializedProperty toggleEvent;

        private new void OnEnable()
        {
            base.OnEnable();
            if (toggleEvent == null)
            try{
                toggleEvent = serializedObject.FindProperty("onToggleValueChange");
            }catch{
                ;
            }
            

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            serializedObject.Update();
            DrawImages();
            DrawColors();
            //DrawMainRect();
            //DrawSecondRect();
            DrawMainRect("Main Rect", "X", "Y", "W", "H");
            DrawSecondRect("Second Rect","X","Y","W","H");
            if (!cell.isSubset)
            {
                EditorGUILayout.PropertyField(toggleEvent);
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

    }
}
