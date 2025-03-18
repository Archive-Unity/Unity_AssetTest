using System;
using UnityEditor;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.TableUI
{
    public class InputProperties : Cell<TMP_InputField>
    {
        [SerializeField]
        public OnInputValueChange onInputValueChange;

        public override void Init()
        {
            base.Init();

            if (onInputValueChange == null)
                onInputValueChange = new OnInputValueChange();

#if UNITY_EDITOR
            backgroundImage = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
            
            // Initialize the rectangles with default values similar to the Input Field prefab
            mainRect.x = 0;
            mainRect.y = 0;
            mainRect.z = 0;
            mainRect.w = 0;
            
            secondRect.x = 10;
            secondRect.y = 0;
            secondRect.z = -20;
            secondRect.w = 0;
#endif
        }

        public void OnInputValueChangeEvent(string value)
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            string name = go.name;
            string[] spName = name.Split(',');
            int row = Int32.Parse(spName[0]);
            int column = Int32.Parse(spName[1]);
            onInputValueChange.Invoke(row, column, value);
        }

        protected override void UpdateMainRect()
        {
            TableUI tu = GetComponent<TableUI>();

            for (int i = Min.x; i < Max.x; i++)
            {
                for (int j = Min.y; j < Max.y; j++)
                {
                    try
                    {
                        RectTransform rt = ((TMP_InputField)tu.data[j].list[i]).GetComponent<RectTransform>();
                        rt.offsetMin = new Vector2(MainRect.x, MainRect.w);
                        rt.offsetMax = new Vector2(MainRect.y, MainRect.z);
                        Utils.SetDirty(rt);
                    }
                    catch (System.Exception) { }
                }
            }
        }

        protected override void UpdateBackgroundImage()
        {
            TableUI tu = GetComponent<TableUI>();

            for (int i = Min.x; i < Max.x; i++)
            {
                for (int j = Min.y; j < Max.y; j++)
                {
                    try
                    {
                        ((TMP_InputField)tu.data[j].list[i]).GetComponent<Image>().sprite = BackgroundImage;
                        Utils.SetDirty(tu.data[j].list[i]);
                    }
                    catch (System.Exception) { }
                }
            }
        }
    }

    [System.Serializable]
    public class OnInputValueChange : UnityEvent<int, int, string>
    {
    }
}
