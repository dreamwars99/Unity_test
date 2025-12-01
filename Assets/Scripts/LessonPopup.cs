// 파일명: LessonPopup.cs
using UnityEngine;

public class LessonPopup : MonoBehaviour
{
    public GameObject popupPanel; // 팝업창 전체 (검은 배경 포함)

    // 🐍 Python: def open_popup():
    // 아이콘을 누르면 실행될 함수
    public void OpenPopup()
    {
        popupPanel.SetActive(true); // 팝업 켜기
    }

    // 🐍 Python: def close_popup():
    // 팝업 닫기 버튼이나 배경을 누르면 실행
    public void ClosePopup()
    {
        popupPanel.SetActive(false); // 팝업 끄기
    }

    // "학습 시작" 버튼 누르면 실행
    public void StartLesson()
    {
        Debug.Log("학습 시작!");
        // 나중에 여기에 실제 게임 화면으로 넘어가는 코드 넣으면 됨
        ClosePopup();
    }
}