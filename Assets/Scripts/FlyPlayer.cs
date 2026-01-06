using UnityEngine;

public class FlyPlayer : MonoBehaviour
{
    [Header("Settings")]
    public float gravity = 3000f; // 중력 (내려가는 힘)
    public float jumpForce = 1200f; // 점프 힘
    
    // 현재 속도 (양수면 위로, 음수면 아래로)
    private float velocity = 0f; 
    
    // 게임 오버 체크용
    public bool isDead = false;

    void Start()
    {
        // 시작할 때 시간 흐르게 (이전 게임 영향 방지)
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        if (isDead) return;

        // 1. 중력 적용 (속도가 점점 아래로 빨라짐)
        velocity -= gravity * Time.deltaTime;

        // 2. 점프 입력 (터치/클릭)
        if (Input.GetMouseButtonDown(0))
        {
            velocity = jumpForce; // 속도를 위쪽으로 확 바꿈
        }

        // 3. 이동 적용 (현재 위치 + 속도)
        transform.localPosition += Vector3.up * velocity * Time.deltaTime;

        // 4. 천장/바닥 충돌 처리
        // 화면 높이가 대충 2000px이니까 위아래 1000px 넘으면 사망
        if (transform.localPosition.y > 1000f || transform.localPosition.y < -1000f)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("으악! 추락!");
        
        // 여기에 나중에 매니저 연결해서 게임오버 팝업 띄울 거임
        Time.timeScale = 0; // 일단 멈춤
    }
}