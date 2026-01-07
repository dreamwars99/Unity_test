using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject lobbyPanel;      // 로비 화면 (Lobby_Main)
    public GameObject backButton;      // 뒤로가기(홈) 버튼 (Btn_BackToLobby)

    [Header("Games List")]
    // 게임 오브젝트들을 리스트(배열)로 관리
    // 인덱스: 0=Clicker, 1=Rain, 2=Tower, 3=Fly, 4=Stopwatch, 5=ArrowBeat
    public GameObject[] games; 

    void Start()
    {
        GoToLobby(); // 시작하면 로비로
    }

    // 로비로 돌아가는 함수 (Btn_BackToLobby 버튼에 연결)
    public void GoToLobby()
    {
        // 1. 모든 게임 끄기
        for (int i = 0; i < games.Length; i++)
        {
            if (games[i] != null) 
                games[i].SetActive(false);
        }

        // 2. 로비 켜기
        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        
        // 3. 뒤로가기 버튼은 로비에선 안 보여야 함 (핵심)
        if (backButton != null) backButton.SetActive(false);
        
        // 4. 시간 정상화 (중요: 정지된 게임에서 탈출했을 때 대비)
        Time.timeScale = 1.0f;
    }

    // 게임 실행 함수 (각 게임 버튼에서 0, 1, 2... 번호를 인자로 전달)
    public void OpenGame(int index)
    {
        // 1. 로비 끄기
        if (lobbyPanel != null) lobbyPanel.SetActive(false);

        // 2. 해당 게임만 켜기
        if (index >= 0 && index < games.Length)
        {
            if (games[index] != null)
                games[index].SetActive(true);
        }

        // 3. 뒤로가기 버튼 켜기 (이제 게임 중이니까 보여야 함)
        if (backButton != null) backButton.SetActive(true);
    }
}