using UnityEngine;
using TMPro;

public class TetrisManager : MonoBehaviour
{
    // 어디서든 접근 가능한 싱글톤
    public static TetrisManager instance;

    [Header("Settings")]
    public int width = 10;
    public int height = 20;
    public Transform origin; // Board_Origin (기준점)
    
    [Header("Prefabs")]
    public GameObject[] tetrominos; 
    public Transform spawnPoint;

    [Header("UI")]
    public TextMeshProUGUI txtScore;
    public GameObject popupGameOver;

    // 핵심 데이터
    private Transform[,] grid; 
    private int score = 0;

    void Awake()
    {
        instance = this;
        grid = new Transform[width, height]; 
    }

    void OnEnable()
    {
        ResetGame();
    }

    // --- [추가됨] Retry 버튼에 연결할 함수 ---
    public void RetryGame()
    {
        Debug.Log("🔄 Retry Game!");
        ResetGame();
    }

    void ResetGame()
    {
        score = 0;
        UpdateUI();
        popupGameOver.SetActive(false);

        // 1. 그리드 데이터 및 씬의 블록 청소
        for(int x=0; x<width; x++) {
            for(int y=0; y<height; y++) {
                if(grid[x, y] != null) {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
        
        // 2. 현재 떨어지고 있는(아직 그리드에 등록 안 된) 블록들도 찾아서 삭제
        TetrisPiece[] pieces = FindObjectsOfType<TetrisPiece>();
        foreach(var p in pieces) {
            Destroy(p.gameObject);
        }

        // 3. 새 블록 생성
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (popupGameOver.activeSelf) return;

        int index = Random.Range(0, tetrominos.Length);
        
        // SpawnPoint 위치에 생성
        GameObject go = Instantiate(tetrominos[index], spawnPoint.position, Quaternion.identity);
        
        // [안전장치] 생성되자마자 위치가 유효하지 않으면 (이미 꽉 찼거나 벽 밖)
        if (!IsValidMove(go.transform))
        {
            Debug.LogError("❌ Game Over: 스폰 위치가 막혀있거나 벽 밖입니다! SpawnPoint 위치를 확인하세요.");
            Destroy(go); 
            GameOver(); 
            return;      
        }
    }

    public void AddToGrid(Transform block, int x, int y)
    {
        // 정상 범위 안이라면 그리드에 등록
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            grid[x, y] = block;
        }
        else
        {
            // 범위를 벗어남 (천장 뚫음) -> 게임 오버
            Debug.Log($"💀 Game Over: 블록이 천장을 뚫었습니다. (Y: {y}, Height Limit: {height})");
            GameOver();
        }
    }

    public bool IsValidMove(Transform piece)
    {
        foreach (Transform child in piece)
        {
            int roundedX = Mathf.RoundToInt(child.position.x - origin.position.x) / 100;
            int roundedY = Mathf.RoundToInt(child.position.y - origin.position.y) / 100;

            // 1. 벽 밖으로 나갔니? (좌, 우, 바닥)
            if (roundedX < 0 || roundedX >= width || roundedY < 0)
                return false;

            // 2. 다른 블록이 이미 있니? (천장 위인 경우는 검사하지 않음 -> 낙하 중에는 허용)
            if (roundedY < height && grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }

    public void CheckForLines()
    {
        for (int i = height - 1; i >= 0; i--) 
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
                i++; // 다시 같은 줄 검사
                
                score += 100;
                UpdateUI();
            }
        }
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null) return false;
        }
        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] != null)
            {
                Destroy(grid[j, i].gameObject);
                grid[j, i] = null;
            }
        }
    }

    void RowDown(int i)
    {
        for (int y = i + 1; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].localPosition += new Vector3(0, -100, 0);
                }
            }
        }
    }

    void UpdateUI()
    {
        if(txtScore != null) txtScore.text = "Score: " + score;
    }

    public void GameOver()
    {
        popupGameOver.SetActive(true);
    }
    
    // UI 버튼 연결용
    public void ButtonInput(string command)
    {
        if (popupGameOver.activeSelf) return; // 게임오버 상태면 조작 불가

        TetrisPiece piece = FindObjectOfType<TetrisPiece>();
        if(piece == null || !piece.enabled) return;

        if(command == "Left") piece.Move(new Vector3(-100, 0, 0));
        if(command == "Right") piece.Move(new Vector3(100, 0, 0));
        if(command == "Rotate") piece.Rotate();
        if(command == "Drop") piece.fallTime = 0.05f; 
    }
}