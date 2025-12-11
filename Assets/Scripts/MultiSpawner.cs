using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject slavePrefab; // 아까 만든 껍데기 (Lottie_Slave)
    public Transform spawnRoot;    // Panel_Super
    public int count = 50;         // 테스트 개수

    [Header("Masters Collection")]
    public MeshRenderer[] masterEngines; // 여기에 Master들을 다 넣을 거야

    void Start()
    {
        SpawnMixedSlaves();
    }

    public void SpawnMixedSlaves()
    {
        for (int i = 0; i < count; i++)
        {
            // 1. 랜덤으로 마스터 하나 선정 (룰렛 돌리기)
            int randomIndex = Random.Range(0, masterEngines.Length);
            MeshRenderer selectedMaster = masterEngines[randomIndex];

            // 2. Slave 생성
            GameObject newObj = Instantiate(slavePrefab, spawnRoot);
            
            // 위치 랜덤 (화면 안쪽)
            float x = Random.Range(-400f, 400f);
            float y = Random.Range(-800f, 800f);
            newObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // 3. 선택된 마스터와 연결 (이전에 만든 Linker 재사용)
            var linker = newObj.AddComponent<SlaveLinker>();
            linker.master = selectedMaster; // <- 여기가 핵심
        }
    }
}