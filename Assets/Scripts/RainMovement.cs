using UnityEngine;

public class RainMovement : MonoBehaviour
{
    public float speed = 500f;
    
    // 플레이어를 찾기 위한 변수
    private Transform playerTransform;

    void Start()
    {
        // 게임 시작하자마자 "Player"라는 이름의 오브젝트를 찾아서 기억해둠
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // 1. 비 내리기
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // 2. 화면 밖 삭제 부분 수정
        if (transform.localPosition.y < -1500)
        {
        // [신규] 살아남았다! 점수 추가!
        // 싱글톤 덕분에 연결 없이 바로 부를 수 있음
        DodgeManager.instance.AddScore(); 
        
        Destroy(gameObject);
        }
        
        // 3. [신규] 충돌 감지 (플레이어가 살아있을 때만)
        if (playerTransform != null)
        {
            // 거리 계산 (피타고라스 정리 같은 거)
            float distance = Vector3.Distance(transform.localPosition, playerTransform.localPosition);
            
            // 거리가 60 이하면 닿았다고 판정 (플레이어 크기 100의 절반 좀 넘게)
            if (distance < 60f)
            {
                // [신규] 매니저에게 게임 오버 알림
                DodgeManager.instance.GameOver(); 
                Destroy(gameObject); // 나는 삭제
                }       
        }
    }
}