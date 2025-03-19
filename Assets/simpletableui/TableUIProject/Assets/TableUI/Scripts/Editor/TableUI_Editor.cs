
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
using TMPro.EditorUtilities;
using static TMPro.TMP_Dropdown;
using System;

namespace UnityEngine.UI.TableUI
{
    [CustomEditor(typeof(TableUI))]
    public class TableUI_Editor : Editor
    {
        int toolbarValue = 0;
        TableUI tableUI;
        Vector2 gridScroll;
        bool headerFoldout,bodyFoldout;

        Editor bodyCellPropertiesEditor,headerCellPropertiesEditor,selectionCellPropertiesEditor
            ,togglePropertiesEditor, buttonPropertiesEditor,dropdownPropertiesEditor,
            inputPropertiesEditor, togglePropertiesSubsetEditor,buttonPropertiesSubsetEditor,
            dropdownPropertiesSubsetEditor, inputPropertiesSubsetEditor;

        Vector2 previousSize;

        GUIStyle labelStyle, labelStyle2;

        private void OnEnable()
        {
            if(tableUI==null)
             tableUI = target as TableUI;

            Undo.undoRedoPerformed += tableUI.OnUndoRedoEvent;
            
            if(bodyCellPropertiesEditor==null)
                bodyCellPropertiesEditor = CreateEditor(tableUI.bodyCellProperties);

            if(headerCellPropertiesEditor==null)
                headerCellPropertiesEditor = CreateEditor(tableUI.headerCellProperties);

            if (selectionCellPropertiesEditor == null)
                selectionCellPropertiesEditor = CreateEditor(tableUI.selectionCellProperties);

            if (togglePropertiesEditor == null)
                togglePropertiesEditor = CreateEditor(tableUI.toggleProperties);

            if (buttonPropertiesEditor == null)
                buttonPropertiesEditor = CreateEditor(tableUI.buttonProperties);

            if (dropdownPropertiesEditor == null)
                dropdownPropertiesEditor = CreateEditor(tableUI.dropdownProperties);

            if (togglePropertiesSubsetEditor == null)
                togglePropertiesSubsetEditor = CreateEditor(tableUI.togglePropertiesSubset);

            if (buttonPropertiesSubsetEditor == null)
                buttonPropertiesSubsetEditor = CreateEditor(tableUI.buttonPropertiesSubset);

            if (dropdownPropertiesSubsetEditor == null)
                dropdownPropertiesSubsetEditor = CreateEditor(tableUI.dropdownPropertiesSubset);

            if (inputPropertiesEditor == null)
                inputPropertiesEditor = CreateEditor(tableUI.inputProperties);

            if (inputPropertiesSubsetEditor == null)
                inputPropertiesSubsetEditor = CreateEditor(tableUI.inputPropertiesSubset);
            
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.fontSize = 15;
            }

