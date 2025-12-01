using UnityEngine;
using UnityEngine.UI; // 이미지 제어용
using TMPro;

public class ItemToggle : MonoBehaviour
{
    [Header("바꿔줄 녀석들")]
    public Image background;      // 배경판
    public Outline outline;       // 테두리
    public Image checkboxBox;     // 체크박스 네모
    public GameObject checkmark;  // 체크표시(V) 그림

    [Header("색상 설정")]
    // 선택 안 됐을 때 (하얀색, 회색)
    private Color bgNormal = Color.white;
    private Color outlineNormal = new Color32(229, 229, 229, 255); // #E5E5E5

    // 선택 됐을 때 (하늘색, 파란색)
    private Color bgSelected = new Color32(221, 244, 255, 255);    // #DDF4FF
    private Color outlineSelected = new Color32(28, 176, 246, 255); // #1CB0F6

    private bool isSelected = false; // 🐍 Python: flag 변수

    // 버튼 누르면 실행될 함수
    public void Toggle()
    {
        isSelected = !isSelected; // True <-> False 뒤집기
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (isSelected)
        {
            // 🟦 파란색 모드
            background.color = bgSelected;
            outline.effectColor = outlineSelected;
            checkboxBox.color = outlineSelected; // 체크박스도 파랗게
            checkmark.SetActive(true);           // V 표시 켜기
        }
        else
        {
            // ⬜ 흰색 모드
            background.color = bgNormal;
            outline.effectColor = outlineNormal;
            checkboxBox.color = Color.white;     // 체크박스는 다시 하얗게
            checkmark.SetActive(false);          // V 표시 끄기
        }
    }
}