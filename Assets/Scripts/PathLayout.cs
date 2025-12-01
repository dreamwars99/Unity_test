using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // LayoutRebuilder를 위해 필요

[ExecuteAlways]
public class PathLayout : MonoBehaviour
{
    public float spacing = 300f;     // 버튼 사이 간격
    public float amplitude = 250f;   // 지그재그 좌우 폭
    public float frequency = 1.5f;   // 지그재그 빈도
    public float xOffset = 0f;       // 전체적인 좌우 이동 미세조정

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // 게임 실행 시, UI 크기가 확정될 때까지 한 프레임 대기 후 정렬
        if (Application.isPlaying)
        {
            StartCoroutine(ArrangeAfterFrame());
        }
    }

    void Update()
    {
        // 에디터 모드(Scene 뷰)에서는 실시간으로 갱신하여 바로 확인 가능하게 함
        if (!Application.isPlaying)
        {
            ArrangeButtons();
        }
    }
    
    // [핵심 수정] UI 레이아웃이 다 그려진 뒤에 버튼 위치 계산
    IEnumerator ArrangeAfterFrame()
    {
        yield return null; // 1프레임 대기 (중요!)
        ArrangeButtons();
    }

    public void ArrangeButtons()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        // [중요] 만약 폭이 0으로 잡히면 강제로 1080으로 가정 (안전장치)
        float width = rectTransform.rect.width;
        if (width < 100) width = 1080f; 

        // 버튼들을 지그재그로 배치
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            RectTransform childRect = child.GetComponent<RectTransform>();

            // Y축: 위에서부터 아래로 spacing 만큼씩 내림 (i가 커질수록 아래로)
            // 컨테이너의 중심이 (0,0)이라고 가정할 때, 위쪽으로 올라가려면 -가 아니라 +가 될 수도 있으나
            // 보통 ScrollView Content 안에서는 위에서 아래로 -방향으로 배치함.
            // 하지만 여기서는 ButtonContainer가 길어지므로, 단순히 위에서부터 i * spacing 만큼 띄움
            
            // 사인 그래프 공식: sin(인덱스 * 빈도) * 진폭
            float x = Mathf.Sin(i * frequency) * amplitude + xOffset;
            float y = -(i * spacing) - (spacing / 2); // 첫 버튼도 약간 아래서 시작

            childRect.anchoredPosition = new Vector2(x, y);
        }

        // [레이아웃 자동 높이 조절]
        // 자식 개수에 맞춰서 컨테이너의 높이(Height)를 부모에게 보고
        // LayoutElement 컴포넌트가 있어야 작동함
        LayoutElement le = GetComponent<LayoutElement>();
        if (le != null)
        {
            float requiredHeight = (childCount * spacing) + 200f; // 여유분 200
            le.minHeight = requiredHeight;
            le.preferredHeight = requiredHeight;
        }
    }
}