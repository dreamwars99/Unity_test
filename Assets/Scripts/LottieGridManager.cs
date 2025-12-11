using UnityEngine;

public class LottieGridManager : MonoBehaviour
{
    [Header("설정")]
    public GameObject lottiePrefab;
    public Transform parentPanel;
    public int spawnCount = 100;

    void Start()
    {
        if (lottiePrefab == null || parentPanel == null) return;

        // 기존 삭제
        foreach (Transform child in parentPanel)
        {
            Destroy(child.gameObject);
        }

        // 100개 생성
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(lottiePrefab, parentPanel);
        }
        
        Debug.Log("✅ 생성 완료!");
    }
}