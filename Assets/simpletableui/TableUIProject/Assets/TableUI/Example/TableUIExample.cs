
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;

namespace UnityEngine.UI.TableUI
{
    public class TableUIExample : MonoBehaviour
    {
        public TableUI table;
        public Text rows, cols, text;

        void Start()
        {
            Debug.developerConsoleVisible = true;
        }

        public void OnChangeTextValue()
        {
            int r = System.Int32.Parse(rows.text);
            int c = System.Int32.Parse(cols.text);
            string value = text.text;

            if (r < TableUI.MIN_ROWS - 1 || r >= table.Rows)
            {
                Debug.Log("The row number is not in range");
                return;
            }

            if (c < TableUI.MIN_COL - 1 || c >= table.Columns)
            {
                Debug.Log("The column number is not in range");
                return;
            }
            try
            {
                ((TextMeshProUGUI)table.data[r].list[c]).GetComponent<TextMeshProUGUI>().text = value;
            }
            catch (System.Exception) { }
        }

        public void OnToggleChangeEvent(int row, int column, bool value)
        {
            Debug.Log("On Toggle Changed-> row:" + row + " column: " + column + " value: " + value);
        }

        public void OnButtonPressed(int row, int column)
        {
            Debug.Log("On Button Pressed-> row:" + row + " column: " + column);
        }

        public void OnDropdownChanged(int row, int column, int value)
        {
            Debug.Log("On DropdownChanged Changed-> row:" + row + " column: " + column + " value: " + value);
        }

        public void OnAddNewRowClick()
        {
            table.Rows++;
        }

        public void OnAddNewColumnClick()
        {
            table.Columns++;
        }

        public void OnRemoveLastColumn()
        {
            table.Columns--;
        }

        public void OnRemoveLastRow()
        {
            table.Rows--;
        }


    }
}
