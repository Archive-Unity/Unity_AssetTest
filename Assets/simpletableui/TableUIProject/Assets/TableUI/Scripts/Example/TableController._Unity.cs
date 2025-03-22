using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System.Collections;

// InputTableExample의 Unity 예약 메서드들을 포함하는 partial 클래스
public partial class TableController : MonoBehaviour
{
    public TableUI table;
    public Text outputText; // 출력 텍스트를 표시할 UI Text
    
    // Awake 메서드 - 컴포넌트가 초기화될 때 호출됨
    private void Awake()
    {
        // 기본 초기화 작업이 필요하다면 여기에 구현
    }

    // Start 메서드 - 첫 번째 프레임 업데이트 전에 호출됨
    private void Start()
    {
        // 테이블 초기화가 완료될 시간을 주기 위해 코루틴 실행
        StartCoroutine(InitializeAndBindData());
    }
    
    private IEnumerator InitializeAndBindData()
    {
        // OnEnable에서 초기화가 완료될 때까지 잠시 대기
        yield return new WaitForSeconds(0.2f);
        
        // 샘플 데이터 생성
        CreateSampleData();
    
        // 테이블에 데이터 바인딩
        yield return StartCoroutine(BindDataToTableCoroutine(tableData));
    
        
        // // 또는 데이터에 맞게 자동으로 크기 조정
        // ResizeTableToFitData(tableData);
        
        Debug.Log("테이블 초기화 및 데이터 바인딩 완료");
    }

    // OnEnable 메서드 - 게임 오브젝트가 활성화될 때 호출됨
    private void OnEnable()
    {
        // 약간의 딜레이 후 초기화 (테이블이 완전히 로드된 후)
        StartCoroutine(InitializeTableWithDelay());
    }

    // OnDisable 메서드 - 게임 오브젝트가 비활성화될 때 호출됨
    private void OnDisable()
    {
        // 이벤트 핸들러 해제 등의 정리 작업이 필요하다면 여기에 구현
        if (table != null && table.inputProperties != null)
        {
            table.inputProperties.onInputValueChange.RemoveListener(OnInputValueChanged);
        }
    }

    // 약간의 딜레이 후 테이블 초기화 (안정성을 위해)
    private IEnumerator InitializeTableWithDelay()
    {
        // 한 프레임 기다리기
        yield return null;
        
        // 테이블 초기화
        InitializeTable(resetColumns:true, defaultColumnCount: 3);
        
        // 입력 이벤트 핸들러 등록
        if (table != null && table.inputProperties != null)
        {
            table.inputProperties.onInputValueChange.AddListener(OnInputValueChanged);
        }
    }
}