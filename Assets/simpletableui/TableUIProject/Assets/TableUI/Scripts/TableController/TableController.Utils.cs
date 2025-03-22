using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;

public partial class TableController : MonoBehaviour
{
    #region Get, Set Cell

    /// <summary>
    /// 지정된 셀의 텍스트 값을 가져옵니다.
    /// </summary>
    /// <param name="row">행 인덱스</param>
    /// <param name="col">열 인덱스</param>
    /// <returns>셀 텍스트 값</returns>
    string GetCellTextValue(int row, int col)
    {
        try
        {
            if (table == null || row < 0 || row >= table.Rows || col < 0 || col >= table.Columns)
                return string.Empty;
        
            var cellObj = table.data[row].list[col];
        
            if (cellObj is TMP_InputField inputField)
            {
                return inputField.text;
            }
            else if (cellObj is TextMeshProUGUI textField)
            {
                return textField.text;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"셀 텍스트 가져오기 오류 ({row}, {col}): {ex.Message}");
            return string.Empty;
        }
    }
    
    /// <summary>
    /// 지정된 셀에 텍스트 값을 설정합니다.
    /// </summary>
    /// <param name="row">행 인덱스</param>
    /// <param name="col">열 인덱스</param>
    /// <param name="value">설정할 텍스트 값</param>
    /// <returns>성공 여부</returns>
    public bool SetCellTextValue(int row, int col, string value)
    {
        try
        {
            if (table == null || row < 0 || row >= table.Rows || col < 0 || col >= table.Columns)
                return false;
        
            var cellObj = table.data[row].list[col];
        
            if (cellObj is TMP_InputField inputField)
            {
                inputField.text = value;
                return true;
            }
            else if (cellObj is TextMeshProUGUI textField)
            {
                textField.text = value;
                return true;
            }
            else
            {
                Debug.LogWarning($"셀({row}, {col})이 지원되지 않는 타입입니다: {cellObj.GetType().Name}");
                return false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"셀 텍스트 설정 오류 ({row}, {col}): {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 특정 셀에 숫자 값을 설정합니다.
    /// </summary>
    /// <param name="row">행 인덱스</param>
    /// <param name="col">열 인덱스</param>
    /// <param name="value">설정할 숫자 값</param>
    /// <param name="format">숫자 형식 문자열 (예: "F2")</param>
    /// <returns>성공 여부</returns>
    public bool SetCellNumericValue(int row, int col, float value, string format = "")
    {
        string formattedValue = string.IsNullOrEmpty(format) ? 
            value.ToString() : 
            value.ToString(format);
    
        return SetCellTextValue(row, col, formattedValue);
    }

    /// <summary>
    /// 지정된 셀의 숫자 값을 가져옵니다.
    /// </summary>
    /// <param name="row">행 인덱스</param>
    /// <param name="col">열 인덱스</param>
    /// <param name="defaultValue">파싱 실패 시 반환할 기본값</param>
    /// <returns>셀의 숫자 값</returns>
    public float GetCellNumericValue(int row, int col, float defaultValue = 0f)
    {
        string cellText = GetCellTextValue(row, col);
    
        if (float.TryParse(cellText, out float value))
        {
            return value;
        }
    
        return defaultValue;
    }
    
    #endregion

    #region Cells (Row, Column, All)

    /// <summary>
    /// 테이블의 모든 셀을 초기화합니다.
    /// </summary>
    /// <param name="preserveHeader">헤더 보존 여부</param>
    /// <param name="defaultValue">기본값 (비워두려면 빈 문자열 사용)</param>
    /// <returns>성공 여부</returns>
    public bool ClearAllCells(bool preserveHeader = true, string defaultValue = "")
    {
        if (table == null || table.Rows <= 0)
            return false;
    
        int startRow = preserveHeader && table.Header ? 1 : 0;
    
        for (int row = startRow; row < table.Rows; row++)
        {
            for (int col = 0; col < table.Columns; col++)
            {
                SetCellTextValue(row, col, defaultValue);
            }
        }
    
        return true;
    }
    
    /// <summary>
    /// 특정 행의 모든 셀을 설정합니다.
    /// </summary>
    /// <param name="rowIndex">행 인덱스</param>
    /// <param name="values">설정할 값 목록</param>
    /// <returns>성공 여부</returns>
    public bool SetRowValues(int rowIndex, List<string> values)
    {
        if (table == null || rowIndex < 0 || rowIndex >= table.Rows || values == null)
            return false;
    
        for (int col = 0; col < Mathf.Min(table.Columns, values.Count); col++)
        {
            SetCellTextValue(rowIndex, col, values[col]);
        }
    
        return true;
    }
    
    /// <summary>
    /// 특정 열의 모든 셀을 설정합니다.
    /// </summary>
    /// <param name="columnIndex">열 인덱스</param>
    /// <param name="values">설정할 값 목록</param>
    /// <param name="startFromHeader">헤더부터 설정할지 여부</param>
    /// <returns>성공 여부</returns>
    public bool SetColumnValues(int columnIndex, List<string> values, bool startFromHeader = false)
    {
        if (table == null || columnIndex < 0 || columnIndex >= table.Columns || values == null)
            return false;
    
        int startRow = startFromHeader || !table.Header ? 0 : 1;
        int valueIndex = 0;
    
        for (int row = startRow; row < table.Rows && valueIndex < values.Count; row++, valueIndex++)
        {
            SetCellTextValue(row, columnIndex, values[valueIndex]);
        }
    
        return true;
    }

    #endregion
    
    
}