            if (labelStyle2 == null)
            {
                labelStyle2 = new GUIStyle();
                labelStyle2.fontStyle = FontStyle.Bold;
            }

        }      

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= tableUI.OnUndoRedoEvent;
        }
        public override void OnInspectorGUI()
        {


            tableUI.undoRedoEvent = Undo.GetCurrentGroupName();

            

            base.OnInspectorGUI();

            EditorGUILayout.LabelField("SIMPLE TABLE UI [PRO]", labelStyle);
            EditorGUILayout.LabelField("---------------------------------------", labelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Table Properties", labelStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(new GUIStyle("box"));

            EditorGUI.BeginChangeCheck();
            int rows = EditorGUILayout.IntSlider(new GUIContent("Row Number","The number of rows of the table."), tableUI.Rows, TableUI.MIN_ROWS, TableUI.MAX_ROWS);
            int columns = EditorGUILayout.IntSlider(new GUIContent("Column Number", "The number of columns of the table."), tableUI.Columns, TableUI.MIN_COL, TableUI.MAX_COL);
            bool header = EditorGUILayout.Toggle(new GUIContent("Show Header", "If checked, the first row will be a header."), tableUI.Header);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Table");
                tableUI.Rows = rows;
                tableUI.Columns = columns;
                tableUI.Header = header;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Border Properties", labelStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(new GUIStyle("box"));

            EditorGUI.BeginChangeCheck();
            BorderType borderType = (BorderType)EditorGUILayout.EnumPopup(new GUIContent("Border Type", "Style of the borders of the table."), tableUI.BorderType);
            float borderThickness = EditorGUILayout.FloatField(new GUIContent("Border Thickness", "How thick is the border of the table."), tableUI.BorderThickness);
            Color borderColor = EditorGUILayout.ColorField(new GUIContent("Border Color", "Color of the border of the table."), tableUI.BorderColor);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Border");
                tableUI.BorderType = borderType;
                tableUI.BorderThickness = borderThickness;
                tableUI.BorderColor = borderColor;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Table Colors", labelStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(new GUIStyle("box"));
            EditorGUI.BeginChangeCheck();
            bool striped = EditorGUILayout.Toggle(new GUIContent("Striped", "Set two colors for the background rows in an striped pattern (Header color not included)."), tableUI.Striped);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Color");
                tableUI.Striped = striped;
            }
            
            
            if (tableUI.Header)
            {
                EditorGUI.BeginChangeCheck();
                Color headerColor = EditorGUILayout.ColorField(new GUIContent("Header Color", "The background color of the header row."), tableUI.HeaderColor);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Color");
                    tableUI.HeaderColor = headerColor;
                }
            }

            EditorGUI.BeginChangeCheck();
            Color mainColor = EditorGUILayout.ColorField(new GUIContent("Main Color", "The background color of non header rows."), tableUI.MainColor);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Color");
                tableUI.MainColor = mainColor;

            }

            if (tableUI.Striped)
            {
                EditorGUI.BeginChangeCheck();
                Color secondaryColor = EditorGUILayout.ColorField(new GUIContent("Secondary Color", "The secondary background color of non header rows if Stripped is checked."), tableUI.SecondaryColor);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Color");
                    tableUI.SecondaryColor = secondaryColor;
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            Rect r = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Make all rows the same height", EditorStyles.miniButtonLeft, GUILayout.Height(50f), GUILayout.MinWidth(r.width / 2)))
            {
                tableUI.MakeAllRowsTheSameHeight();
            }
            if (GUILayout.Button("Make all columns the same width", EditorStyles.miniButtonRight, GUILayout.Height(50f), GUILayout.MinWidth(r.width / 2)))
            {
                tableUI.MakeAllColumnsTheSameWidth();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            
            DrawGrid();
            
        }

        private void TextProperties()
        {
            EditorGUILayout.BeginVertical(new GUIStyle("box"));

            EditorGUI.indentLevel += 1;

            if (tableUI.Header)
            {
                headerFoldout = EditorGUILayout.Foldout(headerFoldout, new GUIContent("Header Style"), true, TMP_UIStyleManager.boldFoldout);
                if (headerFoldout)
                {
                    headerCellPropertiesEditor.OnInspectorGUI();
                }
            }

            bodyFoldout = EditorGUILayout.Foldout(bodyFoldout, new GUIContent("Body Style"), true, TMP_UIStyleManager.boldFoldout);
            if (bodyFoldout)
            {
                bodyCellPropertiesEditor.OnInspectorGUI();

            }

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.EndVertical();
        }

        private void ToggleProperties()
        {
            if (tableUI.columnTypes.Contains(ColumnType.Toggle))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("box"));

                EditorGUI.indentLevel += 1;


                togglePropertiesEditor.OnInspectorGUI();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.EndVertical();
            }
        }

        private void ButtonProperties()
        {
            if (tableUI.columnTypes.Contains(ColumnType.Button))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("box"));

                EditorGUI.indentLevel += 1;


                buttonPropertiesEditor.OnInspectorGUI();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.EndVertical();
            }
        }

        private void DropdownProperties()
        {
            if (tableUI.columnTypes.Contains(ColumnType.Dropdown))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("box"));

                EditorGUI.indentLevel += 1;


                dropdownPropertiesEditor.OnInspectorGUI();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.EndVertical();
            }
        }
        
        private void InputProperties()
        {
            if (tableUI.columnTypes.Contains(ColumnType.Input))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("box"));

                EditorGUI.indentLevel += 1;

                inputPropertiesEditor.OnInspectorGUI();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.EndVertical();
            }
        }

        private void OnSceneGUI()
        {
            Vector2 size = tableUI.GetRectSize(tableUI.gameObject.GetComponent<RectTransform>());
            
            if (previousSize!=size)
            {
                previousSize = size;
                Undo.RecordObject(target, "Resize");
                tableUI.ResizeTable(size);
            }
                
            
        }

        int markRow = -1;
        int markColumn = -1;
        private void DrawGrid()
        {
            

            /*if (tableUI.chosenCell.y == -1 && tableUI.chosenCell.x == -1)
            {
                GUILayout.Space(18f);
            }*/
            DrawGridUpperConf();

            DrawGridTable();

            EditorGUILayout.Space();

            try
            {
                Vector2Int index = GetFirstValue();
                EditorGUILayout.Space();
                if(index.y==0 ||index.x==0)
                    toolbarValue = GUILayout.Toolbar(toolbarValue, new string[] { "Text", "Toggle", "Button", "Dropdown", "Input" }, EditorStyles.toolbarButton);
                if (index.x==0 && index.y==0)
                {
                    if (toolbarValue == 0)
                    {
                        TextProperties();
                    }else if (toolbarValue == 1)
                    {
                        ToggleProperties();
                    }else if (toolbarValue == 2)
                    {
                        ButtonProperties();
                    }else if (toolbarValue == 3)
                    {
                        DropdownProperties();
                    }else if (toolbarValue == 4)
                    {
                        InputProperties();
                    }
                    else
                    {
                        GUILayoutUtility.GetRect(Screen.width, 200f);
                    }
                }else if (index.y == 0)
                {
                    // 컬럼 선택 시 활성화할 탭 결정
                    int selectedColumnIndex = index.x - 1;
                    ColumnType selectedColumnType = tableUI.columnTypes[selectedColumnIndex];
                    
                    // 선택한 컬럼 타입과 일치하는 탭만 활성화
                    bool isTextTab = (toolbarValue == 0);
                    bool isToggleTab = (toolbarValue == 1);
                    bool isButtonTab = (toolbarValue == 2);
                    bool isDropdownTab = (toolbarValue == 3);
                    bool isInputTab = (toolbarValue == 4);
                    // bool isToggleTab = (toolbarValue == 1 && selectedColumnType == ColumnType.Toggle);
                    // bool isButtonTab = (toolbarValue == 2 && selectedColumnType == ColumnType.Button);
                    // bool isDropdownTab = (toolbarValue == 3 && selectedColumnType == ColumnType.Dropdown);
                    // bool isInputTab = (toolbarValue == 4 && selectedColumnType == ColumnType.Input);
                    
                    if (isTextTab)
                    {
                        try
                        {

                            tableUI.selectionCellProperties.min.x = index.x - 1;
                            tableUI.selectionCellProperties.max.x = index.x;
                            tableUI.selectionCellProperties.min.y = 0;
                            tableUI.selectionCellProperties.max.y = tableUI.Rows;

                            EditorGUILayout.BeginVertical(new GUIStyle("box"));
                            selectionCellPropertiesEditor.OnInspectorGUI();
                            EditorGUILayout.EndVertical();

                        }
                        catch (Exception) { }
                    }else if (isToggleTab)
                    {
                        Vector2Int min = tableUI.togglePropertiesSubset.Min;
                        Vector2Int max = tableUI.togglePropertiesSubset.Max;
                        min.x = index.x - 1;
                        max.x = index.x;
                        max.y = tableUI.Rows;
                        tableUI.togglePropertiesSubset.Min = min;
                        tableUI.togglePropertiesSubset.Max = max;
                        togglePropertiesSubsetEditor.OnInspectorGUI();


                    }
                    else if (isButtonTab)
                    {
                        Vector2Int min = tableUI.buttonPropertiesSubset.Min;
                        Vector2Int max = tableUI.buttonPropertiesSubset.Max;
                        min.x = index.x - 1;
                        max.x = index.x;
                        min.y = 0;
                        max.y = tableUI.Rows;
                        tableUI.buttonPropertiesSubset.Min = min;
                        tableUI.buttonPropertiesSubset.Max = max;
                        buttonPropertiesSubsetEditor.OnInspectorGUI();


                    }
                    else if (isDropdownTab)
                    {
                        Vector2Int min = tableUI.dropdownPropertiesSubset.Min;
                        Vector2Int max = tableUI.dropdownPropertiesSubset.Max;
                        min.x = index.x - 1;
                        max.x = index.x;
                        min.y = 0;
                        max.y = tableUI.Rows;
                        tableUI.dropdownPropertiesSubset.Min = min;
                        tableUI.dropdownPropertiesSubset.Max = max;
                        dropdownPropertiesSubsetEditor.OnInspectorGUI();
                    }
                    else if (isInputTab)
                    {
                        Vector2Int min = tableUI.inputPropertiesSubset.Min;
                        Vector2Int max = tableUI.inputPropertiesSubset.Max;
                        min.x = index.x - 1;
                        max.x = index.x;
                        min.y = 0;
                        max.y = tableUI.Rows;
                        tableUI.inputPropertiesSubset.Min = min;
                        tableUI.inputPropertiesSubset.Max = max;
                        inputPropertiesSubsetEditor.OnInspectorGUI();
                    }
                    else
                    {
                        GUILayoutUtility.GetRect(Screen.width, 200f);
                    }

                    
                }
                else if (index.x == 0)
                {
                    if (toolbarValue == 0)
                    {
                        try
                        {

                            tableUI.selectionCellProperties.min.x = 0;
                            tableUI.selectionCellProperties.max.x = tableUI.Columns;
                            tableUI.selectionCellProperties.min.y = index.y - 1;
                            tableUI.selectionCellProperties.max.y = index.y;
                            EditorGUILayout.BeginVertical(new GUIStyle("box"));
                            selectionCellPropertiesEditor.OnInspectorGUI();
                            EditorGUILayout.EndVertical();

                        }
                        catch (Exception) { }

                    }else if (toolbarValue==1 && tableUI.columnTypes.Contains(ColumnType.Toggle))
                    {
                        Vector2Int min = tableUI.togglePropertiesSubset.Min;
                        Vector2Int max = tableUI.togglePropertiesSubset.Max;
                        min.x = 0;
                        max.x = tableUI.Columns;
                        min.y = index.y - 1;
                        max.y = index.y;
                        tableUI.togglePropertiesSubset.Min = min;
                        tableUI.togglePropertiesSubset.Max = max;
                        togglePropertiesSubsetEditor.OnInspectorGUI();

                    }else if (toolbarValue==2 && tableUI.columnTypes.Contains(ColumnType.Button))
                    {
                        Vector2Int min = tableUI.buttonPropertiesSubset.Min;
                        Vector2Int max = tableUI.buttonPropertiesSubset.Max;
                        min.x = 0;
                        max.x = tableUI.Columns;
                        min.y = index.y - 1;
                        max.y = index.y;
                        tableUI.buttonPropertiesSubset.Min = min;
                        tableUI.buttonPropertiesSubset.Max = max;
                        buttonPropertiesSubsetEditor.OnInspectorGUI();

                    }
                    else if (toolbarValue==3 && tableUI.columnTypes.Contains(ColumnType.Dropdown))
                    {
                        Vector2Int min = tableUI.dropdownPropertiesSubset.Min;
                        Vector2Int max = tableUI.dropdownPropertiesSubset.Max;
                        min.x = 0;
                        max.x = tableUI.Columns;
                        min.y = index.y - 1;
                        max.y = index.y;
                        tableUI.dropdownPropertiesSubset.Min = min;
                        tableUI.dropdownPropertiesSubset.Max = max;
                        dropdownPropertiesSubsetEditor.OnInspectorGUI();
                    }
                    else if (toolbarValue==4 && tableUI.columnTypes.Contains(ColumnType.Input))
                    {
                        Vector2Int min = tableUI.inputPropertiesSubset.Min;
                        Vector2Int max = tableUI.inputPropertiesSubset.Max;
                        min.x = 0;
                        max.x = tableUI.Columns;
                        min.y = index.y - 1;
                        max.y = index.y;
                        tableUI.inputPropertiesSubset.Min = min;
                        tableUI.inputPropertiesSubset.Max = max;
                        inputPropertiesSubsetEditor.OnInspectorGUI();
                    }
                    else
                    {
                        GUILayoutUtility.GetRect(Screen.width, 200f);
                    }
                }
                else if (!index.Equals(Vector2.zero))
                {
                    Rect r = EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("Go to object",EditorStyles.miniButton, GUILayout.Height(50f),GUILayout.Width(Screen.width<450f? Screen.width*0.8f:400f)))
                    {
                        Selection.activeObject = ((Component)tableUI.data[index.y - 1].list[index.x - 1]).gameObject;
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    try
                    {
                        TextMeshProUGUI tmp = (TextMeshProUGUI)tableUI.data[index.y - 1].list[index.x - 1];
                        EditorGUILayout.BeginVertical(new GUIStyle("box"));
                        CreateEditor(tmp).OnInspectorGUI();
                        EditorGUILayout.EndVertical();
                        
                    }
                    catch (Exception) {

                        if (tableUI.columnTypes[index.x - 1].Equals(ColumnType.Toggle))
                        {
                            Vector2Int min = tableUI.togglePropertiesSubset.Min;
                            Vector2Int max = tableUI.togglePropertiesSubset.Max;
                            min.x = index.x-1;
                            max.x = index.x;
                            min.y = index.y-1;
                            max.y = index.y;
                            tableUI.togglePropertiesSubset.Min = min;
                            tableUI.togglePropertiesSubset.Max = max;
                            togglePropertiesSubsetEditor.OnInspectorGUI();
                        }else if (tableUI.columnTypes[index.x - 1].Equals(ColumnType.Button))
                        {
                            Vector2Int min = tableUI.buttonPropertiesSubset.Min;
                            Vector2Int max = tableUI.buttonPropertiesSubset.Max;
                            min.x = index.x - 1;
                            max.x = index.x;
                            min.y = index.y - 1;
                            max.y = index.y;
                            tableUI.buttonPropertiesSubset.Min = min;
                            tableUI.buttonPropertiesSubset.Max = max;
                            buttonPropertiesSubsetEditor.OnInspectorGUI();
                        }else if (tableUI.columnTypes[index.x - 1].Equals(ColumnType.Dropdown))
                        {
                            Vector2Int min = tableUI.dropdownPropertiesSubset.Min;
                            Vector2Int max = tableUI.dropdownPropertiesSubset.Max;
                            min.x = index.x - 1;
                            max.x = index.x;
                            min.y = index.y - 1;
                            max.y = index.y;
                            tableUI.dropdownPropertiesSubset.Min = min;
                            tableUI.dropdownPropertiesSubset.Max = max;
                            dropdownPropertiesSubsetEditor.OnInspectorGUI();
                        }
                        else if (tableUI.columnTypes[index.x - 1].Equals(ColumnType.Input))
                        {
                            Vector2Int min = tableUI.inputPropertiesSubset.Min;
                            Vector2Int max = tableUI.inputPropertiesSubset.Max;
                            min.x = index.x - 1;
                            max.x = index.x;
                            min.y = index.y - 1;
                            max.y = index.y;
                            tableUI.inputPropertiesSubset.Min = min;
                            tableUI.inputPropertiesSubset.Max = max;
                            inputPropertiesSubsetEditor.OnInspectorGUI();
                        }

                    }
                }
            }
            catch(System.Exception)
            {
            }
        }

        Vector2Int GetFirstValue()
        {
            for(int x =0; x < tableUI.Columns+1; x++)
            {
                for(int y =0; y< tableUI.Rows+1; y++)
                {
                    if (tableUI.Values[y].list[x])
                        return new Vector2Int(x, y);
                }
            }
            throw new System.Exception();
        }

        void SetAll(bool value)
        {
            for (int x = 0; x < tableUI.Columns + 1; x++)
            {
                for (int y = 0; y < tableUI.Rows + 1; y++)
                {
                    tableUI.Values[y].list[x] = value;
                }
            }
        }

        private void UpdateChosenCell()
        {
            if (tableUI.chosenCell.x >= tableUI.Columns || tableUI.chosenCell.y>=tableUI.Rows)
            {
                tableUI.chosenCell.x = -1;
                tableUI.chosenCell.y = -1;
            }
        }

        private void DrawGridUpperConf()
        {
            UpdateChosenCell();

            EditorGUILayout.BeginHorizontal(new GUIStyle("box"), GUILayout.Height(Screen.width < 450 ? 65 : 25));

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Bold;

            Rect r = EditorGUILayout.GetControlRect(false, 17);
            r.width = 40;
            float rx = r.x;
            if (tableUI.chosenCell.y != -1)
            {
                EditorGUI.LabelField(r, "Row: ");
                r.x += 30;
                EditorGUI.LabelField(r, tableUI.chosenCell.y.ToString());
                r.x += 20;
            }
            if (tableUI.chosenCell.x != -1)
            {
                r.width = 55;
                EditorGUI.LabelField(r, "Column: ");
                r.x += 50;
                EditorGUI.LabelField(r, tableUI.chosenCell.x.ToString());
                r.x += 30;
            }

            if (tableUI.chosenCell.y == -1 && tableUI.chosenCell.x >= 0)
            {
                if (Screen.width < 450f)
                {
                    r.x = rx;
                    r.y += 20;
                }

                r.width = 150;
                float labelwidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 90;
                tableUI.UpdateColumnWidth(EditorGUI.FloatField(r, "Column Width: ", tableUI.ColumnsWidth[tableUI.chosenCell.x]), tableUI.chosenCell.x);
                if (Screen.width < 450f)
                {
                    r.x = rx;
                    r.y += 20;
                }
                else
                {
                    r.x += 170;
                }

                r.width = 180;
                tableUI.UpdateColumnType(tableUI.chosenCell.x, (ColumnType)EditorGUI.Popup(r, "Column Type: ", (int)tableUI.columnTypes[tableUI.chosenCell.x], Enum.GetNames(typeof(ColumnType)), EditorStyles.popup));
                EditorGUIUtility.labelWidth = labelwidth;
            }
            else if (tableUI.chosenCell.x == -1 && tableUI.chosenCell.y >= 0)
            {
                if (Screen.width < 450f)
                {
                    r.x = rx;
                    r.y += 20;
                }
                r.width = 140;
                float labelwidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 75;
                tableUI.UpdateRowHeight(EditorGUI.FloatField(r, "Row Height: ", tableUI.RowsHeight[tableUI.chosenCell.y]), tableUI.chosenCell.y);
                if (Screen.width < 450f)
                {
                    r.x = rx;
                    r.y += 20;
                }
                else
                {
                    r.x += 155;
                }
                tableUI.UpdateSingleRowColor(tableUI.chosenCell.y, EditorGUI.ColorField(r, "Row Color: ", tableUI.GetSingleRowColor(tableUI.chosenCell.y)));
                EditorGUIUtility.labelWidth = labelwidth;
            }
            EditorGUILayout.EndHorizontal();
        }
       
        private void DrawGridTable() {
            GUIStyle columnStyle = new GUIStyle();

            columnStyle.fixedWidth = 32f;
            float h1 = 40f * (tableUI.Rows + 1);
            float h = h1 > 400 ? 400 : h1;

            EditorGUILayout.Space();
            gridScroll = EditorGUILayout.BeginScrollView(gridScroll, GUILayout.Height(h));

            EditorGUILayout.BeginHorizontal(new GUIStyle("box"));
            GUILayout.FlexibleSpace();
            for (int x = -1; x < tableUI.Columns; x++)
            {
                EditorGUILayout.BeginVertical(columnStyle);
                for (int y = -1; y < tableUI.Rows; y++)
                {
                    if (x == -1 && y == -1)
                    {
                        EditorGUI.BeginChangeCheck();
                        tableUI.Values[y + 1].list[x + 1] = EditorGUILayout.Toggle(tableUI.Values[y + 1].list[x + 1], tableUI.skin.customStyles[3], GUILayout.Width(30), GUILayout.Height(30));
                        if (EditorGUI.EndChangeCheck())
                        {
                            SetAll(tableUI.Values[y + 1].list[x + 1]);
                            if (tableUI.Values[y + 1].list[x + 1])
                            {
                                tableUI.chosenCell.x = 0;
                                tableUI.chosenCell.y = 0;
                            }
                            else
                            {
                                tableUI.chosenCell.x = -1;
                                tableUI.chosenCell.y = -1;
                            }
                        }
                    }
                    else if (x == -1)
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        tableUI.Values[y + 1].list[x + 1] = EditorGUILayout.Toggle(tableUI.Values[y + 1].list[x + 1], tableUI.skin.customStyles[1], GUILayout.Width(30), GUILayout.Height(30));

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (tableUI.Values[0].list[0])
                            {
                                SetAll(false);
                                continue;
                            }

                            markRow = y;
                            if (tableUI.Values[y + 1].list[x + 1])
                            {
                                SetAll(false);
                                tableUI.Values[y + 1].list[x + 1] = true;
                                tableUI.chosenCell.x = x;
                                tableUI.chosenCell.y = y;
                            }
                            else
                            {
                                tableUI.chosenCell.x = -1;
                                tableUI.chosenCell.y = -1;
                            }
                        }


                        EditorGUILayout.EndHorizontal();
                    }
                    else if (y == -1)
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUI.BeginChangeCheck();
                        tableUI.Values[y + 1].list[x + 1] = EditorGUILayout.Toggle(tableUI.Values[y + 1].list[x + 1], tableUI.skin.customStyles[2], GUILayout.Width(30), GUILayout.Height(30));
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (tableUI.Values[0].list[0])
                            {
                                SetAll(false);
                                continue;
                            }

                            markColumn = x;
                            if (tableUI.Values[y + 1].list[x + 1])
                            {
                                SetAll(false);
                                tableUI.Values[y + 1].list[x + 1] = true;
                                tableUI.chosenCell.x = x;
                                tableUI.chosenCell.y = y;
                            }
                            else
                            {
                                tableUI.chosenCell.x = -1;
                                tableUI.chosenCell.y = -1;
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        GUIStyle toggleStyle = new GUIStyle("toggle");

                        if (markRow == y || markColumn == x)
                        {
                            tableUI.Values[y + 1].list[x + 1] = tableUI.Values[y + 1].list[0] || tableUI.Values[0].list[x + 1];
                        }
                        else
                        {
                            bool initial = tableUI.Values[y + 1].list[x + 1];
                            tableUI.Values[y + 1].list[x + 1] = EditorGUILayout.Toggle(tableUI.Values[y + 1].list[x + 1], tableUI.skin.customStyles[0], GUILayout.Width(30), GUILayout.Height(30));
                            if (initial != tableUI.Values[y + 1].list[x + 1])
                            {
                                SetAll(false);
                                tableUI.Values[y + 1].list[x + 1] = !initial;
                                if (!initial)
                                {
                                    tableUI.chosenCell.x = x;
                                    tableUI.chosenCell.y = y;
                                }
                                else
                                {
                                    tableUI.chosenCell.x = -1;
                                    tableUI.chosenCell.y = -1;
                                }
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            markRow = -1;
            markColumn = -1;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

    }
}
