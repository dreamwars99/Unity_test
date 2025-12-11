using UnityEngine;      // <- 이게 없어서 MonoBehaviour, MeshRenderer 에러가 난 거야
using UnityEngine.UI;   // <- 이게 없어서 RawImage 에러가 난 거야

public class SlaveLinker : MonoBehaviour
{
    public MeshRenderer master;
    private RawImage myImage;

    void Start()
    {
        myImage = GetComponent<RawImage>();
        
        // UV 뒤집힘 해결 (필요시)
        if (myImage != null)
        {
            myImage.uvRect = new Rect(0, 1, 1, -1); 
        }
    }

    void Update()
    {
        // 마스터와 나(UI)가 존재하고, 텍스처가 다를 때만 갱신
        if (master != null && myImage != null)
        {
            // 마스터의 재질(Material)이 있는지 체크 (NullReference 방지)
            if (master.sharedMaterial != null && 
                myImage.texture != master.sharedMaterial.mainTexture)
            {
                myImage.texture = master.sharedMaterial.mainTexture;
            }
        }
    }
}