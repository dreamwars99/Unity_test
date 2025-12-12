using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 ImageLottiePlayer 옆에 붙여주면 돼
public class LottieFilterFix : MonoBehaviour
{
    private RawImage _rawImage;
    private bool _isFixed = false;

    void Start()
    {
        // 1. 내 몸에 붙은 RawImage 컴포넌트를 찾는다.
        _rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        // 이미 고쳤으면 더 이상 검사 안 함
        if (_isFixed) return;

        // 2. Lottie가 텍스처를 생성했는지 감시
        if (_rawImage != null && _rawImage.texture != null)
        {
            // 3. 텍스처가 생성되었다면 필터 모드를 확인
            if (_rawImage.texture.filterMode == FilterMode.Point)
            {
                // 4. "부드럽게(Bilinear)"로 강제 변경!
                _rawImage.texture.filterMode = FilterMode.Bilinear;
                
                // (선택) 테두리 깔끔하게
                _rawImage.texture.wrapMode = TextureWrapMode.Clamp; 
                
                Debug.Log("✨ Lottie 화질 보정 완료 (Point -> Bilinear)");
                _isFixed = true; // 작업 끝!
            }
        }
    }
}