using UnityEngine;
using TMPro;

public class FlyManager : MonoBehaviour
{
    public static FlyManager instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPopup;

    [Header("Game Data")]
    public int score = 0;
    public int highScore = 0;
    public bool isGameOver = false;

    [Header("Difficulty")]
    public float currentPipeSpeed = 500f; // [신규] 전체 게임 속도

    void Awake() { instance = this; }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("FlyBest", 0);
        GameStart();
    }

    public void GameStart()
    {
        score = 0;
        isGameOver = false;
        
        // [신규] 시작 속도 초기화 (기본 500)
        currentPipeSpeed = 500f;
        
        Time.timeScale = 1;

        if(gameOverPopup != null) gameOverPopup.SetActive(false);
        UpdateUI();

        // 파이프 청소
        FlyPipe[] pipes = FindObjectsOfType<FlyPipe>();
        foreach(var p in pipes) Destroy(p.gameObject);

        // 플레이어 위치 초기화
        GameObject player = GameObject.Find("Player");
        if(player != null) 
        {
            // [수정] 네가 원한 -550 위치
            player.transform.localPosition = new Vector3(-550, 0, 0);
            
            // 떨어지던 힘(Rigidbody 등)이 없게 초기화하고 싶다면
            // FlyPlayer 스크립트에서 start시 velocity를 0으로 맞추는 것도 좋음
        }
    }

    public void AddScore()
    {
        if (isGameOver) return;
        score++;
        
        // [신규] 점수 얻을 때마다 속도 빨라짐 (최대 1000까지)
        currentPipeSpeed += 20f;
        if (currentPipeSpeed > 1000f) currentPipeSpeed = 1000f;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("FlyBest", highScore);
        }
        UpdateUI();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        Debug.Log("게임 오버!");
        Time.timeScale = 0;

        if(finalScoreText != null) finalScoreText.text = "Final Score: " + score;
        if(gameOverPopup != null) gameOverPopup.SetActive(true);
    }

    void UpdateUI()
    {
        if(scoreText != null) scoreText.text = score.ToString();
        if(highScoreText != null) highScoreText.text = "Best: " + highScore;
    }
}