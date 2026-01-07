using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class ArrowGameManager : MonoBehaviour
{
    // ==========================================
    // 1. Inspector 연결 변수
    // ==========================================
    [Header("Standard UI Connections")]
    public GameObject panelLobby;
    public GameObject panelThisGame;
    
    [Header("Score UI")]
    public TextMeshProUGUI txtCurrentScore; 
    public TextMeshProUGUI txtBestScore;    
    
    [Header("Game Elements")]
    public TextMeshProUGUI txtTarget;   
    public TextMeshProUGUI txtTime;     
    public TextMeshProUGUI txtFeedback; 

    [Header("Popups")]
    public GameObject popupGameOver;    
    public TextMeshProUGUI txtFinalScore; 

    [Header("Game Settings (Balance)")]
    public float initialTimeLimit = 3.0f; // [변경] 한 문제당 주어지는 초기 시간
    public float minTimeLimit = 0.5f;     // [추가] 최소 시간 제한
    public float timeReduction = 0.1f;    // [추가] 맞출 때마다 줄어드는 시간 양

    // ==========================================
    // 2. 내부 변수
    // ==========================================
    private int score = 0;
    private int bestScore = 0;
    private float currentTime;       
    private float currentMaxTime;    
    private bool isPlaying = false;
    private int targetDirection; 

    private float reactionStartTime; 
    
    private string[] arrowSymbols = { "UP", "DOWN", "LEFT", "RIGHT" };
    private Color[] arrowColors = { Color.red, Color.blue, Color.green, Color.yellow };

    // ==========================================
    // 3. 초기화
    // ==========================================
    void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore_Arrow", 0);
        UpdateScoreUI();
        GameStart();
    }

    void Update()
    {
        if (!isPlaying) return; 

        currentTime -= Time.deltaTime;
        txtTime.text = $"Time: {currentTime:F2}"; 

        if (currentTime <= 0)
        {
            GameOver(); 
        }

        CheckInput();
    }

    // ==========================================
    // 4. 게임 로직
    // ==========================================
    void GameStart()
    {
        score = 0;
        currentMaxTime = initialTimeLimit; 
        isPlaying = true;
        
        if(popupGameOver != null) popupGameOver.SetActive(false); 

        UpdateScoreUI();
        txtFeedback.text = "Start!";
        
        NextArrow();
    }

    void NextArrow()
    {
        targetDirection = Random.Range(0, 4);
        txtTarget.text = arrowSymbols[targetDirection];
        txtTarget.color = arrowColors[targetDirection];

        currentTime = currentMaxTime; 
        reactionStartTime = Time.time; 
    }

    void CheckInput()
    {
        if (!Input.anyKeyDown) return;

        bool isCorrect = false;

        if (Input.GetKeyDown(KeyCode.UpArrow) && targetDirection == 0) isCorrect = true;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && targetDirection == 1) isCorrect = true;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && targetDirection == 2) isCorrect = true;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && targetDirection == 3) isCorrect = true;
        
        if (isCorrect)
        {
            float reactionTime = Time.time - reactionStartTime;

            int addedScore = 100 + Mathf.RoundToInt(currentTime * 100);
            score += addedScore;

            txtFeedback.text = $"Perfect! ({reactionTime:F2}s)";
            txtFeedback.color = Color.green;

            currentMaxTime = Mathf.Max(minTimeLimit, currentMaxTime - timeReduction);

            NextArrow();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || 
                 Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            score -= 50;
            currentTime -= 0.5f; 

            txtFeedback.text = "Miss!";
            txtFeedback.color = Color.red;
        }
        
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if(txtCurrentScore != null) txtCurrentScore.text = $"{score}";
        if(txtBestScore != null) txtBestScore.text = $"Best: {bestScore}";
    }

    void GameOver()
    {
        isPlaying = false;
        txtTime.text = "0.00";
        
        // [수정] 팝업이랑 겹치지 않게 배경 글자들은 깔끔하게 지워버리기!
        txtTarget.text = ""; 
        txtFeedback.text = ""; 

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore_Arrow", bestScore);
            PlayerPrefs.Save(); 
        }

        if(popupGameOver != null) popupGameOver.SetActive(true);
        if(txtFinalScore != null) txtFinalScore.text = $"Final Score\n{score}";
    }

    // ==========================================
    // 5. 버튼 연결
    // ==========================================
    public void OnClick_Retry()
    {
        GameStart();
    }

    public void OnClick_BackToLobby()
    {
        if (panelThisGame != null) panelThisGame.SetActive(false);
        if (panelLobby != null) panelLobby.SetActive(true);
    }
}