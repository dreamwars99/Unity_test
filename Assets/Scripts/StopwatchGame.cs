using UnityEngine;
using TMPro;

public class StopwatchGame : MonoBehaviour
{
    [Header("UI Groups")]
    public GameObject menuGroup; // 모드 선택 화면
    public GameObject gameGroup; // 게임 플레이 화면

    [Header("Game UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI buttonText;

    [Header("Settings")]
    public float targetTime = 10.00f;
    public float tolerance = 0.05f;

    // 내부 변수
    private float currentTime = 0f;
    private bool isRunning = false;
    private bool isFinished = false;
    
    // [신규] 블라인드 모드인지 체크하는 변수
    private bool isBlindMode = false;

    void Start()
    {
        ShowMenu(); // 시작하면 메뉴부터 보여줌
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;

            // [블라인드 모드 로직 수정]
            if (isBlindMode && currentTime > 3.0f)
            {
                // 숫자를 랜덤으로 막 돌려서 '작동 중'임을 보여줌!
                // (진짜 시간은 가려지지만, 뭔가 급박하게 돌아가는 느낌)
                int r1 = Random.Range(0, 100); // 앞자리 00~99
                int r2 = Random.Range(0, 100); // 뒷자리 00~99
                
                // <color=#888888> 태그를 써서 약간 회색으로 표시하면 더 '가려진' 느낌 남
                timerText.text = $"<color=#AAAAAA>{r1:00}.{r2:00}</color>";
            }
            else
            {
                // 평소에는 진짜 시간 표시
                timerText.text = currentTime.ToString("F2");
            }
        }
    }

    // --- [메뉴 관련 함수] ---
    public void ShowMenu()
    {
        menuGroup.SetActive(true);
        gameGroup.SetActive(false);
    }

    // 일반 모드 버튼 연결용
    public void SelectNormalMode()
    {
        isBlindMode = false;
        StartGame();
    }

    // 블라인드 모드 버튼 연결용
    public void SelectBlindMode()
    {
        isBlindMode = true;
        StartGame();
    }

    // --- [게임 로직] ---
    void StartGame()
    {
        menuGroup.SetActive(false);
        gameGroup.SetActive(true);
        
        // 데이터 초기화
        currentTime = 0f;
        isRunning = false; // 일단 대기 (START 눌러야 시작)
        isFinished = false;

        timerText.text = "00.00";
        resultText.text = isBlindMode ? "Mode: BLIND" : "Mode: NORMAL";
        resultText.color = Color.white;
        buttonText.text = "START";
    }

    // 게임 화면의 큰 버튼 (START / STOP / RETRY)
    public void OnActionClick()
    {
        if (!isRunning && !isFinished)
        {
            // 1. 시작
            isRunning = true;
            resultText.text = "Go to 10.00!";
            buttonText.text = "STOP!";
        }
        else if (isRunning)
        {
            // 2. 멈춤 (결과 확인)
            isRunning = false;
            isFinished = true;
            CheckResult();
            buttonText.text = "TO MENU"; // 다시 메뉴로
        }
        else if (isFinished)
        {
            // 3. 메뉴로 돌아가기
            ShowMenu();
        }
    }

    void CheckResult()
    {
        // 멈췄을 때는 실제 시간을 딱 보여줘야 함! (물음표 해제)
        timerText.text = currentTime.ToString("F2");

        float diff = Mathf.Abs(currentTime - targetTime);

        if (diff <= tolerance)
        {
            resultText.text = "PERFECT! 🎉";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "FAILED... 😭";
            resultText.color = Color.red;
        }
    }
}