using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 🔥 TextMeshPro 사용

public class MoleManager : MonoBehaviour
{
    // ==========================================
    // 1. 설정 변수 (Inspector에서 세팅)
    // ==========================================
    [Header("Game Settings")]
    public float limitTime = 30.0f; // 제한 시간
    public float baseSpawnInterval = 1.0f; // 기본 두더지 나오는 간격

    [Header("Connected Objects")]
    public List<Mole> moles; // 9마리 두더지 리스트

    [Header("UI Connections")]
    public TMP_Text txtScore;     // 현재 점수 텍스트
    public TMP_Text txtTime;      // 시간 텍스트
    public TMP_Text txtStage;     // [NEW] 몇 단계인지 보여줄 텍스트 (없으면 연결 안 해도 됨)
    public GameObject popupGameOver; // 게임 오버 팝업
    public TMP_Text txtFinalScore;   // 팝업 안에 뜰 최종 점수
    public TMP_Text txtBestScore;    // 최고 점수 텍스트

    // ==========================================
    // 2. 내부 변수 (Private)
    // ==========================================
    private float currentTime;
    private int currentScore;
    private bool isPlaying = false;
    
    // 🔥 스테이지 관리 변수
    private int currentStage = 1;
    private float currentInterval; // 계산된 실제 속도

    // 저장 키값
    private string keyBestScore = "BestScore_Mole"; 

    // ==========================================
    // 3. 생명주기
    // ==========================================
    void Start()
    {
        Time.timeScale = 1.0f; 

        // 저장된 최고 점수 표시
        UpdateBestScoreUI();

        // 두더지 세팅
        foreach (Mole mole in moles)
        {
            if(mole != null) mole.Setup(this);
        }

        // 1단계부터 시작!
        currentStage = 1;
        StartGame();
    }

    void Update()
    {
        if (!isPlaying) return; 

        // 시간 줄이기
        currentTime -= Time.deltaTime;
        
        // UI 갱신
        float displayTime = Mathf.Max(0, currentTime);
        if (txtTime != null)
            txtTime.text = $"Time: {displayTime:F1}";

        // 시간 종료 체크
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }
    }

    // ==========================================
    // 4. 게임 흐름 함수들 (Retry / Next Stage)
    // ==========================================
    
    // 게임을 실제로 시작하는 내부 함수
    void StartGame()
    {
        // 변수 리셋
        // (점수는 스테이지가 올라도 0부터 시작할지, 이어갈지 결정해야 하는데 
        // 보통 아케이드 게임은 스테이지마다 점수가 누적되거나 리셋됨. 
        // 여기서는 매 판 '새로운 도전' 느낌으로 리셋시킴. 누적하고 싶으면 이 줄을 삭제!)
        currentScore = 0; 
        
        currentTime = limitTime;
        isPlaying = true;
        
        // 🔥 [난이도 조절 핵심 로직]
        // 단계가 오를수록 0.1초씩 빨라짐 (최소 0.2초까지만)
        currentInterval = Mathf.Max(0.2f, baseSpawnInterval - ((currentStage - 1) * 0.1f));

        if (popupGameOver != null) 
            popupGameOver.SetActive(false);

        UpdateUI();

        // 기존 코루틴 정리 및 시작
        StopAllCoroutines(); 
        foreach (Mole mole in moles)
        {
            if(mole != null) mole.Hide();
        }
        StartCoroutine(SpawnRoutine());
    }

    // 🔥 [버튼 연결용] 리트라이 버튼 누르면 -> 다음 단계로 진화!
    public void RetryGame()
    {
        // 팝업 닫기
        if (popupGameOver != null) 
            popupGameOver.SetActive(false);
            
        // 단계 상승!
        currentStage++;

        // 게임 시작
        StartGame();
    }
    
    // 혹시 처음부터 다시 하고 싶을 때를 대비한 함수 (필요하면 버튼 연결)
    public void ResetToStageOne()
    {
        currentStage = 1;
        StartGame();
    }

    // ==========================================
    // 5. 두더지 소환 및 점수 로직
    // ==========================================

    IEnumerator SpawnRoutine()
    {
        int lastIndex = -1; 
        yield return new WaitForSeconds(0.5f);

        while (isPlaying)
        {
            if (moles.Count > 0)
            {
                int index = Random.Range(0, moles.Count);
                if (index == lastIndex && moles.Count > 1) index = Random.Range(0, moles.Count);
                lastIndex = index;

                if(moles[index] != null) 
                {
                    moles[index].PopUp();
                }
            }

            // 🔥 계산된 속도(currentInterval)만큼 대기
            yield return new WaitForSeconds(currentInterval);
        }
    }

    public void AddScore(int baseScore)
    {
        if (!isPlaying) return;

        // 🔥 [점수 뻥튀기] 단계만큼 곱하기! (1단계=1배, 5단계=5배)
        int finalScore = baseScore * currentStage;
        currentScore += finalScore;
        UpdateUI();
    }

    // 🔥 [NEW] 두더지를 놓쳤을 때 호출할 함수 (5단계 이상일 때만 감점)
    // ※ 주의: 이 함수는 두더지(Mole.cs)가 사라질 때 스스로 호출해줘야 작동함!
    public void OnMoleMissed()
    {
        if (!isPlaying) return;

        // 5단계 이상이면 감점!
        if (currentStage >= 5)
        {
            currentScore -= 50; // 50점 감점
            if (currentScore < 0) currentScore = 0; // 음수 방지
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (txtScore != null)
            txtScore.text = $"Score: {currentScore}";
            
        // 단계 표시 (UI 연결되어 있다면)
        if (txtStage != null)
            txtStage.text = $"STAGE {currentStage}";
    }
    
    void UpdateBestScoreUI()
    {
        int best = PlayerPrefs.GetInt(keyBestScore, 0);
        if (txtBestScore != null)
        {
            txtBestScore.text = $"Best: {best}";
        }
    }

    void GameOver()
    {
        isPlaying = false;
        StopAllCoroutines();
        
        foreach (Mole mole in moles)
        {
            if(mole != null) mole.Hide();
        }

        // 최고 기록 저장
        int bestScore = PlayerPrefs.GetInt(keyBestScore, 0);
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt(keyBestScore, bestScore);
            PlayerPrefs.Save();
            UpdateBestScoreUI();
        }

        // 팝업 띄우기
        if (popupGameOver != null)
        {
            popupGameOver.SetActive(true);
            if (txtFinalScore != null)
            {
                // 다음 단계 예고 멘트 추가
                txtFinalScore.text = $"Score: {currentScore}\nNext: STAGE {currentStage + 1}";
            }
        }
    }
}