using UnityEngine;
using UnityEngine.UI; // UI를 다루기 위해 필요

public class LottieTransmitter : MonoBehaviour
{
    // 우리가 만든 '방송용 텍스처'를 여기에 연결할 거야
    public RenderTexture broadcastTexture;
    
    private Image myImage;

    void Start()
    {
        // 내 몸에 붙어있는 Image 컴포넌트를 찾는다
        myImage = GetComponent<Image>();
    }

    void Update()
    {
        // 로티가 그려놓은 그림(mainTexture)을 -> 방송용 텍스처(broadcastTexture)로 복사!
        if (myImage.mainTexture != null)
        {
            Graphics.Blit(myImage.mainTexture, broadcastTexture);
        }
    }
}