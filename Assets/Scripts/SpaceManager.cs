using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SpaceManager : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab; 
    public GameObject itemPrefab;    
    public GameObject bossPrefab;    // 🔥 [추가] 보스 프리팹
    public Transform spawnContainer; 
    public GameObject playerObject;  
    
    [Header("Settings")]
    public float spawnInterval = 1.5f; 
    public float enemySpeed = 300f;    
    public float xLimit = 350f;        
    public int bossScoreThreshold = 1000; // 🔥 [추가] 보스 등장 점수

    [Header("UI")]
    public TMP_Text txtScore;
    public GameObject popupGameOver;
    public TMP_Text txtFinalScore;
    public TMP_Text txtNotice; // 🔥 [추가] "WARNING!" 같은 알림 텍스트

    private float spawnTimer = 0f;
    private float itemTimer = 5.0f; 
    private int score = 0;
    private bool isPlaying = true;
    private bool isBossPhase = false; // 🔥 [추가] 보스전 상태 체크

    // 🔥 스테이지 관리를 위한 변수 추가
    private int stage = 1;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (!isPlaying) return;

        // 보스전이면 쫄병/아이템 생성 중단
        if (isBossPhase) return;

        // 1. 적 생성
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }

        // 2. 아이템 생성
        itemTimer -= Time.deltaTime;
        if (itemTimer <= 0)
        {
            SpawnItem();
            itemTimer = Random.Range(10.0f, 20.0f); 
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float randomX = Random.Range(-xLimit, xLimit);
        Vector2 spawnPos = new Vector2(randomX, 1300f); 

        GameObject enemyObj = Instantiate(enemyPrefab, spawnContainer);
        RectTransform rect = enemyObj.GetComponent<RectTransform>();
        rect.anchoredPosition = spawnPos;

        SpaceEnemy enemyScript = enemyObj.GetComponent<SpaceEnemy>();
        if (enemyScript != null)
        {
            enemyScript.Init(this, enemySpeed);
        }
    }

    void SpawnItem()
    {
        if (itemPrefab == null) return;
        float randomX = Random.Range(-xLimit, xLimit);
        Vector2 spawnPos = new Vector2(randomX, 1300f);
        GameObject itemObj = Instantiate(itemPrefab, spawnContainer);
        RectTransform rect = itemObj.GetComponent<RectTransform>();
        rect.anchoredPosition = spawnPos;
    }

    // 🔥 [추가] 보스 소환 함수
    void SpawnBoss()
    {
        if (bossPrefab == null) return;

        isBossPhase = true; // 쫄병 생성 중지
        
        // 보스 알림
        if (txtNotice != null) 
        {
            txtNotice.gameObject.SetActive(true);
            txtNotice.text = "WARNING!\nBOSS APPROACHING";
            Invoke("HideNotice", 3.0f);
        }

        // 기존 적들 다 없애주기 (보스와 1:1)
        ClearNormalEnemies();

        // 보스 생성 (화면 위쪽)
        GameObject bossObj = Instantiate(bossPrefab, spawnContainer);
        RectTransform rect = bossObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 1500f); // 화면 밖 위에서 대기

        SpaceBoss bossScript = bossObj.GetComponent<SpaceBoss>();
        if (bossScript != null)
        {
            bossScript.Init(this);
        }
        
        Debug.Log("🦖 보스 등장!");
    }

    void HideNotice()
    {
        if (txtNotice != null) txtNotice.gameObject.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();

        // 🔥 보스 소환 체크 (보스전 아닐 때만)
        if (!isBossPhase && score >= bossScoreThreshold)
        {
            SpawnBoss();
        }
        // 난이도 상승 (보스전 아닐 때만)
        else if (!isBossPhase && score % 500 == 0)
        {
            spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.2f);
            enemySpeed = Mathf.Min(600f, enemySpeed + 50f);
        }
    }

    public void GameOver()
    {
        isPlaying = false;
        if (popupGameOver != null)
        {
            popupGameOver.SetActive(true);
            if (txtFinalScore != null) txtFinalScore.text = $"FAILED\nScore: {score}";
        }
    }
    
    // 🔥 [수정] 게임 클리어 -> 다음 스테이지 진행
    public void GameClear() // 보스 처치 시 호출됨
    {
        // 게임을 끝내지 않고 스테이지를 올림
        stage++;
        isBossPhase = false;
        
        // 다음 보스 컷 점수 높이기 (현재 점수 + 2000점)
        bossScoreThreshold += 2000;
        
        // 난이도 대폭 상승
        enemySpeed += 50f;
        spawnInterval = Mathf.Max(0.3f, spawnInterval - 0.2f);

        // 스테이지 알림
        if (txtNotice != null)
        {
            txtNotice.gameObject.SetActive(true);
            txtNotice.text = $"STAGE {stage} START!\nSPEED UP!";
            Invoke("HideNotice", 3.0f);
        }

        // 총알 등 청소
        ClearAllEntities();
    }

    void UpdateUI()
    {
        if (txtScore != null) txtScore.text = $"Score: {score}";
    }

    public void StartGame()
    {
        score = 0;
        spawnTimer = 0;
        itemTimer = 5.0f; 
        
        // 🔥 게임 초기화 시 난이도 리셋
        stage = 1;
        bossScoreThreshold = 1000;
        spawnInterval = 1.5f;
        enemySpeed = 300f;

        isPlaying = true;
        isBossPhase = false; // 초기화
        UpdateUI();
        
        if (popupGameOver != null) popupGameOver.SetActive(false);
        if (txtNotice != null) txtNotice.gameObject.SetActive(false); // 알림 끄기
        
        if (playerObject != null)
        {
             playerObject.SetActive(true); 
             playerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1250f);
             
             SpacePlayer playerScript = playerObject.GetComponent<SpacePlayer>();
             if (playerScript != null) playerScript.InitPlayer();
        }

        ClearAllEntities();
    }
    
    public void RetryGame()
    {
        StartGame();
    }

    void ClearAllEntities()
    {
        foreach(Transform child in spawnContainer)
        {
            if (child.CompareTag("Enemy") || child.CompareTag("EnemyBullet") || 
                child.CompareTag("PlayerBullet") || child.CompareTag("Item")) 
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    void ClearNormalEnemies()
    {
        foreach(Transform child in spawnContainer)
        {
            if (child.CompareTag("Enemy")) Destroy(child.gameObject);
        }
    }
}