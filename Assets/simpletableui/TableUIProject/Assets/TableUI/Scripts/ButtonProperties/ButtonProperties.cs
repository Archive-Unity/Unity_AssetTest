using System;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.TableUI
{
    public class ButtonProperties : Cell<Button>
    {



        [SerializeField]
        public OnButtonClick onButtonClick;

        public override void Init()
        {
            base.Init();

            if (onButtonClick == null)
                onButtonClick = new OnButtonClick();

#if UNITY_EDITOR
            backgroundImage = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            RectTransform rt = Resources.Load<GameObject>("Prefabs/Button").transform.GetChild(0).GetComponent<RectTransform>();

            mainRect.x = rt.offsetMin.x;
            mainRect.y = rt.offsetMax.x;
            mainRect.z = rt.offsetMax.y;
            mainRect.w = rt.offsetMin.y;
#endif

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
                        RectTransform rt = ((Button)tu.data[j].list[i]).GetComponent<RectTransform>();
                        rt.offsetMin = new Vector2(mainRect.x, mainRect.w);
                        rt.offsetMax = new Vector2(mainRect.y, mainRect.z);
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
                       ((Button)tu.data[j].list[i]).GetComponent<Image>().sprite=BackgroundImage;                       
                        Utils.SetDirty(tu.data[j].list[i]);
                    }
                    catch (System.Exception) { }
                }
            }
        }

        public void OnButtonClickEvent()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            string name = go.name;
            string[] spName = name.Split(',');
            int row = Int32.Parse(spName[0]);
            int column = Int32.Parse(spName[1]);
            onButtonClick.Invoke(row, column);
        }
    }

    [System.Serializable]
    public class OnButtonClick : UnityEvent<int, int>
    {

    }
}
