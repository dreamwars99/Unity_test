using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    public GameObject rainPrefab; // 아까 만든 붕어빵 틀
    public float spawnInterval = 0.5f; // 0.5초마다 생성
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // [신규] 난이도 조절 로직
        // 점수가 높을수록 간격이 줄어듦 (최소 0.1초까지)
        // 현재 점수 가져오기
        int currentScore = DodgeManager.instance.score;
        
        // 공식: 기본 0.5초 - (점수 * 0.01초) 
        // 예: 10점이면 0.4초, 40점이면 0.1초
        float currentInterval = spawnInterval - (currentScore * 0.01f);
        
        // 아무리 빨라도 0.1초보다 빠르면 안됨 (Mathf.Max)
        currentInterval = Mathf.Max(0.1f, currentInterval);

        if (timer > currentInterval) // 변한 간격 적용
        {
            SpawnRain();
            timer = 0f;
        }
    }

    void SpawnRain()
    {
        // 1. 프리팹 복제 (Instantiate)
        GameObject newRain = Instantiate(rainPrefab);

        // 2. 부모 설정 (Game_DodgeRain 안으로 들어가게)
        // 이걸 안 하면 UI 좌표가 꼬여서 화면에 안 보일 수 있어!
        newRain.transform.SetParent(transform.parent, false);

        // 3. 랜덤 위치 설정 (X좌표 랜덤)
        // Panel 크기에 따라 다르겠지만 대략 -400 ~ 400 사이
        float randomX = Random.Range(-650f, 650f);
        
        // Y축은 화면 위쪽(600), Z는 0
        newRain.transform.localPosition = new Vector3(randomX, 1500, 0);
    }
}