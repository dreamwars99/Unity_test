using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoSpawner : MonoBehaviour
{
    [Header("--- Settings ---")]
    public GameObject obstaclePrefab; // 생성할 장애물 프리팹 (선인장)
    public Transform spawnPoint;      // 생성 위치

    [Header("--- Difficulty ---")]
    public float minTime = 1.0f;      // 초기 최소 간격
    public float maxTime = 3.0f;      // 초기 최대 간격
    
    private float timer = 0f;
    private float nextSpawnTime = 0f;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        // 게임오버면 장애물 생성 중단
        if (DinoManager.instance.isGameOver) return;

        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnObstacle();
            timer = 0f;
            SetNextSpawnTime();
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null || spawnPoint == null) return;

        // 심플하게 변경: 복잡한 패턴 없이 무조건 하나씩 생성! 🌵
        Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity, transform);
    }

    void SetNextSpawnTime()
    {
        // 난이도 조절: 점수가 높을수록 생성 간격만 빨라짐
        float score = DinoManager.instance.score;
        
        // 5000점이 되면 난이도 MAX
        // 너무 빠르면 못 피하니까 최소 0.8초 간격은 유지 (인간적인 난이도)
        float difficultyRatio = Mathf.Clamp01(score / 5000f); 

        // 점수가 오르면 생성 간격이 (1.0~3.0초)에서 -> (0.8~1.5초)로 줄어듦
        float currentMin = Mathf.Lerp(minTime, 0.8f, difficultyRatio);
        float currentMax = Mathf.Lerp(maxTime, 1.5f, difficultyRatio);

        nextSpawnTime = Random.Range(currentMin, currentMax);
    }
}