using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System.Collections;
using System.Collections.Generic;

// InputTableExample의 partial 클래스로 테이블 초기화 기능 구현
public partial class TableController : MonoBehaviour
{
    // 테이블 초기화 함수
    // Awake/Start나 OnEnable에서 호출할 수 있음
    private void InitializeTable(bool resetColumns = false, int defaultColumnCount = 1)
    {
        // 1. 모든 열을 Input 타입으로 변경
        SetAllColumnsToInputType();
        
        // 2. 열 초기화 옵션이 켜져 있으면 열 초기화 수행
        if (resetColumns)
        {
            RemoveAllColumnsExceptDefault(defaultColumnCount);
        }
        
        // 3. Header를 제외한 모든 행 삭제 (초기화)
        RemoveAllRowsExceptHeader();
    }

    // 테이블의 모든 열을 Input 타입으로 변경
    private void SetAllColumnsToInputType()
    {
        // 각 열에 대해 수행
        for (int columnIndex = 0; columnIndex < table.Columns; columnIndex++)
        {
            // 현재 열 타입이 Input이 아닌 경우만 변경
            if (table.columnTypes[columnIndex] != ColumnType.Input)
            {
                // 열 타입을 Input으로 변경
                table.UpdateColumnType(columnIndex, ColumnType.Input);
            }
        }

        // 변경 내용 적용
        table.Refresh();
    }

    // Header를 제외한 모든 행을 제거 (테이블 초기화)
    private void RemoveAllRowsExceptHeader()
    {
        // 테이블에 헤더가 있고, 행이 1개 이상인 경우에만
        if (table.Header && table.Rows > 1)
        {
            // 헤더 행을 제외한 모든 행 제거
            // Rows 프로퍼티에 값을 직접 할당하면 테이블 크기가 변경됨
            table.Rows = 1; // 헤더만 남김
        }
        // 헤더가 없는 경우 (특수 상황)
        else if (!table.Header && table.Rows > 0)
        {
            // 최소 1행은 유지해야 하므로
            table.Rows = 1;
        }

        // 변경 내용 적용
        table.Refresh();
    }
    
    // InputTableExample.Initialize.cs에 추가할 메소드
    private void RemoveAllColumnsExceptDefault(int keepCount = 1)
    {
        // 테이블에 열이 있고, 유지할 개수보다 많은 경우에만 처리
        if (table.Columns > keepCount)
        {
            // 열 개수를 keepCount로 설정 (기본값 1)
            table.Columns = keepCount;
        
            // 변경 내용 적용
            table.Refresh();
        }
    }
    
    // 데이터에 맞게 테이블 구조 초기화 메소드
    private void ResizeTableToFitData(List<List<float>> data, bool preserveExistingData = false)
    {
        if (data == null || data.Count == 0)
        {
            Debug.LogWarning("ResizeTableToFitData: 유효한 데이터가 없습니다.");
            return;
        }

        int rowCount = data.Count;
    
        // 데이터의 모든 행을 검사하여 최대 열 수 찾기
        int maxColumnCount = 0;
        foreach (var row in data)
        {
            maxColumnCount = Mathf.Max(maxColumnCount, row.Count);
        }
    
        // 최소 열 수는 1
        maxColumnCount = Mathf.Max(1, maxColumnCount);
    
        // 기존 데이터 보존이 필요 없다면 테이블 완전 초기화
        if (!preserveExistingData)
        {
            // 모든 열을 Input 타입으로 설정
            SetAllColumnsToInputType();
        
            // 헤더만 남기고 모든 행 제거
            RemoveAllRowsExceptHeader();
        }
    
        // 테이블 크기 조정
        table.Rows = table.Header ? rowCount + 1 : rowCount;  // 헤더가 있으면 +1
        table.Columns = maxColumnCount;
    
        // 변경 내용 적용
        table.Refresh();
    
        Debug.Log($"테이블 크기가 조정되었습니다: {table.Rows}행 x {table.Columns}열");
    }
}