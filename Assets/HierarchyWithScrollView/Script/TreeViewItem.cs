using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreeViewItem : MonoBehaviour
{
    [SerializeField] private Button itemButton;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Transform childContainer;
    [SerializeField] private Image expandIcon;
    [SerializeField] private Sprite expandedSprite;
    [SerializeField] private Sprite collapsedSprite;
    
    // 높이 조정을 위한 컴포넌트
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private float collapsedHeight = 40f; // 접혔을 때 높이
    
    private bool isExpanded = false;
    private RectTransform rectTransform;
    private VerticalLayoutGroup childLayoutGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // 필요한 컴포넌트가 없으면 추가
        if (layoutElement == null)
        {
            layoutElement = GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = gameObject.AddComponent<LayoutElement>();
            }
        }
        
        // 자식 컨테이너의 레이아웃 그룹 참조
        childLayoutGroup = childContainer.GetComponent<VerticalLayoutGroup>();
        if (childLayoutGroup == null)
        {
            childLayoutGroup = childContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        }
        
        // 자식 컨테이너의 ContentSizeFitter 확인
        ContentSizeFitter fitter = childContainer.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = childContainer.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    public void Initialize(string text)
    {
        itemText.text = text;
        itemButton.onClick.AddListener(ToggleExpand);
        childContainer.gameObject.SetActive(false);
        expandIcon.sprite = collapsedSprite;
        
        // 초기 높이 설정
        layoutElement.minHeight = collapsedHeight;
        layoutElement.preferredHeight = collapsedHeight;
    }

    public void AddChild(TreeViewItem childItem)
    {
        childItem.transform.SetParent(childContainer);
        
        // 자식이 추가되면 확장 아이콘 활성화
        if (expandIcon != null)
        {
            expandIcon.enabled = true;
        }
    }

    private void ToggleExpand()
    {
        isExpanded = !isExpanded;
        childContainer.gameObject.SetActive(isExpanded);
        expandIcon.sprite = isExpanded ? expandedSprite : collapsedSprite;
        
        // 높이 조정
        StartCoroutine(AdjustHeight());
    }
    
    private IEnumerator AdjustHeight()
    {
        // UI 업데이트를 위한 지연
        yield return null;
        
        if (isExpanded)
        {
            // 확장 시 자식 컨테이너의 높이에 맞게 조정
            Canvas.ForceUpdateCanvases();
            float childrenHeight = CalculateChildrenHeight();
            
            // 자신의 기본 높이 + 자식 컨테이너의 높이
            float newHeight = collapsedHeight + childrenHeight;
            layoutElement.minHeight = newHeight;
            layoutElement.preferredHeight = newHeight;
        }
        else
        {
            // 접힌 상태로 돌아갈 때 원래 높이로 복원
            layoutElement.minHeight = collapsedHeight;
            layoutElement.preferredHeight = collapsedHeight;
        }
        
        // 부모 레이아웃 갱신
        yield return null;
        UpdateParentLayouts();
    }
    
    // 자식 요소들의 전체 높이 계산
    private float CalculateChildrenHeight()
    {
        if (!childContainer.gameObject.activeSelf)
            return 0f;
            
        float totalHeight = 0;
        
        // 자식 컨테이너의 패딩 추가
        if (childLayoutGroup != null)
        {
            totalHeight += childLayoutGroup.padding.top + childLayoutGroup.padding.bottom;
        }
        
        // 각 자식 아이템의 높이 합산
        foreach (RectTransform child in childContainer)
        {
            // 활성화된 자식만 계산
            if (child.gameObject.activeSelf)
            {
                // LayoutElement가 있으면 해당 높이 사용
                LayoutElement childElement = child.GetComponent<LayoutElement>();
                if (childElement != null)
                {
                    totalHeight += childElement.preferredHeight;
                }
                else
                {
                    // 없으면 RectTransform 높이 사용
                    totalHeight += child.rect.height;
                }
                
                // 레이아웃 그룹의 간격 추가
                if (childLayoutGroup != null && child != childContainer.GetChild(childContainer.childCount - 1))
                {
                    totalHeight += childLayoutGroup.spacing;
                }
            }
        }
        
        return totalHeight;
    }
    
    // 부모 레이아웃 그룹들 갱신
    private void UpdateParentLayouts()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            // 레이아웃 그룹이나 ContentSizeFitter가 있는 오브젝트 찾기
            LayoutGroup layout = current.GetComponent<LayoutGroup>();
            ContentSizeFitter fitter = current.GetComponent<ContentSizeFitter>();
            
            if (layout != null || fitter != null)
            {
                RectTransform rt = current.GetComponent<RectTransform>();
                if (rt != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                }
            }
            
            // 부모 TreeViewItem이 있으면 높이 재조정
            TreeViewItem parentItem = current.GetComponent<TreeViewItem>();
            if (parentItem != null)
            {
                parentItem.StartCoroutine(parentItem.AdjustHeight());
            }
            
            current = current.parent;
        }
    }
    
    // TreeView 아이템 확장 상태 설정
    public void SetExpanded(bool expanded)
    {
        if (isExpanded != expanded)
        {
            isExpanded = expanded;
            childContainer.gameObject.SetActive(isExpanded);
            expandIcon.sprite = isExpanded ? expandedSprite : collapsedSprite;
            
            // 높이 조정
            StartCoroutine(AdjustHeight());
        }
    }
    
    // 자식 아이템 추가를 위한 헬퍼 메서드
    public TreeViewItem CreateChildItem(string text)
    {
        TreeViewItem newItem = TreeViewManager.Instance.CreateTreeViewItem(text);
        AddChild(newItem);
        return newItem;
    }
    
    // 현재 확장 상태 반환
    public bool IsExpanded()
    {
        return isExpanded;
    }
}