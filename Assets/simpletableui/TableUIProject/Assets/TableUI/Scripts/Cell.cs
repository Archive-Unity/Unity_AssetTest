
using System;
using System.Reflection;
using UnityEditor;

namespace UnityEngine.UI.TableUI
{
    [System.Serializable]
    public enum GroupSelectionMethod { Header, Body, All, MinMax }

    public abstract class Cell<T>: MonoBehaviour where T : Object 
    {
        protected static string[] secondRectLabels = { "a", "b", "c", "d" };

        public bool isSubset;

        [SerializeField, HideInInspector]
        public TextProperties mainTextProperties;

        [SerializeField, HideInInspector]
        public TextProperties secondTextProperties;

        [SerializeField]
        public GroupSelectionMethod groupSelectionMethod = GroupSelectionMethod.All;

        [SerializeField,HideInInspector]
        private Vector2Int min = Vector2Int.zero;

        public Vector2Int Min
        {
            get { return min; }
            set
            {
                if (min == value)
                    return;
                min = value;
                if (mainTextProperties != null)
                    mainTextProperties.min = value;
                if (secondTextProperties != null)
                    secondTextProperties.min = value;
            }
        }

        [SerializeField,HideInInspector]
        private Vector2Int max = Vector2Int.zero;

        public Vector2Int Max
        {
            get { return max; }
            set
            {
                if (max == value)
                    return;
                max = value;
                if (mainTextProperties != null)
                    mainTextProperties.max = value;
                if (secondTextProperties != null)
                    secondTextProperties.max = value;
            }
        }

        [SerializeField, HideInInspector]
        protected ColorBlock colors;

        public ColorBlock Colors
        {
            get { return colors; }
            set
            {
                if (colors == value)
                    return;
                colors = value;
                ApplyProperty("Colors", "colors");
            }
        }

        [SerializeField, HideInInspector]
        protected Sprite backgroundImage;
        public Sprite BackgroundImage
        {
            get { return backgroundImage; }
            set { if (backgroundImage == value) return; backgroundImage = value; UpdateBackgroundImage(); }
        }

        [SerializeField, HideInInspector]
        protected Sprite secondaryImage;
        public Sprite SecondaryImage
        {
            get { return secondaryImage; }
            set { if (secondaryImage == value) return; secondaryImage = value; UpdateSecondaryImage(); }
        }

        [SerializeField, HideInInspector]
        protected Vector4 mainRect;
        public Vector4 MainRect
        {
            get { return mainRect; }
            set { if (!mainRect.Equals(value))mainRect = value; UpdateMainRect(); }
        }

        [SerializeField, HideInInspector]
        protected Vector4 secondRect;
        public Vector4 SecondRect
        {
            get { return secondRect; }
            set { if (!secondRect.Equals(value)) secondRect = value; UpdateSecondRect(); }
        }

        public virtual void Init()
        {
            Color col;
            ColorUtility.TryParseHtmlString("#FFFFFF", out col);
            colors.normalColor = col;
            ColorUtility.TryParseHtmlString("#F5F5F5", out col);
            colors.highlightedColor = col;
            ColorUtility.TryParseHtmlString("#C8C8C8", out col);
            colors.pressedColor = col;
            ColorUtility.TryParseHtmlString("#F5F5F5", out col);
            colors.selectedColor = col;
            ColorUtility.TryParseHtmlString("#C8C8C8", out col);
            col.a = 0.5f;
            colors.disabledColor = col;
            colors.colorMultiplier = 1f;
        }

        protected void UpdateImage(Image img,Sprite sprite)
        {
            TableUI tu = GetComponent<TableUI>();


            for (int i = 0; i < tu.Rows; i++)
            {
                for (int j = 0; j < tu.Columns; j++)
                {
                    try
                    {
                        ((Toggle)tu.data[i].list[j]).transform.GetChild(0).GetComponent<Image>().sprite = BackgroundImage;
                        Utils.SetDirty(tu.data[i].list[j]);
                    }
                    catch (System.Exception) { }
                }
            }
        }

        protected virtual void UpdateBackgroundImage() { }

        protected virtual void UpdateSecondaryImage() { }

        protected virtual void UpdateMainRect() { }

        protected virtual void UpdateSecondRect() { }

        public void ApplyProperty(string oriProp, string targetProp)
        {
            TableUI tu = GetComponent<TableUI>();
            UpdateMinMaxValues(tu);
            Type t = GetType();
            PropertyInfo prop = t.GetProperty(oriProp);

            for (int i = min.x; i < max.x; i++)
            {
                for (int j = min.y; j < max.y; j++)
                {
                    try
                    {
                        T obj = (T)tu.data[j].list[i];
                        typeof(Toggle).GetProperty(targetProp).SetValue(obj, prop.GetValue(this));

                        Utils.SetDirty(obj);
                    }
                    catch (System.Exception) { }

                }
            }
        }

        private void UpdateMinMaxValues(TableUI tu)
        {
            if (groupSelectionMethod.Equals(GroupSelectionMethod.MinMax))
                return;

            if (groupSelectionMethod.Equals(GroupSelectionMethod.Header))
            {
                min.x = 0;
                min.y = 0;
                max.x = tu.Header ? tu.Columns : 0;
                max.y = tu.Header ? 1 : 0;
            }
            else if (groupSelectionMethod.Equals(GroupSelectionMethod.Body))
            {
                min.x = 0;
                min.y = tu.Header ? 1 : 0;
                max.x = tu.Columns;
                max.y = tu.Rows;
            }
            else if (groupSelectionMethod.Equals(GroupSelectionMethod.All))
            {
                min.x = 0;
                min.y = 0;
                max.x = tu.Columns;
                max.y = tu.Rows;
            }
        }
    }
}