using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// InputTableExample의 데이터 바인딩 기능을 담당하는 partial 클래스
public partial class TableController : MonoBehaviour
{
    // 샘플 데이터를 저장할 2차원 리스트
    private List<List<float>> tableData;

    // 입력이 변경될 때 호출되는 이벤트 핸들러
    public void OnInputValueChanged(int row, int column, string value)
    {
        // 입력된 값 출력
        if (outputText != null) outputText.text = $"입력된 값: ({row}, {column}) = {value}";
        Debug.Log($"Input Changed -> 행: {row}, 열: {column}, 값: {value}");

        // 테이블 데이터 업데이트 (데이터가 있는 경우)
        if (tableData != null && row < tableData.Count && column < tableData[row].Count)
            if (float.TryParse(value, out var floatValue))
                tableData[row][column] = floatValue;
    }

    // 샘플 데이터 생성 메서드
    public void CreateSampleData()
    {
        // 샘플 데이터를 저장할 2차원 리스트 초기화
        tableData = new List<List<float>>();

        // 헤더 행 데이터 (열 이름)
        var headerRow = new List<float>();
        for (var col = 0; col < table.Columns; col++) headerRow.Add(col + 1); // 헤더에 열 번호 할당 (1부터 시작)
        tableData.Add(headerRow);

        // 데이터 행들 생성 (3개의 행 추가)
        for (var row = 1; row <= 3; row++)
        {
            var rowData = new List<float>();
            for (var col = 0; col < table.Columns; col++)
            {
                // 샘플 데이터 생성 - 행 번호와 열 번호를 곱한 값
                float value = row * (col + 1);
                rowData.Add(value);
            }

            tableData.Add(rowData);
        }

        Debug.Log($"샘플 데이터 생성 완료: {tableData.Count}행 x {tableData[0].Count}열");
    }

    // 테이블에 데이터 바인딩 코루틴
    public IEnumerator BindDataToTableCoroutine(List<List<float>> data)
    {
        if (data == null || data.Count == 0)
        {
            Debug.LogWarning("바인딩할 데이터가 없습니다.");
            yield break;
        }

        // 데이터의 행 수에 맞게 테이블 행 수 조정 (헤더 포함)
        var dataRows = data.Count;
        if (table.Rows != dataRows)
        {
            Debug.Log($"테이블 행 수 조정: {table.Rows} -> {dataRows}");
            table.Rows = dataRows;
            yield return null; // 변경 사항이 적용될 시간을 줌
        }

        // 데이터의 열 수에 맞게 테이블 열 수 조정
        var maxColumns = 0;
        foreach (var row in data) maxColumns = Mathf.Max(maxColumns, row.Count);

        if (table.Columns != maxColumns)
        {
            Debug.Log($"테이블 열 수 조정: {table.Columns} -> {maxColumns}");
            table.Columns = maxColumns;
            yield return null; // 변경 사항이 적용될 시간을 줌
        }

        // 모든 열이 Input 타입인지 확인
        SetAllColumnsToInputType();
        yield return null; // 변경 사항이 적용될 시간을 줌

        // 테이블 새로고침 - 행/열 수 변경 적용
        table.Refresh();
        yield return new WaitForSeconds(0.1f); // 새로고침 후 약간 대기

        // 이제 각 셀에 데이터 할당
        try
        {
            AssignDataToCells(data);
            Debug.Log("테이블 데이터 바인딩 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"테이블 데이터 할당 중 오류 발생: {e.Message}\n{e.StackTrace}");
        }
    }

    // 테이블에 데이터 바인딩 메서드
    public void BindDataToTable(List<List<float>> data)
    {
        StartCoroutine(BindDataToTableCoroutine(data));
    }

    // 셀에 데이터 할당 메서드 (BindDataToTable에서 분리)
    private void AssignDataToCells(List<List<float>> data)
    {
        // 테이블과 데이터 상태 확인
        if (table == null)
        {
            Debug.LogError("테이블 참조가 null입니다.");
            return;
        }

        if (table.data == null)
        {
            Debug.LogError("테이블 데이터가 초기화되지 않았습니다.");
            return;
        }

        Debug.Log($"데이터 할당 시작: 테이블 행 수={table.Rows}, 데이터 행 수={data.Count}");

        // 각 셀에 데이터 할당
        for (var row = 0; row < data.Count && row < table.Rows; row++)
        {
            if (row >= table.data.Count)
            {
                Debug.LogWarning($"행 {row}가 테이블 데이터의 범위를 벗어납니다 (테이블 데이터 행 수: {table.data.Count})");
                continue;
            }
            
            Debug.Log($"행 {row} 데이터 할당: 데이터 열 수={data[row].Count}, 테이블 열 수={table.Columns}");

            // 각 열에 데이터 할당
            for (var col = 0; col < data[row].Count && col < table.Columns; col++)
            {
                try
                {
                    // SetCellNumericValue 메서드 사용하여 값 설정
                    // 이 메서드는 내부적으로 예외 처리 및 로깅을 수행
                    SetCellNumericValue(row, col, data[row][col]);
                }
                catch (Exception e)
                {
                    // // [Fix this comment]: 초기 데이터 할당시 OnValueChanged가 오동작 하지만 이 이벤트 구조는 이후에 정상 작동
                    // // [Fix this comment]: 때문에 이 오류 구문에 입장하더라도 초기 할당 오류이므로 추가 처리하지 않음
                    // Debug.LogError($"셀({row}, {col}) 데이터 할당 중 심각한 오류: {e.Message}");
                }
            }
        }
    }

    // 테이블에서 데이터 가져오기 메서드
    public List<List<float>> GetDataFromTable()
    {
        var result = new List<List<float>>();

        // 테이블에 행이 없으면 빈 리스트 반환
        if (table.Rows <= 0) return result;

        // 테이블의 각 행에서 데이터 읽기
        for (var row = 0; row < table.Rows; row++)
        {
            var rowData = new List<float>();

            for (var col = 0; col < table.Columns; col++)
            {
                var value = 0f;

                try
                {
                    var cellObject = table.data[row].list[col];
                    var cellText = "";

                    if (cellObject is TMP_InputField inputField)
                        cellText = inputField.text;
                    else if (cellObject is TextMeshProUGUI textField) cellText = textField.text;

                    // 문자열을 실수로 변환
                    if (!string.IsNullOrEmpty(cellText) && float.TryParse(cellText, out var parsedValue))
                        value = parsedValue;
                }
                catch (Exception)
                {
                    // 예외 발생 시 기본값 0 사용
                    value = 0f;
                }

                rowData.Add(value);
            }

            result.Add(rowData);
        }

        return result;
    }
}