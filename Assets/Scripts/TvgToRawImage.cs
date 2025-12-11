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
        // 3. 엔진 체크
        if (thorEngine != null && targetUI != null)
        {
            // [수정된 부분] 안전장치 추가!
            // 불량 파일 때문에 재질(Material)이 아예 안 만들어졌을 경우를 대비함.
            if (thorEngine.sharedMaterial == null) 
            {
                return; // 재질 없으면 아무것도 하지 말고 리턴 (에러 방지)
            }

            // 이제 안전하게 접근 가능
            Texture renderedTexture = thorEngine.sharedMaterial.mainTexture;

            // UI 적용
            if (renderedTexture != null && targetUI.texture != renderedTexture)
            {
                targetUI.texture = renderedTexture;
            }
        }
    }
}