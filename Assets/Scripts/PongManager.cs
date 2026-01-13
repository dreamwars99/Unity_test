using UnityEngine;
using TMPro;

public class PongManager : MonoBehaviour
{
    public static PongManager instance;

    [Header("Objects")]
    public PongBall ballScript;
    
    [Header("UI")]
    public TextMeshProUGUI txtPlayerScore; 
    public TextMeshProUGUI txtEnemyScore;  
    public TextMeshProUGUI txtBestScore;   
    public TextMeshProUGUI txtStage;       
    public GameObject gameOverPopup;       
    public TextMeshProUGUI txtFinalResult; 

    [Header("Data")]
    public int playerScore = 0;
    public int enemyScore = 0;
    public int bestScore = 0;
    public int stage = 1;

    // 게임 결과를 저장하는 변수 (승리여부)
    private bool isStageClear = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("PongBestScore", 0);
        StartGame();
    }

    public void StartGame()
    {
        // 승리해서 재시작한 게 아니라면(완전 초기화라면) 점수 리셋
        if (!isStageClear)
        {
            playerScore = 0;
            enemyScore = 0;
            stage = 1;
        }
        else
        {
            // 스테이지 클리어 상태라면 다음 스테이지 세팅
            isStageClear = false; // 플래그 초기화
        }

        UpdateUI();
        
        if (gameOverPopup != null) gameOverPopup.SetActive(false);
        Time.timeScale = 1.0f;
        
        if (ballScript == null) return;

        CancelInvoke("LaunchBall");
        Invoke("LaunchBall", 1.0f);
    }

    void LaunchBall()
    {
        if (ballScript == null) return;
        ballScript.gameObject.SetActive(true);
        ballScript.Launch();
    }

    // 골인 발생 (PongGoal에서 호출)
    public void OnGoal(bool isPlayerGoal)
    {
        if (isPlayerGoal)
        {
            // [수정] 적 골대에 넣으면 승리! -> 스테이지 클리어 처리
            playerScore++;
            OnStageClear();
        }
        else
        {
            // 내 골대에 들어오면 패배 -> 게임 오버
            enemyScore++;
            OnGameOver();
        }
        UpdateUI();
    }

    // 패들에 닿았을 때 점수 (PongBall에서 호출)
    public void AddScore(int amount)
    {
        playerScore += amount;
        UpdateUI();
    }

    // [신규] 스테이지 클리어 (승리) 처리
    public void OnStageClear()
    {
        isStageClear = true; // 클리어 상태 기록
        stage++; // 스테이지 상승
        
        Time.timeScale = 0; // 게임 정지
        
        // 최고 점수 갱신
        if (playerScore > bestScore)
        {
            bestScore = playerScore;
            PlayerPrefs.SetInt("PongBestScore", bestScore);
            PlayerPrefs.Save();
        }

        // 승리 메시지 표시
        if (txtFinalResult != null) 
            txtFinalResult.text = $"VICTORY!\nNext Stage: {stage}";
            
        if (gameOverPopup != null) gameOverPopup.SetActive(true);
    }

    public void OnGameOver()
    {
        isStageClear = false; // 패배 상태
        Time.timeScale = 0;
        
        if (playerScore > bestScore)
        {
            bestScore = playerScore;
            PlayerPrefs.SetInt("PongBestScore", bestScore);
            PlayerPrefs.Save();
        }

        if (txtFinalResult != null) 
            txtFinalResult.text = $"GAME OVER\nScore: {playerScore}";
            
        if (gameOverPopup != null) gameOverPopup.SetActive(true);
    }

    // 팝업의 'Retry' 버튼이 누르는 함수
    public void RetryGame()
    {
        if (ballScript != null) ballScript.gameObject.SetActive(false);
        
        // StartGame 내부에서 isStageClear 플래그를 확인하여
        // 승리했으면 스테이지를 유지하고, 졌으면 초기화합니다.
        StartGame();
    }

    void UpdateUI()
    {
        if (txtPlayerScore != null) txtPlayerScore.text = $"{playerScore}";
        if (txtEnemyScore != null) txtEnemyScore.text = $"{enemyScore}";
        if (txtBestScore != null) txtBestScore.text = $"BEST: {bestScore}";
        if (txtStage != null) txtStage.text = $"STAGE {stage}";
    }
}