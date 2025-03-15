using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeViewManager : MonoBehaviour
{
    public static TreeViewManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Button rootButton;
    [SerializeField] private GameObject treeViewContent; // 콘텐츠 영역
    [SerializeField] private GameObject scrollViewObject; // ScrollView GameObject
    [SerializeField] private TreeViewItem treeViewItemPrefab;
    [SerializeField] private ScrollRect scrollRect;
    
    [Header("Layout Settings")]
    [SerializeField] private VerticalLayoutGroup contentLayoutGroup;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private float scrollTopPadding = 10f; // 스크롤 시 상단 여백

    private bool isContentExpanded = false;
    private List<TreeViewItem> rootItems = new List<TreeViewItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // ScrollView 참조 확인
        if (scrollViewObject == null)
        {
            // ScrollRect 컴포넌트를 갖고 있는 GameObject를 찾음
            if (scrollRect != null)
            {
                scrollViewObject = scrollRect.gameObject;
            }
        }
        
        // 레이아웃 컴포넌트 확인
        if (contentLayoutGroup == null && treeViewContent != null)
        {
            contentLayoutGroup = treeViewContent.GetComponent<VerticalLayoutGroup>();
            if (contentLayoutGroup == null)
            {
                contentLayoutGroup = treeViewContent.AddComponent<VerticalLayoutGroup>();
                contentLayoutGroup.childControlHeight = true;
                contentLayoutGroup.childForceExpandHeight = false;
            }
        }
        
        if (contentSizeFitter == null && treeViewContent != null)
        {
            contentSizeFitter = treeViewContent.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = treeViewContent.AddComponent<ContentSizeFitter>();
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }
        
        // ScrollRect 확인
        if (scrollRect == null)
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect != null && scrollRect.content == null)
            {
                scrollRect.content = treeViewContent.GetComponent<RectTransform>();
            }
        }
    }

    private void Start()
    {
        rootButton.onClick.AddListener(ToggleTreeViewContent);
        
        // ScrollView 초기 상태 설정
        if (scrollViewObject != null)
        {
            scrollViewObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ScrollView GameObject가 할당되지 않았습니다.");
        }
    }

    private void ToggleTreeViewContent()
    {
        isContentExpanded = !isContentExpanded;
        
        // ScrollView 토글
        if (scrollViewObject != null)
        {
            scrollViewObject.SetActive(isContentExpanded);
            
            // 스크롤뷰 상태 갱신
            if (isContentExpanded)
            {
                StartCoroutine(RefreshTreeViewLayout());
                
                // 스크롤을 맨 위로 이동
                if (scrollRect != null)
                {
                    scrollRect.normalizedPosition = new Vector2(0, 1);
                }
            }
        }
        else
        {
            // 이전 방식으로 폴백 (ScrollView가 없는 경우)
            treeViewContent.SetActive(isContentExpanded);
        }
    }
    
    private IEnumerator RefreshTreeViewLayout()
    {
        // UI 업데이트를 위한 지연
        yield return null;
        
        Canvas.ForceUpdateCanvases();
        
        if (contentLayoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(treeViewContent.GetComponent<RectTransform>());
        }
        
        // 콘텐츠 크기 재계산 후 스크롤 조정
        yield return null;
        
        if (scrollRect != null)
        {
            // 스크롤 위치 조정
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }
    
    // 아이템이 확장/축소될 때 호출 (TreeViewItem에서 참조)
    public void NotifyItemExpandStateChanged(TreeViewItem item)
    {
        StartCoroutine(RefreshAfterExpandStateChanged(item));
    }
    
    private IEnumerator RefreshAfterExpandStateChanged(TreeViewItem item)
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        
        // 확장된 아이템이 스크롤 영역을 벗어나면 스크롤 조정
        if (item.IsExpanded() && scrollRect != null)
        {
            yield return null;
            
            // 아이템의 위치 계산
            RectTransform itemRect = item.GetComponent<RectTransform>();
            Vector3[] itemCorners = new Vector3[4];
            itemRect.GetWorldCorners(itemCorners);
            
            // 뷰포트의 위치 계산
            RectTransform viewportRect = scrollRect.viewport;
            Vector3[] viewportCorners = new Vector3[4];
            viewportRect.GetWorldCorners(viewportCorners);
            
            // 아이템이 뷰포트 하단을 벗어났는지 확인
            if (itemCorners[0].y < viewportCorners[0].y)
            {
                // 스크롤 조정 로직 추가
                // 이 부분은 아이템을 화면에 표시하기 위한 스크롤 위치 계산 로직
                float itemBottom = itemCorners[0].y;
                float viewportBottom = viewportCorners[0].y;
                float difference = (viewportBottom - itemBottom) / scrollRect.content.rect.height;
                
                Vector2 newPosition = scrollRect.normalizedPosition;
                newPosition.y = Mathf.Clamp01(scrollRect.normalizedPosition.y - difference);
                scrollRect.normalizedPosition = newPosition;
            }
        }
    }

    // TreeView 아이템 생성 메서드
    public TreeViewItem CreateTreeViewItem(string text)
    {
        TreeViewItem newItem = Instantiate(treeViewItemPrefab);
        newItem.Initialize(text);
        return newItem;
    }

    // 루트 레벨에 아이템 추가
    public TreeViewItem AddRootItem(string text)
    {
        TreeViewItem rootItem = CreateTreeViewItem(text);
        rootItem.transform.SetParent(treeViewContent.transform, false);
        rootItems.Add(rootItem);
        return rootItem;
    }

    // 트리뷰 콘텐츠 확장 상태 설정
    public void SetContentExpanded(bool expanded)
    {
        if (isContentExpanded != expanded)
        {
            isContentExpanded = expanded;
            
            // ScrollView 토글
            if (scrollViewObject != null)
            {
                scrollViewObject.SetActive(isContentExpanded);
                
                if (isContentExpanded)
                {
                    StartCoroutine(RefreshTreeViewLayout());
                }
            }
            else
            {
                // 이전 방식으로 폴백
                treeViewContent.SetActive(isContentExpanded);
            }
        }
    }
    
    // 특정 아이템을 스크롤 뷰 중앙에 표시
    public void ScrollToItem(TreeViewItem item)
    {
        if (scrollRect == null || item == null)
            return;
            
        StartCoroutine(ScrollToItemCoroutine(item));
    }
    
    private IEnumerator ScrollToItemCoroutine(TreeViewItem item)
    {
        // 레이아웃 업데이트 대기
        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return null;
        
        // 아이템의 위치 계산
        RectTransform itemRect = item.GetComponent<RectTransform>();
        RectTransform contentRect = scrollRect.content;
        
        // 콘텐츠 내에서의 아이템 상대 위치 계산
        float itemPos = itemRect.anchoredPosition.y;
        float contentHeight = contentRect.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        
        // 스크롤 위치 계산 (0이 상단, 1이 하단)
        float scrollPosition = itemPos / (contentHeight - viewportHeight);
        
        // 정규화된 위치로 변환하여 적용 (1이 상단, 0이 하단)
        scrollRect.normalizedPosition = new Vector2(0, 1 - scrollPosition);
    }
}