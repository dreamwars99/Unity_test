// 파일명: StartManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class StartManager : MonoBehaviour
{
    // 🐍 Python: def on_start_click():
    // '시작하기' 버튼이 눌리면 실행될 함수
    public void OnStartClick()
    {
        Debug.Log("학습 화면으로 이동합니다!");
        // 'DuoMain'이라는 이름의 씬을 불러와라 (기존 화면은 덮어씌워짐)
        SceneManager.LoadScene("DuoMain");
    }

    // '로그인' 버튼용 (나중에 만들 로그인 화면을 위해 미리 준비)
    public void OnLoginClick()
    {
        Debug.Log("로그인 화면은 아직 준비 중...");
        // 나중에 "DuoLogin" 같은 걸로 바꾸면 돼
    }
}