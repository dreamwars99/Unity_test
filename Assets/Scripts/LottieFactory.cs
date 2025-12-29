using UnityEngine;
using UnityEngine.UI;

public class LottieFactory : MonoBehaviour
{
    [Header("Blueprint")]
    public GameObject slavePrefab;   // 복제할 껍데기 (RawImage 프리팹)
    public Transform spawnRoot;      // 생성될 부모 위치 (Panel_Super)
    public int count = 50;           // 생성 개수

    [Header("Broadcast Signal")]
    public Texture sharedTexture;    // 우리가 만든 Lottie_Screen (RenderTexture)

    void Start()
    {
        SpawnTVs();
    }

    void SpawnTVs()
    {
        // 안전장치
        if (sharedTexture == null)
        {
            Debug.LogError("방송 신호(Texture)가 없습니다!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            // 1. 껍데기 생성
            GameObject newTV = Instantiate(slavePrefab, spawnRoot);
            
            // 2. 위치 랜덤 배치 (화면 안쪽)
            float x = Random.Range(-400f, 400f);
            float y = Random.Range(-800f, 800f);
            newTV.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // 3. 채널 고정 (핵심 최적화)
            // 매 프레임 검사할 필요 없이, 태어날 때 딱 한 번만 연결하면 됨.
            RawImage rawImage = newTV.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = sharedTexture; // 텍스처 주소만 복사 (메모리 0)
                rawImage.color = Color.white;     // 색상 초기화
            }
        }
    }
}