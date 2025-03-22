using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTest : MonoBehaviour
{
    public TableController tableController;
    
    public void OnTestTableUI()
    {
        // 샘플 데이터를 생성합니다.
        List<List<float>> dataset = new List<List<float>>();
        dataset.Add(new List<float>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        dataset.Add(new List<float>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        dataset.Add(new List<float>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        List<string> labels = new List<string>() { "label1", "label2", "label3", "label4", "label5", "label6", "label7", "label8", "label9", "label10" };
        
        
        tableController.ImportDataset(dataset, labels);
    }
    
    public void OnExportTableUI()
    {
        // TableController에서 현재 테이블 데이터 내보내기
        List<List<string>> dataset;
        List<string> labels;
    
        if (tableController.ExportDataset(out dataset, out labels))
        {
            // 내보내기 성공
            Debug.Log("테이블 데이터 내보내기 성공:");
        
            // 헤더 로깅
            Debug.Log("헤더: " + string.Join(", ", labels));
        
            // 행 데이터 로깅
            for (int i = 0; i < dataset.Count; i++)
            {
                Debug.Log($"행 {i+1}: " + string.Join(", ", dataset[i]));
            }
        
            // 여기서 내보낸 데이터로 추가 작업을 수행할 수 있습니다.
            // 예: 파일로 저장, 다른 시스템으로 전송 등
        }
        else
        {
            // 내보내기 실패
            Debug.LogError("테이블 데이터 내보내기 실패");
        }
    }
}
