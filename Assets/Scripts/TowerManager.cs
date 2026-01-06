using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;

    [Header("Settings")]
    public TowerBlock blockPrefab;
    public RectTransform container;
    
    // [수정 1] 시작 속도랑 높이 변수
    public float startSpeed = 1000f;
    public float startWidth = 300f;
    public float blockHeight = 80f;
    
    // [수정 2] 시작 Y 위치 (변수로 빼서 조절 쉽게 함)
    public float startY = -1200f; 
    
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    
    // [신규] 팝업 관련 변수 추가
    public GameObject gameOverPopup;       
    public TextMeshProUGUI finalScoreText; 

    // 내부 변수
    private TowerBlock currentBlock;
    private TowerBlock previousBlock;
    private float currentY;
    private int score = 0;
    private int highScore = 0;
    
    private float currentSpeed;
    private float currentWidth;
    private bool isGameOver = false;

    void Awake() { instance = this; }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("TowerBest", 0);
        
        // 시작하자마자 팝업은 꺼두기
        if(gameOverPopup != null) gameOverPopup.SetActive(false);
        
        GameStart();
    }

    public void GameStart()
    {
        score = 0;
        isGameOver = false;
        
        // [수정 1] 시작 위치를 더 아래로! (-500 -> startY)
        currentY = startY; 
        
        currentSpeed = startSpeed;
        currentWidth = startWidth;
        previousBlock = null;

        // 컨테이너 위치 원상복구
        container.anchoredPosition = Vector2.zero;
        
        // [신규] 팝업 끄기 (재시작일 때)
        if(gameOverPopup != null) gameOverPopup.SetActive(false);

        // 청소
        foreach (Transform child in container)
        {
            if (child.GetComponent<TowerBlock>()) Destroy(child.gameObject);
        }

        UpdateUI();
        SpawnBlock();
    }

    void Update()
    {
        // 게임 오버 상태면 터치 입력 무시 (Retry 버튼 눌러야 함)
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentBlock != null)
            {
                currentBlock.StopMoving();
                CheckPosition();
            }
        }
    }

    void CheckPosition()
    {
        float prevX = 0;
        if (previousBlock != null) prevX = previousBlock.transform.localPosition.x;

        float currentX = currentBlock.transform.localPosition.x;
        float diff = Mathf.Abs(currentX - prevX);

        if (diff > currentWidth)
        {
            GameOver();
        }
        else
        {
            previousBlock = currentBlock;
            NextLevel();
        }
    }

    void SpawnBlock()
    {
        GameObject obj = Instantiate(blockPrefab.gameObject, container);
        TowerBlock newBlock = obj.GetComponent<TowerBlock>();

        obj.transform.localPosition = new Vector3(0, currentY, 0);
        newBlock.Init(currentSpeed, 550f, currentWidth);

        currentBlock = newBlock;
    }

    void NextLevel()
    {
        score++;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("TowerBest", highScore);
        }
        UpdateUI();

        currentY += blockHeight;
        
        // 난이도 조절
        currentSpeed += 50f;
        if (currentSpeed > 2500f) currentSpeed = 2500f;
        
        currentWidth -= 10f;
        if (currentWidth < 50f) currentWidth = 50f;

        // 화면 내리기 (블록 3개 쌓이면)
        if (score > 15) container.anchoredPosition -= new Vector2(0, blockHeight);

        SpawnBlock();
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("게임 오버!");

        // [신규] 팝업 띄우기 로직
        if(finalScoreText != null) finalScoreText.text = "Final Score: " + score;
        if(gameOverPopup != null) gameOverPopup.SetActive(true);
    }

    void UpdateUI()
    {
        scoreText.text = score + " Floors";
        highScoreText.text = "Best: " + highScore;
    }
}