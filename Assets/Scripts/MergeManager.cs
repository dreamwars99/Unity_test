using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MergeManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject ballPrefab; // Ball_Base 프리팹
    public RectTransform spawnPoint;  // 공 떨어지는 위치
    public RectTransform gameContainer; // 공들이 담길 부모 객체
    
    [Header("Input Settings")]
    public float gameWidth = 800f; // 게임 화면 너비 (Inspector에서 조정 가능)
    public float padding = 40f;    // 양옆 여백 (공이 벽에 끼지 않게)

    [Header("Colors")]
    public Color[] levelColors; // 레벨별 색상

    [Header("UI")]
    public TMP_Text txtScore;
    public GameObject popupGameOver;

    private GameObject currentBall; // 지금 손에 들고 있는 공
    private bool isReady = false;   // 공을 떨어트릴 준비가 됐나?
    private int score = 0;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        // 안전장치: 들고 있던 공이 밖에서 터졌다면(버그 등) 다시 생성
        if (isReady && currentBall == null)
        {
            isReady = false;
            SpawnNewBall();
            return;
        }

        if (!isReady || currentBall == null) return;

        HandleInput();
    }

    void HandleInput()
    {
        // 1. 항상 마우스 X좌표를 따라다님 (조준)
        MoveSpawnPoint();

        // 2. 클릭 순간 즉시 투하!
        if (Input.GetMouseButtonDown(0))
        {
            DropBall();
        }
    }

    void MoveSpawnPoint()
    {
        float ratio = Input.mousePosition.x / Screen.width;
        float targetX = (ratio - 0.5f) * gameWidth;
        float limit = (gameWidth * 0.5f) - padding; 
        targetX = Mathf.Clamp(targetX, -limit, limit);

        Vector2 newPos = spawnPoint.anchoredPosition;
        newPos.x = targetX;
        spawnPoint.anchoredPosition = newPos;

        if (currentBall != null)
        {
            currentBall.GetComponent<RectTransform>().anchoredPosition = spawnPoint.anchoredPosition;
        }
    }

    public void StartGame()
    {
        score = 0;
        isReady = false;
        
        RefreshScoreUI();
        if(popupGameOver != null) popupGameOver.SetActive(false);
        
        if (gameContainer != null)
        {
            foreach(Transform child in gameContainer)
            {
                if(child.name.Contains("Ball")) Destroy(child.gameObject);
            }
        }

        SpawnNewBall();
    }

    void SpawnNewBall()
    {
        int randomLevel = Random.Range(0, 3);
        
        GameObject obj = Instantiate(ballPrefab, gameContainer);
        RectTransform ballRect = obj.GetComponent<RectTransform>();
        ballRect.anchoredPosition = spawnPoint.anchoredPosition;

        // 1. 물리 끄기 (위치 고정)
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true; 
        rb.velocity = Vector2.zero;

        // 🔥 [핵심 수정] 2. 충돌 판정 끄기 (유령 모드)
        // 손에 들고 있을 때 밑에 있는 공이랑 합체되는 사고 방지!
        Collider2D col = obj.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        MergeBall ballLogic = obj.GetComponent<MergeBall>();
        ballLogic.Init(randomLevel, this);

        currentBall = obj;
        isReady = true;
    }

    void DropBall()
    {
        if(currentBall == null) return;

        // 1. 물리 켜기 (떨어짐)
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 10.0f; // 묵직하게

        // 🔥 [핵심 수정] 2. 충돌 판정 다시 켜기 (실체화)
        Collider2D col = currentBall.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        
        currentBall = null; 
        isReady = false;    

        // 0.05초 딜레이 (너무 빠르면 시각적으로 겹쳐 보일 수 있어서 약간 여유 둠)
        Invoke("SpawnNewBall", 0.05f); 
    }

    public void MergeBalls(MergeBall ball1, MergeBall ball2)
    {
        // 안전장치: 이미 파괴된 공이면 무시
        if (ball1 == null || ball2 == null) return;

        Vector2 pos1 = ball1.GetComponent<RectTransform>().anchoredPosition;
        Vector2 pos2 = ball2.GetComponent<RectTransform>().anchoredPosition;
        Vector2 centerPos = (pos1 + pos2) / 2f;

        int nextLevel = ball1.level + 1;
        score += (nextLevel * 10);
        
        RefreshScoreUI();

        Destroy(ball1.gameObject);
        Destroy(ball2.gameObject);

        if (nextLevel < levelColors.Length)
        {
            GameObject newObj = Instantiate(ballPrefab, gameContainer);
            RectTransform newRect = newObj.GetComponent<RectTransform>();
            newRect.anchoredPosition = centerPos;
            
            Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 10.0f;
            rb.AddForce(Vector2.up * 300f, ForceMode2D.Impulse);

            // 생성된 합체 공은 당연히 Collider가 켜져 있어야 함 (Prefab 기본 상태)
            
            MergeBall newBallLogic = newObj.GetComponent<MergeBall>();
            newBallLogic.Init(nextLevel, this);
        }
    }

    public Color GetLevelColor(int level)
    {
        if (levelColors != null && level < levelColors.Length)
        {
            Color c = levelColors[level];
            c.a = 1.0f; 
            return c;
        }
        return Color.white;
    }

    void RefreshScoreUI()
    {
        if(txtScore != null) txtScore.text = score.ToString();
    }
}