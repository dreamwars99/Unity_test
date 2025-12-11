using UnityEngine;
using UnityEngine.UI;

// 네가 찾아온 "3. 유니티 렌더링 통합"을 구현한 스크립트야.
public class TvgToRawImage : MonoBehaviour
{
    [Header("UI Output")]
    public RawImage targetUI; // 최종적으로 그림이 나올 곳

    [Header("Engine Source")]
    public MeshRenderer thorEngine; // ThorVG가 몰래 그림 그리고 있는 곳

    void Start()
    {
        // 1. 연결할 부품들 자동 찾기
        if (targetUI == null) targetUI = GetComponent<RawImage>();
        if (thorEngine == null) thorEngine = GetComponent<MeshRenderer>();

        // 2. 엔진이 멈춰있지 않게 확실히 켜두기 (화면엔 안보여도 돌아는 가야 함)
        if (thorEngine != null)
        {
            thorEngine.enabled = false; // 3D 월드에서는 숨김
        }
    }

    void Update()
    {
        // 3. 엔진에서 텍스처(Texture2D)를 훔쳐와서 UI에 꽂아넣기
        if (thorEngine != null && targetUI != null)
        {
            // ThorVG가 Mesh의 재질(Material)에 그림을 그리고 있음.
            Texture renderedTexture = thorEngine.sharedMaterial.mainTexture;

            // UI에 그 텍스처가 적용 안 되어 있다면 적용.
            if (renderedTexture != null && targetUI.texture != renderedTexture)
            {
                targetUI.texture = renderedTexture;
                Debug.Log($"[ThorVG Integrated] Texture Size: {renderedTexture.width}x{renderedTexture.height}");
            }
        }
    }
}