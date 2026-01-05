using UnityEngine;
using TMPro;

public class DodgeManager : MonoBehaviour
{
    public static DodgeManager instance;

    [Header("UI")]
    public TextMeshProUGUI gameScoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText; // [신규] 최고 기록 UI 연결
    public GameObject gameOverPopup;

    [Header("Game Data")]
    public int score = 0;
    public int highScore = 0; // [신규] 최고 기록 변수
    public bool isGameOver = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // [신규] 게임 켜자마자 저장된 기록 불러오기
        // 주의: 클릭커 게임과 섞이지 않게 키 이름을 "DodgeBest"로 다르게 함!
        highScore = PlayerPrefs.GetInt("DodgeBest", 0);
        
        GameStart();
    }

    public void GameStart()
    {
        score = 0;
        isGameOver = false;
        Time.timeScale = 1;
        
        gameOverPopup.SetActive(false);
        UpdateScoreUI(); // 점수판 갱신

        // ... (비 청소 및 플레이어 위치 초기화 코드는 그대로) ...
        RainMovement[] rains = FindObjectsOfType<RainMovement>();
        foreach (var rain in rains) Destroy(rain.gameObject);
        
        GameObject player = GameObject.Find("Player");
        if(player != null) player.transform.localPosition = new Vector3(0, -1200, 0);
    }

    public void AddScore()
    {
        if (isGameOver) return;

        score += 1;
        
        // [신규] 신기록 달성 체크!
        if (score > highScore)
        {
            highScore = score;
            // 즉시 저장 (신기록은 소중하니까)
            PlayerPrefs.SetInt("DodgeBest", highScore);
        }
        
        UpdateScoreUI();
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0;
        
        finalScoreText.text = "Final Score: " + score;
        gameOverPopup.SetActive(true);
    }

    void UpdateScoreUI()
    {
        gameScoreText.text = "Score: " + score;
        
        // [신규] 최고 기록 UI도 같이 갱신
        highScoreText.text = "🏆 Best: " + highScore;
    }
}