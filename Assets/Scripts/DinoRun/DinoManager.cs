using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 텍스트 UI 쓰려면 필수!

public class DinoManager : MonoBehaviour
{
    public static DinoManager instance; // 어디서든 부를 수 있게 (싱글톤)

    [Header("--- UI ---")]
    public TextMeshProUGUI txtScore;     // 현재 점수 텍스트
    public TextMeshProUGUI txtBestScore; // 최고 점수 텍스트 (New!)
    public GameObject popupGameOver;     // 게임오버 팝업 (재시작 버튼 포함)

    [Header("--- Player & Settings ---")]
    public GameObject player;           // 플레이어 오브젝트 (위치 리셋용)
    private Vector3 playerStartPos;     // 게임 시작 시 플레이어 위치 저장

    [Header("--- Game State ---")]
    public bool isGameOver = false;
    public float score = 0f;

    // 최고 점수 저장을 위한 키값 (오타 방지용 상수)
    private const string BEST_SCORE_KEY = "DinoBestScore";

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // 1. 게임 시작 시 상태 초기화
        isGameOver = false;
        score = 0f;
        Time.timeScale = 1.0f; // 시간 정상 흐름

        if (popupGameOver != null) popupGameOver.SetActive(false);

        // 2. 플레이어의 시작 위치를 기억해둠
        if (player != null)
        {
            playerStartPos = player.transform.localPosition;
        }

        // 3. 최고 점수 불러오기 및 UI 표시 (New!)
        UpdateBestScoreUI();
    }

    void Update()
    {
        if (isGameOver) return; // 게임 오버면 점수 증가 중단

        // 1. 점수 증가 (속도 상향: 1초에 100점! 테스트하기 좋게 빠름)
        score += Time.deltaTime * 100f;
        
        // 2. UI 갱신
        if (txtScore != null)
        {
            txtScore.text = "Score: " + (int)score;
        }
    }

    // 장애물에 부딪혔을 때 Player가 이 함수를 호출할 거야
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // 시간 정지

        // 4. 최고 점수 갱신 및 저장 로직 (New!)
        float currentBest = PlayerPrefs.GetFloat(BEST_SCORE_KEY, 0f);
        if (score > currentBest)
        {
            // 신기록 달성! 저장하고 UI 갱신
            PlayerPrefs.SetFloat(BEST_SCORE_KEY, score);
            PlayerPrefs.Save(); // 즉시 저장
            UpdateBestScoreUI();
        }

        if (popupGameOver != null)
            popupGameOver.SetActive(true); // 팝업 띄우기
    }

    public void RetryGame()
    {
        // 1. 장애물 삭제
        GameObject[] cactusList = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject c in cactusList) Destroy(c);

        // 2. 상태 리셋
        isGameOver = false;
        score = 0f;
        Time.timeScale = 1.0f;
        popupGameOver.SetActive(false);

        // 3. 플레이어 위치 & 물리 리셋
        if (player != null)
        {
            player.transform.localPosition = playerStartPos;
            player.SetActive(true);
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

        // 재시작 시에도 최고 점수 UI 최신화
        UpdateBestScoreUI();
    }

    // 최고 점수 UI 업데이트 함수 (코드 중복 방지)
    void UpdateBestScoreUI()
    {
        if (txtBestScore != null)
        {
            float best = PlayerPrefs.GetFloat(BEST_SCORE_KEY, 0f);
            txtBestScore.text = "Best: " + (int)best;
        }
    }
}