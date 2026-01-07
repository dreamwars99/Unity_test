using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnakeGame : MonoBehaviour
{
    // ==========================================
    // 1. 설정 변수 (Inspector)
    // ==========================================
    [Header("Game Settings")]
    public float moveInterval = 0.1f;  // 뱀 이동 속도 (초 단위 Tick)
    public int stepSize = 40;          // 한 칸 크기 (BodyPart 크기와 맞춰야 함)
    public int gridLimitXY = 400;      // 게임판 반경 (800x800이면 중심에서 400)

    [Header("Prefabs")]
    public GameObject bodyPrefab;      // 뱀 몸통 프리팹 (초록색 네모)
    public GameObject foodPrefab;      // 먹이 프리팹 (빨간색 네모)
    public Transform gameArea;         // 게임이 진행될 부모 객체 (GameArea)

    [Header("UI Connections")]
    public TMP_Text txtScore;
    public TMP_Text txtBestScore;
    public GameObject popupGameOver;
    public TMP_Text txtFinalScore;

    // ==========================================
    // 2. 내부 변수 (State)
    // ==========================================
    // 파이썬의 list와 같아. 뱀의 모든 부위를 담는 리스트.
    // tail[0]은 머리, tail[1]부터 몸통.
    private List<RectTransform> snakeBody = new List<RectTransform>();
    
    private RectTransform food;        // 현재 생성된 먹이
    private Vector2 direction;         // 현재 이동 방향 (Vector2.up, down, left, right)
    private bool isPlaying = false;
    private int currentScore = 0;
    
    // 키 입력 중복 방지 (한 틱에 방향 두 번 바꾸기 금지)
    private bool hasMovedThisTick = false; 

    private string keyBestScore = "BestScore_Snake";

    // ==========================================
    // 3. 초기화 & 게임 루프
    // ==========================================
    void Start()
    {
        // 안전장치: 프리팹 연결 확인
        if (bodyPrefab == null || foodPrefab == null || gameArea == null)
        {
            Debug.LogError("❌ [SnakeGame] 프리팹이나 GameArea가 연결되지 않았습니다! Inspector를 확인해주세요.");
            return;
        }

        // 첫 실행 시 기존 기록 UI 갱신
        UpdateBestScoreUI();
        
        // 🔥 자동 시작 추가! (이게 없어서 안 떴던 거야)
        StartGame();
    }

    void Update()
    {
        // 게임 중이 아니면 입력 무시
        if (!isPlaying) return;

        HandleInput();
    }

    // ==========================================
    // 4. 게임 로직 (Start / Over / Move)
    // ==========================================
    public void StartGame()
    {
        // 1. 상태 초기화
        currentScore = 0;
        isPlaying = true;
        hasMovedThisTick = false;
        direction = Vector2.right; // 기본 오른쪽 이동

        UpdateUI();
        if (popupGameOver != null) popupGameOver.SetActive(false);

        // 2. 기존 뱀 & 먹이 청소 (리스트 순회하며 파괴)
        // 리스트를 복사해서 순회하거나, 안전하게 제거
        foreach (var part in snakeBody)
        {
            if(part != null) Destroy(part.gameObject);
        }
        snakeBody.Clear();

        if (food != null) Destroy(food.gameObject);

        // 3. 머리 생성 (Index 0)
        CreateBodyPart(Vector2.zero); 

        // 4. 첫 먹이 생성
        SpawnFood();

        // 5. 게임 루프 시작 (코루틴 = 파이썬의 while loop + sleep)
        StopAllCoroutines();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(moveInterval); // 0.1초 대기
            MoveSnake();
            hasMovedThisTick = false; // 다음 틱 입력 허용
        }
    }

    // 🔥 핵심 로직: 뱀 이동
    void MoveSnake()
    {
        if (snakeBody.Count == 0) return;

        // 1. 머리가 이동할 '예상 좌표' 계산
        // RectTransform의 anchoredPosition을 사용 (UI 좌표계)
        Vector2 currentHeadPos = snakeBody[0].anchoredPosition;
        Vector2 nextPos = currentHeadPos + (direction * stepSize);

        // 2. 충돌 체크 (벽 or 내 몸)
        if (CheckCollision(nextPos))
        {
            GameOver();
            return;
        }

        // 3. 이동 처리 (꼬리부터 머리 방향으로 당겨오기)
        // 마지막 꼬리를 머리 위치로 옮기는 게 아니라,
        // n번 꼬리가 n-1번 위치로 이동하는 식 (List 역순 순회)
        for (int i = snakeBody.Count - 1; i > 0; i--)
        {
            snakeBody[i].anchoredPosition = snakeBody[i - 1].anchoredPosition;
        }

        // 4. 머리 이동
        snakeBody[0].anchoredPosition = nextPos;

        // 5. 먹이 먹었나?
        // UI 좌표계는 float 오차가 있을 수 있어서 Vector2.Distance로 체크 (< 1.0f)
        if (food != null && Vector2.Distance(nextPos, food.anchoredPosition) < 1.0f)
        {
            EatFood();
        }
    }

    // ==========================================
    // 5. 헬퍼 함수들 (Input, Spawn, Collision)
    // ==========================================
    void HandleInput()
    {
        // 이번 틱에 이미 방향을 바꿨다면 무시 (급격한 180도 회전 방지)
        if (hasMovedThisTick) return;

        // 파이썬의 if-elif 구조와 동일
        // 현재 방향의 반대 방향으로는 못 감 (오른쪽 가는데 왼쪽 키 누르면 죽음 방지)
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down)
        {
            direction = Vector2.up;
            hasMovedThisTick = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up)
        {
            direction = Vector2.down;
            hasMovedThisTick = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right)
        {
            direction = Vector2.left;
            hasMovedThisTick = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left)
        {
            direction = Vector2.right;
            hasMovedThisTick = true;
        }
    }

    void CreateBodyPart(Vector2 pos)
    {
        // Prefab 생성 -> GameArea의 자식으로 설정
        GameObject obj = Instantiate(bodyPrefab, gameArea);
        RectTransform rect = obj.GetComponent<RectTransform>();
        
        // 위치 잡기 (Anchor가 Middle-Center라고 가정)
        rect.anchoredPosition = pos;
        
        // 리스트에 추가 (append)
        snakeBody.Add(rect);
    }

    void SpawnFood()
    {
        // 그리드에 맞춰 랜덤 좌표 생성
        // -360 ~ +360 사이에서 40단위로 끊어짐
        // gridLimitXY(400) - stepSize(40) = 360 (안전 여백)
        
        int maxStep = (gridLimitXY / stepSize) - 1; // 10 - 1 = 9칸
        int x = Random.Range(-maxStep, maxStep + 1);
        int y = Random.Range(-maxStep, maxStep + 1);

        Vector2 spawnPos = new Vector2(x * stepSize, y * stepSize);

        // 혹시 뱀 몸통 위에 생겼나? (재귀 호출로 다시 뽑기)
        foreach (var body in snakeBody)
        {
            if (Vector2.Distance(body.anchoredPosition, spawnPos) < 1.0f)
            {
                SpawnFood(); // 다시!
                return;
            }
        }

        if (food == null)
        {
            GameObject obj = Instantiate(foodPrefab, gameArea);
            food = obj.GetComponent<RectTransform>();
        }
        
        food.anchoredPosition = spawnPos;
    }

    void EatFood()
    {
        currentScore += 10;
        UpdateUI();
        
        // 꼬리 추가 (현재 마지막 꼬리 위치에 생성 -> 다음 틱에 펼쳐짐)
        Vector2 lastPos = snakeBody[snakeBody.Count - 1].anchoredPosition;
        CreateBodyPart(lastPos);

        // 새 먹이
        SpawnFood();
    }

    bool CheckCollision(Vector2 targetPos)
    {
        // 1. 벽 충돌 (범위 밖인가?)
        if (Mathf.Abs(targetPos.x) >= gridLimitXY || Mathf.Abs(targetPos.y) >= gridLimitXY)
        {
            return true;
        }

        // 2. 자기 몸 충돌
        // 머리(0번)는 제외하고 1번부터 검사
        for (int i = 1; i < snakeBody.Count; i++)
        {
            if (Vector2.Distance(targetPos, snakeBody[i].anchoredPosition) < 1.0f)
            {
                return true;
            }
        }

        return false;
    }

    // ==========================================
    // 6. UI 및 종료 처리
    // ==========================================
    void UpdateUI()
    {
        if (txtScore != null) txtScore.text = $"Score: {currentScore}";
    }
    
    void UpdateBestScoreUI()
    {
        int best = PlayerPrefs.GetInt(keyBestScore, 0);
        if (txtBestScore != null) txtBestScore.text = $"Best: {best}";
    }

    void GameOver()
    {
        isPlaying = false;
        StopAllCoroutines();

        // 최고 점수 갱신
        int best = PlayerPrefs.GetInt(keyBestScore, 0);
        if (currentScore > best)
        {
            PlayerPrefs.SetInt(keyBestScore, currentScore);
            PlayerPrefs.Save();
            UpdateBestScoreUI();
        }

        if (popupGameOver != null)
        {
            popupGameOver.SetActive(true);
            if (txtFinalScore != null)
            {
                txtFinalScore.text = $"Score: {currentScore}";
            }
        }
    }
    
    // UI 버튼 연결용
    public void RetryGame()
    {
        StartGame();
    }
}