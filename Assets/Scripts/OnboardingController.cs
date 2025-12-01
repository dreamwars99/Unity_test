using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파일명: OnboardingController.cs
public class OnboardingController : MonoBehaviour
{
    // 🐍 Python: pages = [page1, page2, ...] 리스트라고 생각하면 돼
    public List<GameObject> pages; 
    
    private int currentPageIndex = 0;

    void Start()
    {
        // 시작할 때 첫 페이지만 켜고 나머지 다 끄기
        UpdateUI();
    }

    // 버튼이랑 연결할 함수
    public void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++; // 인덱스 +1 (다음 장으로)
            UpdateUI();
        }
        else
        {
            Debug.Log("마지막 페이지입니다! (나중에 메인 씬 로드)");
        }
    }
    // 뒤로가기 버튼이랑 연결할 함수
    public void PreviousPage()
    {
        // 0번 페이지(스플래시)에서는 더 뒤로 갈 수 없으니 체크
        if (currentPageIndex > 0)
        {
            currentPageIndex--; // 인덱스 -1 (이전 장으로)
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        // 🐍 Python: for i, page in enumerate(pages): 
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == currentPageIndex)
                pages[i].SetActive(true);  // 현재 페이지만 켜기
            else
                pages[i].SetActive(false); // 나머지 끄기
        }
    }
}