using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LottieSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject slavePrefab; // 아까 만든 껍데기 (Lottie_Slave)
    public Transform spawnRoot;    // 생성될 위치 (Panel_Super)
    public int count = 100;        // 목표 개수

    [Header("Master Source")]
    public MeshRenderer masterEngine; // 숨겨진 Master (Lottie_Player)

    void Start()
    {
        SpawnSlaves();
    }

    void SpawnSlaves()
    {
        // 1. 마스터의 텍스처를 가져온다 (참조)
        // 주의: 마스터가 아직 초기화 안 됐을 수 있으니 실제 적용은 Update에서 하거나, 
        // 여기서 텍스처를 못 찾으면 Slave들이 알아서 기다리게 해야 함.
        
        for (int i = 0; i < count; i++)
        {
            GameObject newObj = Instantiate(slavePrefab, spawnRoot);
            
            // 랜덤 위치 (화면 안쪽 대충)
            float x = Random.Range(-400f, 400f);
            float y = Random.Range(-800f, 800f);
            newObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // [핵심] 텍스처 공유 연결
            // 여기서는 간단하게 컴포넌트를 붙여서 해결하자.
            var linker = newObj.AddComponent<SlaveLinker>();
            linker.master = masterEngine;
        }
    }
}