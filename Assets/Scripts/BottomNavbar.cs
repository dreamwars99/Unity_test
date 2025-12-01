using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BottomNavbar : MonoBehaviour
{
    [Header("탭 버튼들 (순서: 0:학습 ~ 5:더보기)")]
    public List<Button> tabButtons;

    [Header("메인 패널들 (순서: 0:학습 ~ 4:슈퍼)")]
    // 주의: 더보기(5번)용 패널은 여기 넣지 마세요!
    public List<GameObject> contentPanels; 

    [Header("팝업 요소")]
    public GameObject popupMore; // 하얀색 팝업창
    public GameObject dimmer;    // 뒤에 깔리는 검은 배경

    [Header("스타일 설정")]
    public Color selectedBgColor = new Color32(221, 244, 255, 255); // 선택됨 (하늘색)
    public Color unselectedBgColor = new Color(1, 1, 1, 0);         // 안됨 (투명)

    // 팝업을 열기 직전에 보고 있던 탭 번호를 기억하는 변수
    private int lastMainTabIndex = 0;

    void Start()
    {
        // 1. 시작할 때 팝업과 배경은 숨겨둡니다.
        ClosePopup();

        // 2. 버튼들에 클릭 기능을 연결합니다.
        for (int i = 0; i < tabButtons.Count; i++)
        {
            int index = i; // (중요) 이렇게 해야 숫자가 안 꼬입니다.
            tabButtons[i].onClick.AddListener(() => OnTabClicked(index));
        }
        
        // 3. 앱 켜지면 0번(학습) 탭을 보여줍니다.
        UpdateTabUI(0);
    }

    // ✨ 외부(Dimmer 등)에서 호출할 수 있는 팝업 닫기 함수
    public void ClosePopup()
    {
        if (popupMore != null) popupMore.SetActive(false);
        if (dimmer != null) dimmer.SetActive(false);
        
        // 팝업이 닫히면, 원래 보고 있던 탭에 다시 파란불을 켜줍니다.
        HighlightButton(lastMainTabIndex);
    }

    // 탭 버튼을 눌렀을 때 실행되는 메인 로직
    void OnTabClicked(int index)
    {
        // [CASE 1] 5번 '더보기(점3개)' 버튼을 눌렀을 때
        if (index == 5) 
        {
            // 팝업이 지금 꺼져있는가? (꺼져있으면 true, 켜져있으면 false)
            bool isOpening = !popupMore.activeSelf; 

            if (isOpening)
            {
                // 열기 모드: 팝업과 배경을 켜고, 5번 버튼에 파란불 킴
                if(popupMore != null) popupMore.SetActive(true);
                if(dimmer != null) dimmer.SetActive(true);
                HighlightButton(5); 
            }
            else
            {
                // 닫기 모드: 팝업 닫기 함수 호출
                ClosePopup();
            }
            return; // 팝업만 처리하고 여기서 끝냄
        }

        // [CASE 2] 일반 탭(0~4번)을 눌렀을 때
        lastMainTabIndex = index; // 현재 위치를 기억해둠 (나중에 돌아오려고)

        // 혹시 팝업이 열려있었다면 닫아줌
        ClosePopup();

        // 화면과 버튼 색깔을 업데이트함
        UpdateTabUI(index);
    }

    // 버튼의 파란 박스(Outline)와 배경색을 켜고 끄는 함수
    void HighlightButton(int targetIndex)
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            Outline outline = tabButtons[i].GetComponent<Outline>();
            Image bg = tabButtons[i].GetComponent<Image>();

            // 혹시 컴포넌트가 없으면 에러 나니까 체크
            if(bg == null) continue;

            if (i == targetIndex)
            {
                // 선택된 버튼: 파란 박스 켜기
                if(outline != null) outline.enabled = true;
                bg.color = selectedBgColor;
            }
            else
            {
                // 나머지 버튼: 투명하게
                if(outline != null) outline.enabled = false;
                bg.color = unselectedBgColor;
            }
        }
    }

    // 중앙 패널 화면을 교체하는 함수
    void UpdateTabUI(int activeIndex)
    {
        // 1. 버튼 스타일 먼저 맞추고
        HighlightButton(activeIndex);

        // 2. 패널들을 돌아가며 켜고 끄기
        for (int i = 0; i < contentPanels.Count; i++)
        {
            if (contentPanels[i] == null) continue;

            if (i == activeIndex)
                contentPanels[i].SetActive(true);
            else
                contentPanels[i].SetActive(false);
        }
    }
}