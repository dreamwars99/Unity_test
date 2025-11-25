// 파일명: ClickCounter.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 텍스트를 다루기 위한 라이브러리 (파이썬의 import 같은 것)

public class ClickCounter : MonoBehaviour
{
    // 🐍 Python: count = 0
    // 내부에서 계산할 숫자 변수
    private int count = 0;

    // 🐍 Python: def __init__(self, text_ui): ...
    // 유니티 에디터(Inspector) 구멍을 뚫어서 연결할 변수 (public을 붙이면 에디터에서 보임!)
    public TMP_Text countText; 

    // 게임이 시작될 때 딱 한 번 실행되는 함수
    void Start()
    {
        count = 0;
        UpdateUI(); // 시작할 때 "0"이라고 표시
    }

    // 버튼이 눌릴 때 실행할 함수
    // 🐍 Python: def on_click():
    public void IncreaseScore()
    {
        count = 1 + count; // 숫자 1 증가
        UpdateUI(); // 화면 갱신
    }

    // 화면에 글자를 업데이트하는 함수
    void UpdateUI()
    {
        // 🐍 Python: print(f"Count: {count}")
        // C#에서는 숫자(int)를 문자열(string)로 바꿀 때 .ToString()을 써야 안전해.
        countText.text = "Count: " + count.ToString();
    }
}