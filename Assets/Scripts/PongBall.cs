using UnityEngine;

public class PongBall : MonoBehaviour
{
    public float startSpeed = 1000f; 
    private Rigidbody2D rb;
    private Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.localPosition;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Launch()
    {
        transform.localPosition = startPosition;
        rb.velocity = Vector2.zero; 

        // 랜덤 발사 (아래쪽 or 위쪽)
        float x = Random.Range(-0.5f, 0.5f);
        float y = Random.Range(0, 2) == 0 ? -1f : 1f;

        Vector2 direction = new Vector2(x, y).normalized;
        
        // 스테이지에 따른 속도 보정은 Manager에서 처리하거나 여기서 기본값 사용
        float speedMultiplier = 1.0f + (PongManager.instance.stage * 0.1f);
        rb.velocity = direction * (startSpeed * speedMultiplier);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // [추가] "Player" 태그가 붙은 패들에 닿았을 때만 점수 획득 (랠리 점수)
        if (collision.gameObject.CompareTag("Player"))
        {
            PongManager.instance.AddScore(1);
        }

        // 튕길 때마다 속도 증가
        float currentSpeed = rb.velocity.magnitude;
        rb.velocity = rb.velocity.normalized * (currentSpeed * 1.05f);
    }
}