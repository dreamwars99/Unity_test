using UnityEngine;

public class FlySpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float baseInterval = 1.5f; // 기본 생성 간격
    
    private float timer = 0f;
    private float nextSpawnTime = 0f; // 다음엔 언제 쏠까?

    void Start()
    {
        SetNextSpawnTime(); // 시작할 때 첫 시간 계산
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > nextSpawnTime)
        {
            SpawnPipe();
            timer = 0f;
            SetNextSpawnTime(); // 쏘고 나서 다시 계산
        }
    }

    // [신규] 점수가 높으면 빨리 쏘고, 약간의 랜덤(엇박자)을 섞는 함수
    void SetNextSpawnTime()
    {
        // 1. 점수 기반 빨라짐 (기본값에서 점수*0.05초 만큼 뺌)
        int score = FlyManager.instance.score;
        float difficulty = score * 0.05f; 
        
        // 최소 0.8초보다는 빠르게 안 나옴 (너무 빠르면 불가능하니까)
        float currentInterval = Mathf.Max(0.8f, baseInterval - difficulty);

        // 2. 랜덤 엇박자 추가 (0초 ~ 0.5초 사이 랜덤)
        float randomDelay = Random.Range(0f, 0.5f);

        // 최종 시간 = 계산된 간격 + 랜덤 딜레이
        nextSpawnTime = currentInterval + randomDelay;
    }

    void SpawnPipe()
    {
        GameObject newPipe = Instantiate(pipePrefab, transform);
        
        // 높이 랜덤 설정 (-300 ~ 300)
        float randomY = Random.Range(-300f, 300f);
        newPipe.transform.localPosition = new Vector3(600f, randomY, 0);
    }
}