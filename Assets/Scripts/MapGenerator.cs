using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject rowPrefab;     // 생성할 프리팹 (Row_center)
    public Transform contentParent;  // ScrollView의 Content
    public int totalUnits = 20;      // 생성할 개수
    
    [Header("Scroll Settings (New!)")]
    public float rowHeight = 200f;   // 버튼 하나당 높이 (간격 포함, 적당히 조절)
    public float bottomPadding = 500f; // 맨 아래 여유 공간

    [Header("Zigzag Layout")]
    public float xAmplitude = 200f;  // 좌우 너비
    public float frequency = 0.5f;   // 굴곡 빈도

    [Header("Design Assets")]
    public Color activeColor = new Color(0.8f, 0.28f, 0.65f); // 핑크 (#CE49A8)
    public Color lockedColor = new Color(0.75f, 0.75f, 0.75f); // 회색 (#BEBEBE)
    
    // 아이콘 이미지 리스트 (0:별, 1:헤드셋, 2:비디오...)
    public List<Sprite> iconList; 

    void Start()
    {
        GenerateMap();
    }

    // 우클릭 메뉴로 에디터에서 실행 가능
    [ContextMenu("Generate Map Now")]
    public void GenerateMap()
    {
        // 1. 기존 생성된 버튼들 청소 (조건부 삭제)
        if (contentParent.childCount > 0)
        {
            // 역순으로 검사하면서 지우기
            for (int i = contentParent.childCount - 1; i >= 0; i--)
            {
                GameObject childObj = contentParent.GetChild(i).gameObject;

                // 💡 이름에 "Row_Unit"이 포함된 녀석만 지운다!
                if (childObj.name.Contains("Row_Unit"))
                {
                    DestroyImmediate(childObj);
                }
            }
        }

        // 2. 새로운 버튼 생성 루프
        for (int i = 0; i < totalUnits; i++)
        {
            SpawnUnitButton(i);
        }

        // 3. [추가됨] 스크롤 영역(Content) 높이 강제 늘리기
        UpdateContentHeight();
    }

    // 🔥 핵심: 버튼 개수에 맞춰서 스크롤 길이를 늘려주는 함수
    void UpdateContentHeight()
    {
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        
        // 공식: (버튼 개수 * 버튼 하나 높이) + 여유 공간
        float finalHeight = (totalUnits * rowHeight) + bottomPadding;
        
        // Content의 높이를 적용
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, finalHeight);
    }

    void SpawnUnitButton(int index)
    {
        // 프리팹 생성
        GameObject newRow = Instantiate(rowPrefab, contentParent);
        newRow.name = $"Row_Unit_{index + 1}";

        // 1. 버튼 위치 찾기 (Row의 첫 번째 자식이 버튼이라고 가정)
        Transform btnTransform = newRow.transform.GetChild(0); 

        // 2. 지그재그 위치 계산 (Sin 파형)
        float xPos = Mathf.Sin(index * frequency) * xAmplitude;
        btnTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0);

        // 3. 디자인 변경 로직 (Img_Main 직접 수정)
        // 경로: Row -> Btn_Unit_Active -> Img_Main
        Transform imgMainTr = btnTransform.Find("Img_Main");
        
        if (imgMainTr != null)
        {
            Image mainImage = imgMainTr.GetComponent<Image>();

            // [중요] 아이콘 비율 유지 (찌그러짐 방지)
            mainImage.preserveAspect = true;

            if (index == 0)
            {
                // [Case A] 현재 학습 중 (0번)
                mainImage.color = activeColor;

                // 이미지: 리스트의 0번 (별)
                if (iconList.Count > 0) 
                    mainImage.sprite = iconList[0];
            }
            else
            {
                // [Case B] 잠김 (나머지)
                mainImage.color = lockedColor;

                // 이미지: 리스트의 1번부터 순환 (별 제외)
                if (iconList.Count > 1)
                {
                    // 1, 2, 3 ... 패턴 반복
                    int cycleIndex = (index % (iconList.Count - 1)) + 1;
                    mainImage.sprite = iconList[cycleIndex];
                }
            }
        }
    }
}