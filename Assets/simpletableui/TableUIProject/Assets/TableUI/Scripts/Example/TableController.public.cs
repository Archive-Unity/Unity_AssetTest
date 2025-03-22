using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using TMPro;

public partial class TableController : MonoBehaviour
{
    /// <summary>
    /// 외부 데이터셋을 테이블로 가져와 표시합니다.
    /// </summary>
    /// <param name="dataset">가져올 데이터셋 (2차원 float 리스트)</param>
    /// <param name="labels">열 레이블 목록 (null이면 "Column 1", "Column 2" 등으로 표시)</param>
    /// <param name="resetTable">테이블을 완전히 초기화할지 여부</param>
    /// <returns>데이터 가져오기 성공 여부</returns>
    public bool ImportDataset(List<List<float>> dataset, List<string> labels = null, bool resetTable = true)
    {
        if (dataset == null || dataset.Count == 0)
        {
            Debug.LogError("ImportDataset: 유효하지 않은 데이터셋입니다.");
            return false;
        }

        // 코루틴 시작하고 성공적으로 시작되었으면 true 반환
        StartCoroutine(ImportDatasetCoroutine(dataset, labels, resetTable));
        return true;
    }

    /// <summary>
    /// 데이터셋 가져오기를 처리하는 코루틴
    /// </summary>
    private IEnumerator ImportDatasetCoroutine(List<List<float>> dataset, List<string> labels, bool resetTable)
    {
        // UI 상태 업데이트 (로딩 메시지 등 표시 가능)
        if (outputText != null)
            outputText.text = "데이터 로딩 중...";

        // 1. 테이블 준비
        if (resetTable)
        {
            // 모든 열을 Input 타입으로 변경
            SetAllColumnsToInputType();
            
            // 헤더가 항상 필요하므로 최소 1행 유지
            if (table.Rows > 0)
                table.Rows = 1;
                
            yield return null; // 프레임 대기
        }

        // 2. 데이터셋 분석
        int rowCount = dataset.Count;
        int maxColumnCount = 0;
        
        foreach (var row in dataset)
        {
            maxColumnCount = Mathf.Max(maxColumnCount, row.Count);
        }
        
        // 레이블의 수가 열 수보다 많으면 열 수 조정
        if (labels != null)
            maxColumnCount = Mathf.Max(maxColumnCount, labels.Count);

        // 3. 테이블 크기 조정
        bool needsRefresh = false;
        
        // 열 수 조정
        if (table.Columns != maxColumnCount)
        {
            table.Columns = maxColumnCount;
            needsRefresh = true;
        }
        
        // 행 수 조정 (헤더 포함)
        int targetRows = rowCount + 1; // 헤더 행 + 데이터 행
        if (table.Rows != targetRows)
        {
            table.Rows = targetRows;
            needsRefresh = true;
        }
        
        if (needsRefresh)
        {
            table.Refresh();
            yield return new WaitForSeconds(0.1f); // 변경 적용 시간
        }
        
        // 헤더 설정 (항상 헤더 사용)
        table.Header = true;

        // 4. 데이터 채우기
        // 헤더 행 채우기
        for (int col = 0; col < table.Columns; col++)
        {
            try
            {
                var cellObj = table.data[0].list[col];
                string headerText = "Column " + (col + 1);
                
                // 레이블이 있으면 사용
                if (labels != null && col < labels.Count)
                    headerText = labels[col];
                
                if (cellObj is TextMeshProUGUI textField)
                {
                    textField.text = headerText;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"헤더 채우기 오류 ({col}): {ex.Message}");
            }
        }
        
        yield return null; // 프레임 대기
        
        // 데이터 셀 채우기
        for (int row = 0; row < dataset.Count; row++)
        {
            int tableRow = row + 1; // 헤더 다음 행부터 시작
            if (tableRow >= table.Rows) break;
            
            var dataRow = dataset[row];
            
            for (int col = 0; col < table.Columns; col++)
            {
                try
                {
                    var cellObj = table.data[tableRow].list[col];
                    string valueText = col < dataRow.Count ? dataRow[col].ToString() : "0";
                    
                    if (cellObj is TMP_InputField inputField)
                    {
                        inputField.text = valueText;
                    }
                    else if (cellObj is TextMeshProUGUI textField)
                    {
                        textField.text = valueText;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"데이터 채우기 오류 ({tableRow}, {col}): {ex.Message}");
                }
            }
            
            // 10행마다 프레임 양보 (성능 개선)
            if (row % 10 == 0)
                yield return null;
        }
        
        // 5. 완료 메시지
        if (outputText != null)
            outputText.text = $"데이터 로딩 완료: {dataset.Count} 행, {maxColumnCount} 열";
            
        Debug.Log($"데이터셋 가져오기 완료: {dataset.Count} 행, {maxColumnCount} 열");
    }
    
    /// <summary>
    /// 헤더 값을 지정된 문자열 배열로 설정합니다.
    /// </summary>
    /// <param name="labels">헤더 레이블 목록</param>
    public void SetHeaderLabels(List<string> labels)
    {
        if (labels == null || labels.Count == 0 || !table.Header)
            return;
            
        for (int col = 0; col < Mathf.Min(labels.Count, table.Columns); col++)
        {
            try
            {
                var cellObj = table.data[0].list[col];
                if (cellObj is TextMeshProUGUI textField)
                {
                    textField.text = labels[col];
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"헤더 레이블 설정 오류 ({col}): {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// CSV 형식의 문자열을 파싱하여 테이블로 가져옵니다.
    /// </summary>
    /// <param name="csvText">CSV 문자열</param>
    /// <param name="hasHeaderRow">CSV의 첫 행이 헤더인지 여부</param>
    /// <param name="delimiter">구분자 (기본값: 쉼표)</param>
    /// <returns>가져오기 성공 여부</returns>
    public bool ImportFromCSV(string csvText, bool hasHeaderRow = true, char delimiter = ',')
    {
        if (string.IsNullOrEmpty(csvText))
        {
            Debug.LogError("ImportFromCSV: CSV 텍스트가 비어 있습니다.");
            return false;
        }
        
        try
        {
            // CSV 파싱
            string[] lines = csvText.Split('\n');
            List<List<float>> dataset = new List<List<float>>();
            List<string> headerLabels = null;
            
            int startRow = hasHeaderRow ? 1 : 0;
            
            // 헤더 처리
            if (hasHeaderRow && lines.Length > 0)
            {
                string[] headerValues = lines[0].Split(delimiter);
                headerLabels = new List<string>();
                
                for (int i = 0; i < headerValues.Length; i++)
                {
                    headerLabels.Add(headerValues[i].Trim());
                }
            }
            
            // 데이터 행 처리
            for (int i = startRow; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                
                string[] values = line.Split(delimiter);
                List<float> dataRow = new List<float>();
                
                foreach (string value in values)
                {
                    if (float.TryParse(value.Trim(), out float parsedValue))
                    {
                        dataRow.Add(parsedValue);
                    }
                    else
                    {
                        dataRow.Add(0f); // 파싱 실패 시 0 추가
                    }
                }
                
                dataset.Add(dataRow);
            }
            
            // 테이블에 가져오기
            return ImportDataset(dataset, headerLabels, true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 가져오기 오류: {ex.Message}");
            return false;
        }
    }
}