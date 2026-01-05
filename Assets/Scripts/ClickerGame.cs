using UnityEngine;
using TMPro;

public class ClickerGame : MonoBehaviour
{
    [Header("UI Objects")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI upgradeText; 
    public TextMeshProUGUI highScoreText; // [신규] 최고 기록 텍스트

    [Header("Game Data")]
    private int score = 0;
    private int clickPower = 1;
    private int upgradeCost = 10;
    
    // [신규] 최고 점수 변수
    private int highScore = 0; 

    void Start()
    {
        // [신규] 저장된 최고 점수 불러오기
        // "HighScore"라는 키로 저장된 값이 있으면 가져오고, 없으면 0을 가져와.
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        UpdateUI();
    }

    public void OnClickButton()
    {
        score += clickPower;
        
        // [신규] 랭킹 갱신 로직
        if (score > highScore)
        {
            highScore = score;
            // 즉시 저장 (파이썬의 file.write)
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        
        UpdateUI();
    }

    public void OnClickUpgrade()
    {
        if (score >= upgradeCost)
        {
            score -= upgradeCost;
            clickPower += 1;
            upgradeCost *= 2; 
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        scoreText.text = score.ToString() + " Gems";
        upgradeText.text = $"Level Up\n(Cost: {upgradeCost})";
        
        // [신규] 최고 기록 UI 표시
        highScoreText.text = $"🏆 Best: {highScore}";
    }
    
    // [개발자용] 테스트하다가 기록 초기화하고 싶을 때 쓰는 함수
    // 유니티 에디터 상단 메뉴나 버튼을 따로 만들어서 연결하면 됨 (지금은 코드로만 존재)
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        highScore = 0;
        score = 0;
        clickPower = 1;
        upgradeCost = 10;
        UpdateUI();
        Debug.Log("데이터 초기화 완료!");
    }
}