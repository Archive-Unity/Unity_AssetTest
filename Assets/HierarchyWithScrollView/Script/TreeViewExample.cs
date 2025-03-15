using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 트리뷰 데이터 예시를 위한 클래스
public class TreeViewExample : MonoBehaviour
{
    [SerializeField] private TreeViewManager treeViewManager;

    private void Start()
    {
        PopulateTreeViewExample();
    }

    private void PopulateTreeViewExample()
    {
        // 루트 레벨 아이템 생성
        TreeViewItem item1 = treeViewManager.AddRootItem("아이템 1");
        TreeViewItem item2 = treeViewManager.AddRootItem("아이템 2");
        TreeViewItem item3 = treeViewManager.AddRootItem("아이템 3");

        // 자식 아이템 추가
        TreeViewItem item1Child1 = item1.CreateChildItem("아이템 1-1");
        TreeViewItem item1Child2 = item1.CreateChildItem("아이템 1-2");
        
        // 중첩된 자식 아이템 추가
        TreeViewItem item1Child1Sub1 = item1Child1.CreateChildItem("아이템 1-1-1");
        TreeViewItem item1Child1Sub2 = item1Child1.CreateChildItem("아이템 1-1-2");
        
        // 아이템 2에 자식 추가
        TreeViewItem item2Child1 = item2.CreateChildItem("아이템 2-1");
        TreeViewItem item2Child2 = item2.CreateChildItem("아이템 2-2");
        TreeViewItem item2Child3 = item2.CreateChildItem("아이템 2-3");
    }
}