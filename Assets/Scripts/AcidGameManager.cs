using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; // [추가] UI 이벤트 시스템 제어용

public class AcidGameManager : MonoBehaviour
{
    public static AcidGameManager instance;

    [Header("Settings")]
    public GameObject wordPrefab;
    public Transform spawnArea;
    public Transform wordContainer;

    [Header("UI")]
    public TextMeshProUGUI txtInput;
    public TextMeshProUGUI txtScore;
    public TextMeshProUGUI txtBestScore;
    public TextMeshProUGUI txtStage;
    public TextMeshProUGUI txtHP;
    public GameObject gameOverPopup;
    public TextMeshProUGUI txtFinalScore;

    [Header("Game Data")]
    public int score = 0;
    public int bestScore = 0;
    public int hp = 5;
    public int stage = 1;

    private List<string> wordDatabase = new List<string>() 
    {
        "apple", "banana", "cherry", "unity", "csharp", "python", "code", "game",
        "rain", "acid", "korea", "seoul", "music", "piano", "mouse", "keyboard",
        "monitor", "screen", "pixel", "vector", "physics", "scene", "script",
        "bug", "error", "debug", "build", "player", "enemy", "level", "score",
        "start", "option", "exit", "class", "void", "public", "private", "return",
        "false", "true", "while", "break", "float", "string", "canvas", "image",
        "sound", "camera", "light", "shadow", "color", "power", "magic", "skill"
    };

    private List<AcidWord> activeWords = new List<AcidWord>();

    private float spawnTimer = 0f;
    private float spawnInterval = 2.0f;
    private string currentInput = "";
    private bool isGameOver = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("AcidBestScore", 0);
        GameStart();
    }

    void GameStart()
    {
        score = 0;
        hp = 5;
        stage = 1;
        currentInput = "";
        spawnInterval = 2.0f;
        isGameOver = false;
        Time.timeScale = 1.0f;
        
        // [수정 핵심] 게임 시작 시 UI 버튼에 남아있는 포커스(선택 상태)를 강제로 해제합니다.
        // 이렇게 해야 Enter 키를 눌렀을 때 다른 버튼이 눌리는 것을 방지할 수 있습니다.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        UpdateUI();
        txtInput.text = "";
        gameOverPopup.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnWord();
            spawnTimer = 0f;
        }

        DetectInput();
        
        // [추가 안전장치] 게임 중 마우스 클릭으로 버튼에 포커스가 다시 잡히는 것을 방지
        if (Input.GetMouseButtonDown(0) && EventSystem.current != null)
        {
             EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void SpawnWord()
    {
        int randomIndex = Random.Range(0, wordDatabase.Count);
        string randomWord = wordDatabase[randomIndex];

        float randomX = Random.Range(-400f, 400f);
        Vector3 spawnPos = new Vector3(randomX, spawnArea.localPosition.y, 0);

        GameObject newObj = Instantiate(wordPrefab, wordContainer);
        newObj.transform.localPosition = spawnPos;

        AcidWord newWordScript = newObj.GetComponent<AcidWord>();
        float speed = 150f + (stage * 30f); 
        newWordScript.SetWord(randomWord, speed);

        activeWords.Add(newWordScript);
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
             CheckWordMatch();
             currentInput = "";
             txtInput.text = "";
             return;
        }

        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentInput.Length > 0)
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
            }
            else if (c != '\n' && c != '\r' && c != ' ')
            {
                currentInput += c;
            }
        }

        txtInput.text = currentInput.ToUpper();
    }

    void CheckWordMatch()
    {
        string cleanInput = currentInput.Trim();
        AcidWord target = null;

        for (int i = 0; i < activeWords.Count; i++)
        {
            if (activeWords[i] == null) continue;

            if (activeWords[i].myWord.ToLower() == cleanInput.ToLower())
            {
                target = activeWords[i];
                break;
            }
        }

        if (target != null)
        {
            activeWords.Remove(target);
            Destroy(target.gameObject);
            AddScore(100);
        }
    }

    public void OnWordHitBottom(AcidWord word)
    {
        if (isGameOver) return;

        if (activeWords.Contains(word))
        {
            activeWords.Remove(word);
        }
        if (word != null) Destroy(word.gameObject);
        
        hp--;
        UpdateUI();

        if (hp <= 0)
        {
            GameOver();
        }
    }

    void AddScore(int amount)
    {
        score += amount;
        
        if (score >= stage * 300)
        {
            stage++;
            spawnInterval = Mathf.Max(0.5f, 2.0f - ((stage - 1) * 0.15f));
            Time.timeScale = 1.0f + ((stage - 1) * 0.05f);
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        txtScore.text = $"SCORE\n{score}";
        
        // [수정] 폰트 문제로 하트 이모지가 깨질 때, 'O'로 대체하고 빨간색으로 표시
        string heartString = "";
        for(int i=0; i<hp; i++) { heartString += "O "; } // 띄어쓰기로 간격 확보
        
        txtHP.text = heartString; 
        txtHP.color = Color.red; // 빨간색으로 강제 설정 (인스펙터 색상 무시)

        txtStage.text = $"STAGE {stage}";
        
        if (txtBestScore != null)
            txtBestScore.text = $"BEST: {bestScore}";
    }

    void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0;
        
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("AcidBestScore", bestScore);
            PlayerPrefs.Save();
        }

        if (txtFinalScore != null)
            txtFinalScore.text = $"Final Score: {score}";

        gameOverPopup.SetActive(true);
    }

    public void RetryGame()
    {
        for (int i = activeWords.Count - 1; i >= 0; i--)
        {
            if (activeWords[i] != null) Destroy(activeWords[i].gameObject);
        }
        activeWords.Clear();
        
        GameStart();
    }
}