using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static TMPro.TMP_Dropdown;

namespace UnityEngine.UI.TableUI
{
    public class DropdownProperties : Cell<TMP_Dropdown>
    {
        [SerializeField, HideInInspector]
        private float itemHeight=20f;
        public float ItemHeight
        {
            get { return itemHeight; }
            set { if (itemHeight == value) return; itemHeight = value; UpdateItemHeight(); }
        }

        [SerializeField, HideInInspector]
        public OptionDataList options;


        public OnDropdownChange onDropdownChange;

        public void ApplyOptions()
        {
            TableUI tu = GetComponent<TableUI>();
            for (int i = Min.x; i < Max.x; i++)
            {
                for (int j = Min.y; j < Max.y; j++)
                {
                    try
                    {
                        TMP_Dropdown drop = (TMP_Dropdown)tu.data[j].list[i];
                        drop.options = options.options;
                        drop.RefreshShownValue();
                        Utils.SetDirty(drop);
                    }
                    catch (System.Exception) { }

                }
            }
        }

        public override void Init()
        {

            base.Init();

            if (onDropdownChange == null)
                onDropdownChange = new OnDropdownChange();
        }

        void UpdateItemHeight()
        {
            TableUI tu = GetComponent<TableUI>();
            for (int i = Min.x; i < Max.x; i++)
            {
                for (int j = Min.y; j < Max.y; j++)
                {
                    try
                    {
                        
                        RectTransform item = ((TMP_Dropdown)tu.data[j].list[i]).transform.Find("Template").Find("Viewport").Find("Content").Find("Item").GetComponent<RectTransform>(); ;
                        Vector2 sizeDelta = item.sizeDelta;
                        sizeDelta.y = ItemHeight;
                        item.sizeDelta = sizeDelta;

                        RectTransform content = ((TMP_Dropdown)tu.data[j].list[i]).transform.Find("Template").Find("Viewport").Find("Content").GetComponent<RectTransform>();
                        sizeDelta = content.sizeDelta;
                        sizeDelta.y = ItemHeight + 8f;
                        content.sizeDelta = sizeDelta;
                        Utils.SetDirty(item);
                    }
                    catch (System.Exception) { }
                }
            }


            
        }

        public void OnDropdownChangeEvent()
        {
            Transform transf = EventSystem.current.currentSelectedGameObject.transform;
            while (!transf.parent.name.Equals("Dropdown"))
            {
                
                transf = transf.parent;
            }
            string name = transf.name;
            string[] spName = name.Split(',');
            int row = Int32.Parse(spName[0]);
            int column = Int32.Parse(spName[1]);
            onDropdownChange.Invoke(row, column,transf.GetComponent<TMP_Dropdown>().value);
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
                        RectTransform rt = ((TMP_Dropdown)tu.data[j].list[i]).GetComponent<RectTransform>();
                        rt.offsetMin = new Vector2(MainRect.x, MainRect.w);
                        rt.offsetMax = new Vector2(MainRect.y, MainRect.z);
                        Utils.SetDirty(rt);
                    }
                    catch (System.Exception) { }
                }
            }
        }
    }

    

    [System.Serializable]
    public class OnDropdownChange : UnityEvent<int, int,int>//row,column,value
    {

    }
}
