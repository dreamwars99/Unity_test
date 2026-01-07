using UnityEngine;

public class MergeWallBuilder : MonoBehaviour
{
    [Header("Wall Settings")]
    public float wallThickness = 50f; // 벽 두께
    
    // 🔥 수정 완료: 사용자가 직접 조정한 값 반영
    public float floorHeight = -350f;    
    
    public float gameWidth = 800f;    // 게임 유효 너비
    
    // 🔥 수정 완료: 게임 높이를 넉넉하게 늘림
    public float gameHeight = 2100f;  

    void Start()
    {
        BuildWalls();
    }

    // 인스펙터의 컴포넌트 이름 위에서 우클릭 -> "Rebuild Walls Now" 선택하면 실행됨
    [ContextMenu("Rebuild Walls Now")] 
    public void RebuildWallsManual()
    {
        // 기존 벽 삭제
        Transform oldWalls = transform.Find("AutoWalls");
        if (oldWalls != null) DestroyImmediate(oldWalls.gameObject);

        BuildWalls();
    }

    void BuildWalls()
    {
        // 이미 벽이 있으면 만들지 않음
        if (transform.Find("AutoWalls") != null) return;

        // 벽들을 담을 부모 객체 생성
        GameObject wallsRoot = new GameObject("AutoWalls");
        wallsRoot.transform.SetParent(transform, false);

        // 1. 바닥 (Bottom)
        // 위치 계산: -1050 + (-350) = -1400 (화면 중앙 기준 아래로 1400)
        float bottomY = -gameHeight / 2 + floorHeight;

        CreateWall(wallsRoot, "Wall_Bottom", 
            new Vector2(gameWidth + 200, wallThickness), 
            new Vector2(0, bottomY));

        // 2. 왼쪽 벽 (Left)
        CreateWall(wallsRoot, "Wall_Left", 
            new Vector2(wallThickness, gameHeight * 2), 
            new Vector2(-gameWidth / 2 - wallThickness / 2, 0));

        // 3. 오른쪽 벽 (Right)
        CreateWall(wallsRoot, "Wall_Right", 
            new Vector2(wallThickness, gameHeight * 2), 
            new Vector2(gameWidth / 2 + wallThickness / 2, 0));
            
        Debug.Log($"🧱 [MergeWallBuilder] 벽 공사 완료! 바닥 위치(Y): {bottomY}");
    }

    void CreateWall(GameObject parent, string name, Vector2 size, Vector2 position)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent.transform, false);
        
        RectTransform rect = wall.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
    }
}