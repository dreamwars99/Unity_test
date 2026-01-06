using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject lobbyPanel;      // 로비 화면
    public GameObject backButton;      // 뒤로가기(홈) 버튼

    [Header("Games List")]
    // 게임 오브젝트들을 리스트(배열)로 관리
    // 인덱스: 0=Clicker, 1=Rain, 2=Tower, 3=Fly, 4=Stopwatch
    public GameObject[] games; 

    void Start()
    {
        GoToLobby(); // 시작하면 로비로
    }

    // 로비로 돌아가는 함수
    public void GoToLobby()
    {
        // 1. 모든 게임 끄기
        for (int i = 0; i < games.Length; i++)
        {
            games[i].SetActive(false);
        }

        // 2. 로비 켜기
        lobbyPanel.SetActive(true);
        
        // 3. 뒤로가기 버튼은 로비에선 안 보여야 함
        backButton.SetActive(false);
        
        // 시간 정상화 (혹시 멈춘 게임에서 나왔을까봐)
        Time.timeScale = 1.0f;
    }

    // 게임 실행 함수 (버튼에서 0, 1, 2... 번호를 넘겨줌)
    public void OpenGame(int index)
    {
        // 1. 로비 끄기
        lobbyPanel.SetActive(false);

        // 2. 해당 게임만 켜기
        // (배열 범위 안전장치)
        if (index >= 0 && index < games.Length)
        {
            games[index].SetActive(true);
        }

        // 3. 뒤로가기 버튼 켜기
        backButton.SetActive(true);
    }
}