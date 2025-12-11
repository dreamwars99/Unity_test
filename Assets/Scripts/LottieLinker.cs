using UnityEngine;
using UnityEngine.UI;

public class LottieLinker : MonoBehaviour
{
    private RawImage myScreen;
    private GameObject masterObject;

    void Start()
    {
        // 1. 내 몸에 있는 RawImage 찾기
        myScreen = GetComponent<RawImage>();

        // 2. 씬에 있는 Master 찾기
        masterObject = GameObject.Find("Lottie_Master");

        // 3. 연결 시도
        if (masterObject != null && myScreen != null)
        {
            // 약간의 딜레이를 줘서 마스터가 준비될 시간을 붎
            Invoke("SyncTexture", 0.1f);
        }
        else
        {
            Debug.LogError("❌ Linker 실패: 마스터나 RawImage를 못 찾겠어.");
        }
    }

    void SyncTexture()
    {
        var meshRenderer = masterObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            var texture = meshRenderer.material.mainTexture;
            
            // [추가] 텍스처를 부드럽게 처리하라 (깍두기 방지)
            texture.filterMode = FilterMode.Bilinear; 

            myScreen.texture = texture;
        }
    }
}