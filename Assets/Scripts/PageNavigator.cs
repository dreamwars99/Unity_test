using UnityEngine;

public class PageNavigator : MonoBehaviour
{
    // 이동하고 싶은 목적지 화면 (Inspector에서 연결)
    public GameObject targetScreen; 
    
    // 현재 화면 (닫아야 할 경우 사용, 선택사항)
    public GameObject currentScreen;

    public void GoToPage()
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false); // 현재 화면 끄기
        }

        if (targetScreen != null)
        {
            targetScreen.SetActive(true); // 목표 화면 켜기
        }
    }
    
    // 뒤로가기용 (자기 자신을 끄기만 함)
    public void GoBack()
    {
        gameObject.SetActive(false); // 나 자신(현재 화면) 끄기
        // 이전 화면을 켜야 한다면 targetScreen에 이전 화면을 넣고 GoToPage를 쓰세요.
    }
}