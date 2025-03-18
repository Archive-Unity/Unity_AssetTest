using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.TableUI
{
    public class ToggleProperties : Cell<Toggle>
    {

        [SerializeField]
        public OnToggleValueChange onToggleValueChange;

        public override void Init()
        {
            base.Init();

            if (onToggleValueChange == null)
                onToggleValueChange = new OnToggleValueChange();
#if UNITY_EDITOR

            backgroundImage = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            secondaryImage = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
            RectTransform rt = Resources.Load<GameObject>("Prefabs/Toggle").transform.GetChild(0).GetComponent<RectTransform>();
            mainRect.x = rt.localPosition.x;
            mainRect.y = rt.localPosition.y;
            mainRect.z = rt.sizeDelta.x;
            mainRect.w = rt.sizeDelta.y;

            RectTransform rt2 = rt.transform.GetChild(0).GetComponent<RectTransform>();
            secondRect.x = rt2.localPosition.x;
            secondRect.y = rt2.localPosition.y;
            secondRect.z = rt2.sizeDelta.x;
            secondRect.w = rt2.sizeDelta.y;
#endif

        }

        public void OnToggleValueChangeEvent()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            string name = go.name;
            string[] spName = name.Split(',');
            int row = Int32.Parse(spName[0]);
            int column = Int32.Parse(spName[1]);
            onToggleValueChange.Invoke(row, column, go.GetComponent<Toggle>().isOn);
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
                        RectTransform rt = ((Toggle)tu.data[j].list[i]).transform.GetChild(0).GetComponent<RectTransform>();
                        Vector3 pos = rt.localPosition;
                        pos.x = MainRect.x;
                        pos.y = MainRect.y;
                        rt.localPosition = pos;
                        Vector2 sizeDelta = rt.sizeDelta;
                        sizeDelta.x = MainRect.z;
                        sizeDelta.y = MainRect.w;
                        rt.sizeDelta = sizeDelta;
                        Utils.SetDirty(rt);
                    }
                    catch (System.Exception) { }
                }
            }
        }

        protected override void UpdateSecondRect()
        {
            TableUI tu = GetComponent<TableUI>();


            for (int i = Min.x; i < Max.x; i++)
            {
                for (int j = Min.y; j < Max.y; j++)
                {
                    try
                    {
                        RectTransform rt = ((Toggle)tu.data[j].list[i]).transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
                        Vector3 pos = rt.localPosition;
                        pos.x = SecondRect.x;
                        pos.y = SecondRect.y;
                        rt.localPosition = pos;
                        Vector2 sizeDelta = rt.sizeDelta;
                        sizeDelta.x = SecondRect.z;
                        sizeDelta.y = SecondRect.w;
                        rt.sizeDelta = sizeDelta;
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
                        ((Toggle)tu.data[j].list[i]).transform.GetChild(0).GetComponent<Image>().sprite = BackgroundImage;
                        Utils.SetDirty(tu.data[j].list[i]);
                    }
                    catch (System.Exception) { }
                }
            }
        }

        protected override void UpdateSecondaryImage()
        {
            TableUI tu = GetComponent<TableUI>();


            for (int i = Min.x; i < Max.x; i++)
            {
                for (int j = Min.y; j < Max.y; j++)
                {
                    try
                    {
                        ((Toggle)tu.data[j].list[i]).transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = SecondaryImage;
                        Utils.SetDirty(tu.data[j].list[i]);
                    }
                    catch (System.Exception) { }
                }
            }
        }
    }

    [System.Serializable]
    public class OnToggleValueChange : UnityEvent<int, int, bool>
    {

    }
}
