using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System.Collections;

// InputTableExample의 partial 클래스로 테이블 초기화 기능 구현
public partial class InputTableExample : MonoBehaviour
{
    // 테이블 초기화 함수
    // Awake/Start나 OnEnable에서 호출할 수 있음
    private void InitializeTable()
    {
        // 1. 모든 열을 Input 타입으로 변경
        SetAllColumnsToInputType();
        
        // 2. Header를 제외한 모든 행 삭제 (초기화)
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
}