// 파일명: MainTabManager.cs
using UnityEngine;
using UnityEngine.UI;

public class MainTabManager : MonoBehaviour
{
    // 🐍 Python: pages = [] 
    // 관리할 페이지(패널)들을 담을 리스트
    public GameObject[] panels;

    void Start()
    {
        // 시작할 때 첫 번째 페이지(학습)만 켜고 나머진 끈다
        OnTabClick(0);
    }

    // 🐍 Python: def on_tab_click(index):
    // 버튼이 클릭되면 호출할 함수 (0번, 1번, 2번...)
    public void OnTabClick(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index)
            {
                panels[i].SetActive(true); // 선택된 녀석만 켜기
            }
            else
            {
                panels[i].SetActive(false); // 나머지는 끄기
            }
        }
    }
}